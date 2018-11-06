using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Modification.Operations.Selection;
using Sledge.BspEditor.Primitives.MapData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.BspEditor.Rendering.Resources;
using Sledge.BspEditor.Rendering.Viewport;
using Sledge.BspEditor.Tools.Draggable;
using Sledge.BspEditor.Tools.Properties;
using Sledge.BspEditor.Tools.Vertex.Selection;
using Sledge.BspEditor.Tools.Vertex.Tools;
using Sledge.Common.Shell.Components;
using Sledge.Common.Shell.Documents;
using Sledge.Common.Shell.Hooks;
using Sledge.Common.Shell.Hotkeys;
using Sledge.Common.Translations;
using Sledge.DataStructures.Geometric;
using Sledge.Providers.Texture;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Pipelines;
using Sledge.Rendering.Primitives;
using Sledge.Rendering.Resources;
using Sledge.Shell.Input;

namespace Sledge.BspEditor.Tools.Vertex
{
    [Export(typeof(ITool))]
    [Export(typeof(IInitialiseHook))]
    [Export]
    [OrderHint("P")]
    [AutoTranslate]
    [DefaultHotkey("Shift+V")]
    public class VertexTool : BaseDraggableTool, IInitialiseHook
    {
        private readonly IEnumerable<Lazy<VertexSubtool>> _subTools;

        private readonly VertexSelection _selection;

        [ImportingConstructor]
        public VertexTool(
            [ImportMany] IEnumerable<Lazy<VertexSubtool>> subTools
        )
        {
            _subTools = subTools;

            Usage = ToolUsage.Both;

            _selection = new VertexSelection();
        }

        public Task OnInitialise()
        {
            foreach (var st in _subTools.OrderBy(x => x.Value.OrderHint))
            {
                st.Value.Selection = _selection;
                st.Value.Active = Children.Count == 0;
                Children.Add(st.Value);
            }

            return Task.FromResult(false);
        }

        public override Image GetIcon()
        {
            return Resources.Tool_VM;
        }

        public override string GetName()
        {
            return "Vertex Manipulation Tool";
        }

        protected override IEnumerable<Subscription> Subscribe()
        {
            yield return Oy.Subscribe<IDocument>("MapDocument:SelectionChanged", x =>
            {
                if (x == GetDocument()) SelectionChanged();
            });
            yield return Oy.Subscribe<Type>("VertexTool:SetSubTool", t =>
            {
                CurrentSubTool = _subTools.Select(x => x.Value).FirstOrDefault(x => x.GetType() == t);
            });
            yield return Oy.Subscribe<String>("VertexTool:Reset", async _ =>
            {
                var document = GetDocument();
                if (document != null) await _selection.Reset(document);
                CurrentSubTool?.Update();
                Invalidate();
            });
        }

        public override async Task ToolSelected()
        {
            await SelectionChanged();
            var ct = CurrentSubTool;
            if (ct != null) await ct.ToolSelected();
            await base.ToolSelected();
        }

        public override async Task ToolDeselected()
        {
            var document = GetDocument();
            if (document != null)
            {
                await _selection.Commit(document);
                await _selection.Clear(document);
            }

            var ct = CurrentSubTool;
            if (ct != null) await ct.ToolDeselected();
            await base.ToolDeselected();
        }

        private async Task SelectionChanged()
        {
            var document = GetDocument();
            if (document != null)
            {
                await _selection.Commit(document);
                await _selection.Update(document);
            }

            var ct = CurrentSubTool;
            if (ct != null) await ct.SelectionChanged();
            Invalidate();
        }

        #region Tool switching
        
        internal VertexSubtool CurrentSubTool
        {
            get { return Children.OfType<VertexSubtool>().FirstOrDefault(x => x.Active); }
            set
            {
                foreach (var tool in Children.Where(x => x != value && x.Active))
                {
                    tool.ToolDeselected();
                    tool.Active = false;
                }
                if (value != null)
                {
                    value.Active = true;
                    value.ToolSelected();
                }

                Oy.Publish("VertexTool:SubToolChanged", value?.GetType());
            }
        }

        #endregion

        private void SelectObject(MapDocument document, Solid closestObject)
        {
            // Nothing was clicked, don't change the selection
            if (closestObject == null) return;

            var operation = new Transaction();

            // Ctrl doesn't toggle selection, only adds to it.
            // Ctrl+clicking a selected solid will do nothing.

            if (!KeyboardState.Ctrl)
            {
                // Ctrl isn't down, so we want to clear the selection
                operation.Add(new Deselect(document.Selection.Where(x => !ReferenceEquals(x, closestObject)).ToList()));
            }

            if (!closestObject.IsSelected)
            {
                // The clicked object isn't selected yet, select it.
                operation.Add(new Select(closestObject));
            }

            if (!operation.IsEmpty)
            {
                MapDocumentOperation.Perform(document, operation);
            }
        }
        
        #region 3D interaction

        /// <summary>
        /// When the mouse is pressed in the 3D view, we want to select the clicked object.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="viewport">The viewport that was clicked</param>
        /// <param name="camera"></param>
        /// <param name="e">The click event</param>
        protected override void MouseDown(MapDocument document, MapViewport viewport, PerspectiveCamera camera, ViewportEvent e)
        {
            // First, get the ray that is cast from the clicked point along the viewport frustrum
            var (start, end) = camera.CastRayFromScreen(new Vector3(e.X, e.Y, 0));
            var ray = new Line(start, end);

            // Grab all the elements that intersect with the ray
            var closestObject = document.Map.Root.GetIntersectionsForVisibleObjects(ray)
                .Where(x => x.Object is Solid)
                .Select(x => (Solid) x.Object)
                .FirstOrDefault();

            SelectObject(document, closestObject);
        }

        #endregion

        #region 2D interaction

        private IEnumerable<IMapObject> GetLineIntersections(MapDocument document, Box box)
        {
            return document.Map.Root.Collect(
                x => x is Root || (x.BoundingBox != null && x.BoundingBox.IntersectsWith(box)),
                x => x.Hierarchy.Parent != null && !x.Hierarchy.HasChildren && x is Solid && x.GetPolygons().Any(p => p.GetLines().Any(box.IntersectsWith))
            );
        }

        private IMapObject SelectionTest(MapDocument document, OrthographicCamera camera, ViewportEvent e)
        {
            // Create a box to represent the click, with a tolerance level
            var unused = camera.GetUnusedCoordinate(new Vector3(100000, 100000, 100000));
            var tolerance = 4 / camera.Zoom; // Selection tolerance of four pixels
            var used = camera.Expand(new Vector3(tolerance, tolerance, 0));
            var add = used + unused;
            var click = camera.ScreenToWorld(e.X, e.Y);
            var box = new Box(click - add, click + add);
            return GetLineIntersections(document, box).FirstOrDefault();
        }

        protected override void MouseDown(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e)
        {
            // Get the first element that intersects with the box, selecting or deselecting as needed
            var closestObject = SelectionTest(document, camera, e) as Solid;
            SelectObject(document, closestObject);
        }

        #endregion

        protected override void Render(MapDocument document, BufferBuilder builder, ResourceCollector resourceCollector)
        {
            base.Render(document, builder, resourceCollector);

            // Force this work to happen on a new thread so waiting on it won't block the context
            Task.Run(async () =>
            {
                foreach (var obj in _selection)
                {
                    await Convert(builder, document, obj.Copy, resourceCollector);
                }
            }).Wait();
        }

        public void Invalidate()
        {
            Oy.Publish("VertexTool:Updated", _selection);
        }

        private async Task Convert(BufferBuilder builder, MapDocument document, MutableSolid solid, ResourceCollector resourceCollector)
        {
            var displayFlags = document.Map.Data.GetOne<DisplayFlags>();
            var hideNull = displayFlags?.HideNullTextures == true;

            var faces = solid.Faces.Where(x => x.Vertices.Count > 2).ToList();

            // Pack the vertices like this [ f1v1 ... f1vn ] ... [ fnv1 ... fnvn ]
            var numVertices = (uint)faces.Sum(x => x.Vertices.Count);

            // Pack the indices like this [ solid1 ... solidn ] [ wireframe1 ... wireframe n ]
            var numSolidIndices = (uint)faces.Sum(x => (x.Vertices.Count - 2) * 3);
            var numWireframeIndices = numVertices * 2;

            var points = new VertexStandard[numVertices];
            var indices = new uint[numSolidIndices + numWireframeIndices];
            
            var tint = Color.FromArgb(128, 255, 128).ToVector4();

            var tc = await document.Environment.GetTextureCollection();

            var vi = 0u;
            var si = 0u;
            var wi = numSolidIndices;
            foreach (var face in faces)
            {
                var opacity = tc.GetOpacity(face.Texture.Name);
                var t = await tc.GetTextureItem(face.Texture.Name);
                var w = t?.Width ?? 0;
                var h = t?.Height ?? 0;

                var tintModifier = new Vector4(1, 1, 1, opacity);

                var offs = vi;
                var numFaceVerts = (uint)face.Vertices.Count;

                var textureCoords = face.GetTextureCoordinates(w, h).ToList();

                var normal = face.Plane.Normal;
                for (var i = 0; i < face.Vertices.Count; i++)
                {
                    var v = face.Vertices[i];
                    points[vi++] = new VertexStandard
                    {
                        Position = v.Position,
                        Colour = Vector4.One,
                        Normal = normal,
                        Texture = new Vector2(textureCoords[i].Item2, textureCoords[i].Item3),
                        Tint = tint * tintModifier,
                        Flags = VertexFlags.None
                    };
                }

                // Triangles - [0 1 2]  ... [0 n-1 n]
                for (uint i = 2; i < numFaceVerts; i++)
                {
                    indices[si++] = offs;
                    indices[si++] = offs + i - 1;
                    indices[si++] = offs + i;
                }

                // Lines - [0 1] ... [n-1 n] [n 0]
                for (uint i = 0; i < numFaceVerts; i++)
                {
                    indices[wi++] = offs + i;
                    indices[wi++] = offs + (i == numFaceVerts - 1 ? 0 : i + 1);
                }
            }

            var groups = new List<BufferGroup>();

            uint texOffset = 0;
            foreach (var f in faces)
            {
                var texInd = (uint)(f.Vertices.Count - 2) * 3;

                if (hideNull && tc.IsNullTexture(f.Texture.Name))
                {
                    texOffset += texInd;
                    continue;
                }

                var opacity = tc.GetOpacity(f.Texture.Name);
                var t = await tc.GetTextureItem(f.Texture.Name);
                var transparent = opacity < 0.95f || t?.Flags.HasFlag(TextureFlags.Transparent) == true;

                var texture = t == null ? string.Empty : $"{document.Environment.ID}::{f.Texture.Name}";

                groups.Add(transparent
                    ? new BufferGroup(PipelineType.TexturedAlpha, CameraType.Perspective, f.Origin, texture, texOffset, texInd)
                    : new BufferGroup(PipelineType.TexturedOpaque, CameraType.Perspective, texture, texOffset, texInd)
                );

                texOffset += texInd;

                if (t != null) resourceCollector.RequireTexture(t.Name);
            }

            groups.Add(new BufferGroup(PipelineType.Wireframe, CameraType.Both, numSolidIndices, numWireframeIndices));

            builder.Append(points, indices, groups);
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows.Forms;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Modification.Operations.Selection;
using Sledge.BspEditor.Modification.Operations.Tree;
using Sledge.BspEditor.Primitives;
using Sledge.BspEditor.Primitives.MapData;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.BspEditor.Rendering.Resources;
using Sledge.BspEditor.Rendering.Viewport;
using Sledge.BspEditor.Tools.Draggable;
using Sledge.BspEditor.Tools.Properties;
using Sledge.Common.Shell.Components;
using Sledge.Common.Shell.Context;
using Sledge.Common.Shell.Hotkeys;
using Sledge.Common.Shell.Settings;
using Sledge.Common.Translations;
using Sledge.DataStructures.Geometric;
using Sledge.Providers.Texture;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Engine;
using Sledge.Rendering.Pipelines;
using Sledge.Rendering.Primitives;
using Sledge.Rendering.Resources;

namespace Sledge.BspEditor.Tools.Brush
{
    [Export(typeof(ITool))]
    [Export(typeof(ISettingsContainer))]
    [Export]
    [OrderHint("H")]
    [AutoTranslate]
    [DefaultHotkey("Shift+B")]
    public class BrushTool : BaseDraggableTool, ISettingsContainer
    {
        [Import] private EngineInterface _engine;

        private bool _updatePreview;
        private List<IMapObject> _preview;
        private BoxDraggableState box;
        private IBrush _activeBrush;

        public bool RoundVertices { get; set; } = true;

        public string CreateObject { get; set; } = "Create Object";
        
        // Settings

        [Setting("SelectionBoxBackgroundOpacity")] private int _selectionBoxBackgroundOpacity = 64;
        [Setting("SelectCreatedBrush")] private bool _selectCreatedBrush = true;
        [Setting("SwitchToSelectAfterBrushCreation")] private bool _switchToSelectAfterCreation = false;
        [Setting("ResetBrushTypeOnCreation")] private bool _resetBrushTypeOnCreation = false;

        string ISettingsContainer.Name => "Sledge.BspEditor.Tools.BrushTool";

        IEnumerable<SettingKey> ISettingsContainer.GetKeys()
        {
            yield return new SettingKey("Tools/Brush", "SelectionBoxBackgroundOpacity", typeof(int));
            yield return new SettingKey("Tools/Brush", "SelectCreatedBrush", typeof(bool));
            yield return new SettingKey("Tools/Brush", "SwitchToSelectAfterBrushCreation", typeof(bool));
            yield return new SettingKey("Tools/Brush", "ResetBrushTypeOnCreation", typeof(bool));
        }

        void ISettingsContainer.LoadValues(ISettingsStore store)
        {
            store.LoadInstance(this);
        }

        void ISettingsContainer.StoreValues(ISettingsStore store)
        {
            store.StoreInstance(this);
        }

        public BrushTool()
        {
            box = new BoxDraggableState(this);
            box.BoxColour = Color.Turquoise;
            box.FillColour = Color.FromArgb(_selectionBoxBackgroundOpacity, Color.Green);
            box.State.Changed += BoxChanged;
            States.Add(box);
        }

        protected override IEnumerable<Subscription> Subscribe()
        {
            yield return Oy.Subscribe<Change>("MapDocument:Changed", x =>
            {
                if (x.Document == GetDocument())
                {
                    TextureSelected();
                }
                return Task.FromResult(0);
            });
            yield return Oy.Subscribe<object>("BrushTool:ValuesChanged", x =>
            {
                _updatePreview = true;
            });
            yield return Oy.Subscribe<RightClickMenuBuilder>("MapViewport:RightClick", x =>
            {
                x.Clear();
                x.AddCallback(String.Format(CreateObject, _activeBrush?.Name), () =>
                {
                    var doc = GetDocument();
                    if (doc != null) Confirm(doc, x.Viewport);
                });
            });
        }

        protected override void ContextChanged(IContext context)
        {
            _activeBrush = context.Get<IBrush>("BrushTool:ActiveBrush");
            _updatePreview = true;

            base.ContextChanged(context);
        }

        public override async Task ToolSelected()
        {
            var document = GetDocument();
            if (document == null) return;

            var sel = document.Selection.OfType<Solid>().ToList();
            if (sel.Any())
            {
                box.RememberedDimensions = new Box(sel.Select(x => x.BoundingBox));
            }
            else if (box.RememberedDimensions == null)
            {
                var gs = document.Map.Data.GetOne<GridData>()?.Grid;
                var start = Vector3.Zero;
                var next = gs?.AddStep(Vector3.Zero, Vector3.One) ?? Vector3.One * 64;
                box.RememberedDimensions = new Box(start, next);
            }

            _updatePreview = true;
            await base.ToolSelected();
        }

        public override async Task ToolDeselected()
        {
            _updatePreview = false;
            await base.ToolDeselected();
        }

        private void TextureSelected()
        {
            _updatePreview = true;
        }

        private void BoxChanged(object sender, EventArgs e)
        {
            _updatePreview = true;
        }

        public override Image GetIcon()
        {
            return Resources.Tool_Brush;
        }

        public override string GetName()
        {
            return "BrushTool";
        }

        protected override void KeyDown(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e)
        {
            if (e.KeyCode == Keys.Enter) Confirm(document, viewport);
            else if (e.KeyCode == Keys.Escape) Cancel(document, viewport);

            base.KeyDown(document, viewport, camera, e);
        }

        protected override void KeyDown(MapDocument document, MapViewport viewport, PerspectiveCamera camera, ViewportEvent e)
        {
            if (e.KeyCode == Keys.Enter) Confirm(document, viewport);
            else if (e.KeyCode == Keys.Escape) Cancel(document, viewport);

            base.KeyDown(document, viewport, camera, e);
        }

        private void CreateBrush(MapDocument document, Box bounds)
        {
            var brush = GetBrush(document, bounds, document.Map.NumberGenerator);
            if (brush == null) return;
            
            var transaction = new Transaction();

            transaction.Add(new Attach(document.Map.Root.ID, brush));

            if (_selectCreatedBrush)
            {
                transaction.Add(new Deselect(document.Selection));
                transaction.Add(new Select(brush.FindAll()));
            }

            MapDocumentOperation.Perform(document, transaction);
        }

        private IMapObject GetBrush(MapDocument document, Box bounds, UniqueNumberGenerator idg)
        {
            var brush = _activeBrush;
            if (brush == null) return null;

            // Don't round if the box is rather small
            var rounding = RoundVertices ? 0 : 2;
            if (bounds.SmallestDimension < 10) rounding = 2;

            var ti = document.Map.Data.GetOne<ActiveTexture>()?.Name ?? "aaatrigger";
            var created = brush.Create(idg, bounds, ti, rounding).ToList();

            // Align all textures to the face and set the texture scale
            foreach (var f in created.SelectMany(x => x.Data.OfType<Face>()))
            {
                f.Texture.XScale = f.Texture.YScale = (float) document.Environment.DefaultTextureScale;
                f.Texture.AlignToNormal(f.Plane.Normal);
            }

            // If there's more than one object in the result, group them up
            if (created.Count > 1)
            {
                var g = new Group(idg.Next("MapObject"));
                created.ForEach(x => x.Hierarchy.Parent = g);
                g.DescendantsChanged();
                return g;
            }

            return created.FirstOrDefault();
        }

        private void Confirm(MapDocument document, MapViewport viewport)
        {
            if (box.State.Action != BoxAction.Drawn) return;
            var bbox = new Box(box.State.Start, box.State.End);
            if (!bbox.IsEmpty())
            {
                CreateBrush(document, bbox);
                box.RememberedDimensions = bbox;
            }
            _preview = null;
            box.State.Action = BoxAction.Idle;
            if (_switchToSelectAfterCreation)
            {
                Oy.Publish("ActivateTool", "SelectTool");
            }
            if (_resetBrushTypeOnCreation)
            {
                Oy.Publish("BrushTool:ResetBrushType", this);
            }
        }

        private void Cancel(MapDocument document, MapViewport viewport)
        {
            box.RememberedDimensions = new Box(box.State.Start, box.State.End);
            _preview = null;
            box.State.Action = BoxAction.Idle;
        }

        private List<IMapObject> GetPreview(MapDocument document)
        {
            if (_updatePreview)
            {
                var bbox = new Box(box.State.Start, box.State.End);
                var brush = GetBrush(document, bbox, new UniqueNumberGenerator()).FindAll();
                _preview = brush;
            }

            _updatePreview = false;
            return _preview ?? new List<IMapObject>();
        }

        protected override void Render(MapDocument document, BufferBuilder builder, ResourceCollector resourceCollector)
        {
            if (box.State.Action != BoxAction.Idle)
            {
                // Force this work to happen on a new thread so waiting on it won't block the context
                Task.Run(async () =>
                {
                    foreach (var obj in GetPreview(document).OfType<Solid>())
                    {
                        await Convert(builder, document, obj, resourceCollector);
                    }
                }).Wait();
            }

            base.Render(document, builder, resourceCollector);
        }

        private async Task Convert(BufferBuilder builder, MapDocument document, IMapObject obj, ResourceCollector resourceCollector)
        {
            var solid = (Solid)obj;
            var faces = solid.Faces.Where(x => x.Vertices.Count > 2).ToList();

            // Pack the vertices like this [ f1v1 ... f1vn ] ... [ fnv1 ... fnvn ]
            var numVertices = (uint)faces.Sum(x => x.Vertices.Count);

            // Pack the indices like this [ solid1 ... solidn ] [ wireframe1 ... wireframe n ]
            var numSolidIndices = (uint)faces.Sum(x => (x.Vertices.Count - 2) * 3);
            var numWireframeIndices = numVertices * 2;

            var points = new VertexStandard[numVertices];
            var indices = new uint[numSolidIndices + numWireframeIndices];

            var c = Color.Turquoise;
            var colour = new Vector4(c.R, c.G, c.B, c.A) / 255f;

            c = Color.FromArgb(192, Color.Turquoise);
            var tint = new Vector4(c.R, c.G, c.B, c.A) / 255f;

            var tc = await document.Environment.GetTextureCollection();

            var vi = 0u;
            var si = 0u;
            var wi = numSolidIndices;
            foreach (var face in faces)
            {
                var t = await tc.GetTextureItem(face.Texture.Name);
                var w = t?.Width ?? 0;
                var h = t?.Height ?? 0;

                var offs = vi;
                var numFaceVerts = (uint)face.Vertices.Count;

                var textureCoords = face.GetTextureCoordinates(w, h).ToList();

                var normal = face.Plane.Normal;
                for (var i = 0; i < face.Vertices.Count; i++)
                {
                    var v = face.Vertices[i];
                    points[vi++] = new VertexStandard
                    {
                        Position = v,
                        Colour = colour,
                        Normal = normal,
                        Texture = new Vector2(textureCoords[i].Item2, textureCoords[i].Item3),
                        Tint = tint,
                        Flags = t == null ? VertexFlags.FlatColour : VertexFlags.None
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

            groups.Add(new BufferGroup(PipelineType.Wireframe, solid.IsSelected ? CameraType.Both : CameraType.Orthographic, numSolidIndices, numWireframeIndices));

            builder.Append(points, indices, groups);
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows.Forms;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Modification.Operations;
using Sledge.BspEditor.Modification.Operations.Data;
using Sledge.BspEditor.Modification.Operations.Selection;
using Sledge.BspEditor.Primitives;
using Sledge.BspEditor.Primitives.MapData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.BspEditor.Rendering.Viewport;
using Sledge.BspEditor.Tools.Properties;
using Sledge.Common.Shell.Components;
using Sledge.Common.Shell.Hotkeys;
using Sledge.DataStructures.Geometric;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Pipelines;
using Sledge.Rendering.Primitives;
using Sledge.Rendering.Resources;
using Face = Sledge.BspEditor.Primitives.MapObjectData.Face;
using KeyboardState = Sledge.Shell.Input.KeyboardState;

namespace Sledge.BspEditor.Tools.Texture
{
    [Export(typeof(ITool))]
    [Export]
    [OrderHint("J")]
    [DefaultHotkey("Shift+A")]
    public class TextureTool : BaseTool
    {
        public TextureTool()
        {
            Usage = ToolUsage.View3D;
            UseValidation = true;
        }

        public override Image GetIcon()
        {
            return Resources.Tool_Texture;
        }

        public override string GetName()
        {
            return "Texture Application Tool";
        }

        public FaceSelection GetSelection()
        {
            var fs = Document.Map.Data.GetOne<FaceSelection>();
            if (fs == null)
            {
                fs = new FaceSelection();
                Document.Map.Data.Add(fs);
            }
            return fs;
        }

        private bool ShouldHideFaceMask => Document?.Map.Data.GetOne<HideFaceMask>()?.Hidden == true;

        private void SetActiveTexture(ITextured tex)
        {
            if (tex == null) return;

            var at = new ActiveTexture {Name = tex.Texture.Name};
            MapDocumentOperation.Perform(Document, new TrivialOperation(x => x.Map.Data.Replace(at), x => x.Update(at)));
        }

        private void SetFaceSelectionFromObjectSelection()
        {
            var sel = GetSelection();
            sel.Clear();
            foreach (var obj in Document.Selection)
            {
                sel.Add(obj, obj.Data.OfType<Face>().ToArray());
            }
            SetActiveTexture(sel.FirstOrDefault());
        }

        protected override void DocumentChanged()
        {
            SetFaceSelectionFromObjectSelection();
            base.DocumentChanged();
        }

        protected override IEnumerable<Subscription> Subscribe()
        {
            yield return Oy.Subscribe<Change>("MapDocument:Changed", DocumentUpdated);

            yield return Oy.Subscribe<RightClickMenuBuilder>("MapViewport:RightClick", b =>
            {
                b.Intercepted = true;
            });
        }

        private async Task DocumentUpdated(Change change)
        {
            if (change.Document == Document)
            {
                if (change.HasObjectChanges && change.Updated.Intersect(GetSelection().GetSelectedParents()).Any())
                {
                    Invalidate();
                }
                else if (change.HasDataChanges && change.AffectedData.Any(x => x is HideFaceMask))
                {
                    Invalidate();
                }
            }
        }

        public override async Task ToolSelected()
        {
            SetFaceSelectionFromObjectSelection();

            /*
               Bypass normal operations - this won't ever appear in the history!
               Means our undo stack gets corrupted, but since we don't have knowledge of history
               at this point (and don't really care too much), we're just sitting in the world
               of "eh, whatever" at this point. Just deselect everything and stop worrying!
               Hammer 4 seems to have similar behaviour. It's just easier...
               This is just for rendering anyway so who cares.
            */

            await MapDocumentOperation.Bypass(Document, new Deselect(Document.Selection));
            await base.ToolSelected();
        }
        
        public override async Task ToolDeselected()
        {
            GetSelection().Clear();
            await base.ToolDeselected();
        }

        private IEnumerable<IMapObject> GetBoundingBoxIntersections(DataStructures.Geometric.Line ray)
        {
            return Document.Map.Root.Collect(
                x => x is Root || (x.BoundingBox != null && x.BoundingBox.IntersectsWith(ray)),
                x => x.Hierarchy.Parent != null && !x.Hierarchy.HasChildren
            );
        }

        protected override void MouseDown(MapViewport viewport, PerspectiveCamera camera, ViewportEvent e)
        {
            var vp = viewport;
            if (vp == null || (e.Button != MouseButtons.Left && e.Button != MouseButtons.Right)) return;

            var (start, end) = camera.CastRayFromScreen(new Vector3(e.X, e.Y, 0));
            var ray = new Line(start, end);
            var hits = GetBoundingBoxIntersections(ray);

            var clickedFace = hits.OfType<Solid>().SelectMany(a => a.Faces.Select(f => new { Face = f, Solid = a }))
                .Select(x => new {x.Face, x.Solid, Intersection = new Polygon(x.Face.Vertices).GetIntersectionPoint(ray) })
                .Where(x => x.Intersection != null)
                .OrderBy(x => (x.Intersection.Value - ray.Start).Length())
                .Select(x => x)
                .FirstOrDefault();

            if (clickedFace == null) return;

            if (e.Button == MouseButtons.Left) SelectFace(clickedFace.Face, clickedFace.Solid);
            else if (e.Button == MouseButtons.Right) ApplyFace(clickedFace.Face, clickedFace.Solid);
        }

        private async Task SelectFace(Face face, IMapObject parent)
        {
            // Left:       lift + select
            // Alt+Left:   lift
            // Shift+Left: lift + select all
            // Ctrl+Left:  use toggle selection

            var sel = GetSelection();

            // Only sample the face if we're selecting it
            if (!KeyboardState.Ctrl || !sel.IsSelected(parent, face))
            {
                SetActiveTexture(face);
            }

            // If alt is down, just sample texture without changing selection
            if (KeyboardState.Alt) return;
            
            // Clear selection if ctrl isn't down
            if (!KeyboardState.Ctrl) sel.Clear();

            // Get the list of faces to toggle selection
            var faces = new HashSet<Face> { face };

            // If shift is down, select all
            if (KeyboardState.Shift) faces.UnionWith(parent.Data.OfType<Face>());

            // Toggle selection of all faces
            foreach (var tf in faces)
            {
                if (sel.IsSelected(parent, tf)) sel.Remove(parent, tf);
                else sel.Add(parent, tf);
            }

            Oy.Publish("TextureTool:SelectionChanged", GetSelection());

            Invalidate();
        }

        private async Task ApplyFace(Face face, IMapObject parent)
        {
            // Right:       apply + values
            // Alt+Right:   apply + align
            // Shift+Right: apply only
            // Ctrl+Right:  nothing...?

            var sampleFace = GetSelection().FirstOrDefault();

            var activeTexture = Document.Map.Data.GetOne<ActiveTexture>()?.Name ?? sampleFace?.Texture.Name ?? "";
            if (String.IsNullOrWhiteSpace(activeTexture)) return;

            var alignTexture = KeyboardState.Alt;
            var textureOnly = KeyboardState.Shift;

            var clone = (Face) face.Clone();

            // apply texture
            clone.Texture.Name = activeTexture;

            if (sampleFace != null)
            {
                if (alignTexture)
                {
                    clone.Texture.AlignWithTexture(clone.Plane, sampleFace.Plane, sampleFace.Texture);
                }
                else if (!textureOnly)
                {
                    // apply values
                    clone.Texture.SetRotation(sampleFace.Texture.Rotation);
                    clone.Texture.XScale = sampleFace.Texture.XScale;
                    clone.Texture.XShift = sampleFace.Texture.XShift;
                    clone.Texture.YScale = sampleFace.Texture.YScale;
                    clone.Texture.YShift = sampleFace.Texture.YShift;
                }
            }

            var edit = new Transaction(
                new RemoveMapObjectData(parent.ID, face),
                new AddMapObjectData(parent.ID, clone)
            );

            MapDocumentOperation.Perform(Document, edit);
        }

        private async Task AlignTextureToView()
        {

        }

        public override void Render(BufferBuilder builder)
        {
            base.Render(builder);

            var sel = GetSelection();
            if (sel.IsEmpty) return;

            var verts = new List<VertexStandard>();
            var indices = new List<int>();
            var groups = new List<BufferGroup>();

            var hideFaceMask = ShouldHideFaceMask;

            // Add selection highlights
            if (!hideFaceMask)
            {
                var selectionColour = Color.FromArgb(32, Color.Red).ToVector4();
                foreach (var face in sel)
                {
                    var indOffs = indices.Count;
                    var offs = verts.Count;

                    verts.AddRange(face.Vertices.Select(x => new VertexStandard {Position = x, Colour = selectionColour, Tint = Vector4.One}));
                    for (var i = 2; i < face.Vertices.Count; i++)
                    {
                        indices.Add(offs);
                        indices.Add(offs + i - 1);
                        indices.Add(offs + i);
                    }

                    groups.Add(new BufferGroup(PipelineType.FlatColourGeneric, CameraType.Perspective, true, face.Origin, (uint) indOffs, (uint) (indices.Count - indOffs)));
                }

                builder.Append(verts, indices.Select(x => (uint) x), groups);
            }

            // Add wireframes - selection outlines and texture axes
            var lineColour = Color.Yellow.ToVector4();
            var uAxisColour = Color.Yellow.ToVector4();
            var vAxisColour = Color.Lime.ToVector4();
            var wfIndOffs = indices.Count;
            foreach (var face in sel)
            {
                var offs = verts.Count;

                // outlines
                verts.AddRange(face.Vertices.Select(x => new VertexStandard { Position = x, Colour = lineColour, Tint = Vector4.One }));
                for (var i = 0; i < face.Vertices.Count; i++)
                {
                    indices.Add(offs + i);
                    indices.Add(offs + (i + 1) % face.Vertices.Count);
                }

                // texture axes
                var lineStart = (face.Vertices.Aggregate(Vector3.Zero, (a, b) => a + b) / face.Vertices.Count) + face.Plane.Normal * 0.5f;
                var uEnd = lineStart + face.Texture.UAxis * 20;
                var vEnd = lineStart + face.Texture.VAxis * 20;

                offs = verts.Count;
                verts.Add(new VertexStandard { Position = lineStart, Colour = uAxisColour, Tint = Vector4.One });
                verts.Add(new VertexStandard { Position = uEnd, Colour = uAxisColour, Tint = Vector4.One });
                verts.Add(new VertexStandard { Position = lineStart, Colour = vAxisColour, Tint = Vector4.One });
                verts.Add(new VertexStandard { Position = vEnd, Colour = vAxisColour, Tint = Vector4.One });
                indices.Add(offs + 0);
                indices.Add(offs + 1);
                indices.Add(offs + 2);
                indices.Add(offs + 3);
            }

            groups.Add(new BufferGroup(PipelineType.WireframeGeneric, CameraType.Perspective, false, Vector3.Zero, (uint)wfIndOffs, (uint)(indices.Count - wfIndOffs)));
            builder.Append(verts, indices.Select(x => (uint)x), groups);
        }
    }
}

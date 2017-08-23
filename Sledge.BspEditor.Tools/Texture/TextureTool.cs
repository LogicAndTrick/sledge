using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Modification.Operations;
using Sledge.BspEditor.Modification.Operations.Data;
using Sledge.BspEditor.Modification.Operations.Selection;
using Sledge.BspEditor.Primitives;
using Sledge.BspEditor.Primitives.MapData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.BspEditor.Rendering;
using Sledge.BspEditor.Rendering.Viewport;
using Sledge.BspEditor.Tools.Properties;
using Sledge.Common.Shell.Components;
using Sledge.DataStructures.Geometric;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Materials;
using Sledge.Rendering.Scenes;
using Sledge.Rendering.Scenes.Renderables;
using Face = Sledge.BspEditor.Primitives.MapObjectData.Face;
using KeyboardState = Sledge.Shell.Input.KeyboardState;
using Line = Sledge.Rendering.Scenes.Renderables.Line;

namespace Sledge.BspEditor.Tools.Texture
{
    [Export(typeof(ITool))]
    [Export]
    [OrderHint("J")]
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

        private ITextured _sampled;

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
            _sampled = tex;
            var at = new ActiveTexture {Name = _sampled?.Texture.Name};
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

        public override void DocumentChanged()
        {
            SetFaceSelectionFromObjectSelection();
            base.DocumentChanged();
        }

        protected override IEnumerable<Subscription> Subscribe()
        {
            yield return Oy.Subscribe<Change>("MapDocument:Changed", DocumentUpdated);
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

        public override void ToolSelected()
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

            MapDocumentOperation.Bypass(Document, new Deselect(Document.Selection));
            base.ToolSelected();
        }
        
        public override void ToolDeselected()
        {
            GetSelection().Clear();
            _sampled = null;
            base.ToolDeselected();
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

            var ray = vp.CastRayFromScreen(e.X, e.Y);
            var hits = GetBoundingBoxIntersections(ray);

            var clickedFace = hits.OfType<Solid>().SelectMany(a => a.Faces.Select(f => new { Face = f, Solid = a }))
                .Select(x => new {x.Face, x.Solid, Intersection = new Polygon(x.Face.Vertices).GetIntersectionPoint(ray) })
                .Where(x => x.Intersection != null)
                .OrderBy(x => (x.Intersection - ray.Start).VectorMagnitude())
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

            var activeTexture = Document.Map.Data.GetOne<ActiveTexture>()?.Name ?? _sampled?.Texture.Name ?? "";
            if (String.IsNullOrWhiteSpace(activeTexture)) return;

            var alignTexture = KeyboardState.Alt;
            var textureOnly = KeyboardState.Shift;

            var clone = (Face) face.Clone();

            // apply texture
            clone.Texture.Name = activeTexture;

            if (_sampled != null)
            {
                if (alignTexture)
                {
                    AlignTextureToSample(_sampled, clone);
                }
                else if (!textureOnly)
                {
                    // apply values
                    clone.Texture.SetRotation(_sampled.Texture.Rotation);
                    clone.Texture.XScale = _sampled.Texture.XScale;
                    clone.Texture.XShift = _sampled.Texture.XShift;
                    clone.Texture.YScale = _sampled.Texture.YScale;
                    clone.Texture.YShift = _sampled.Texture.YShift;
                }
            }

            var sel = GetSelection();

            var edit = new Transaction(
                new RemoveMapObjectData(parent.ID, face),
                new AddMapObjectData(parent.ID, clone)
            );

            MapDocumentOperation.Perform(Document, edit);
        }

        private async Task AlignTextureToView()
        {

        }

        private async Task AlignTextureToSample(ITextured sample, ITextured target)
        {
            // todo: align texture
        }

        protected override IEnumerable<SceneObject> GetSceneObjects()
        {
            var list = base.GetSceneObjects().ToList();

            var hideFaceMask = ShouldHideFaceMask;

            foreach (var face in GetSelection())
            {
                // face masks

                if (!hideFaceMask)
                {
                    list.Add(new Sledge.Rendering.Scenes.Renderables.Face(
                        Material.Flat(Color.FromArgb(160, Color.Red)),
                        face.GetTextureCoordinates(64, 64)
                            .Select(x => new Vertex(x.Item1.ToVector3(), (float) x.Item2, (float) x.Item3)).ToList()
                    )
                    {
                        CameraFlags = CameraFlags.Perspective,
                        ForcedRenderFlags = face == _sampled ? RenderFlags.Wireframe : RenderFlags.None,
                        AccentColor = Color.Yellow
                    });
                }

                // texture axes

                var lineStart = (face.Vertices.Aggregate(Coordinate.Zero, (a,b) => a + b) / face.Vertices.Count) + face.Plane.Normal * 0.5m;
                var uEnd = lineStart + face.Texture.UAxis * 20;
                var vEnd = lineStart + face.Texture.VAxis * 20;

                // If we don't want the axis markers to be depth tested, we can use an element instead:
                //list.Add(new LineElement(PositionType.World, Color.Yellow, new List<Position> { new Position(lineStart.ToVector3()), new Position(uEnd.ToVector3()) }));
                //list.Add(new LineElement(PositionType.World, Color.FromArgb(0, 255, 0), new List<Position> { new Position(lineStart.ToVector3()), new Position(vEnd.ToVector3()) }));

                list.Add(new Line(Color.Yellow, lineStart.ToVector3(), uEnd.ToVector3()) { CameraFlags = CameraFlags.Perspective });
                list.Add(new Line(Color.FromArgb(0, 255, 0), lineStart.ToVector3(), vEnd.ToVector3()) { CameraFlags = CameraFlags.Perspective });
            }

            return list;
        }
    }
}

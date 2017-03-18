using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using OpenTK;
using Sledge.Common;
using Sledge.Common.Mediator;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Actions;
using Sledge.Editor.Actions.MapObjects.Operations;
using Sledge.Editor.Actions.MapObjects.Selection;
using Sledge.Editor.Documents;
using Sledge.Editor.Extensions;
using Sledge.Editor.History;
using Sledge.Editor.Properties;
using Sledge.Editor.Rendering;
using Sledge.Editor.UI;
using Sledge.Providers.Texture;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Materials;
using Sledge.Rendering.Scenes;
using Sledge.Settings;
using Face = Sledge.DataStructures.MapObjects.Face;
using Line = Sledge.Rendering.Scenes.Renderables.Line;

namespace Sledge.Editor.Tools.TextureTool
{
    public class TextureTool : BaseTool
    {

        #region Enums

        public enum SelectBehaviour
        {
            LiftSelect,
            Lift,
            Select,
            Apply,
            ApplyWithValues,
            AlignToView
        }

        public enum JustifyMode
        {
            Fit,
            Left,
            Right,
            Center,
            Top,
            Bottom
        }

        public enum AlignMode
        {
            Face,
            World
        }

        #endregion

        private readonly TextureApplicationForm _form;
        private readonly TextureToolSidebarPanel _sidebarPanel;

        public TextureTool()
        {
            Usage = ToolUsage.View3D;
            _form = new TextureApplicationForm();
            _form.PropertyChanged += TexturePropertyChanged;
            _form.TextureAlign += TextureAligned;
            _form.TextureApply += TextureApplied;
            _form.TextureJustify += TextureJustified;
            _form.HideMaskToggled += HideMaskToggled;
            _form.TextureChanged += TextureChanged;

            _sidebarPanel = new TextureToolSidebarPanel();
            _sidebarPanel.TileFit += TileFit;
            _sidebarPanel.RandomiseXShiftValues += RandomiseXShiftValues;
            _sidebarPanel.RandomiseYShiftValues += RandomiseYShiftValues;
        }

        public override void DocumentChanged()
        {
            _form.Document = Document;
        }

        private void HideMaskToggled(object sender, bool hide)
        {
            Document.Map.HideFaceMask = !hide;
            Mediator.Publish(HotkeysMediator.ToggleHideFaceMask);
        }

        private void RandomiseXShiftValues(object sender, int min, int max)
        {
            if (Document.Selection.IsEmpty()) return;

            var rand = new Random();
            Action<Document, Face> action = (d, f) =>
            {
                f.Texture.XShift = rand.Next(min, max + 1); // Upper bound is exclusive
            };
            Document.PerformAction("Randomise X shift values", new EditFace(Document.Selection.GetSelectedFaces(), action, false));
        }

        private void RandomiseYShiftValues(object sender, int min, int max)
        {
            if (Document.Selection.IsEmpty()) return;

            var rand = new Random();
            Action<Document, Face> action = (d, f) =>
            {
                f.Texture.YShift = rand.Next(min, max + 1); // Upper bound is exclusive
            };
            Document.PerformAction("Randomise Y shift values", new EditFace(Document.Selection.GetSelectedFaces(), action, false));
        }

        private void TextureJustified(object sender, JustifyMode justifymode, bool treatasone)
        {
            TextureJustified(justifymode, treatasone, 1, 1);
        }

        private void TileFit(object sender, int tileX, int tileY)
        {
            TextureJustified(JustifyMode.Fit, _form.ShouldTreatAsOne(), tileX, tileY);
        }

        private void TextureJustified(JustifyMode justifymode, bool treatasone, int tileX, int tileY)
        {
            if (Document.Selection.IsEmpty()) return;
            var boxAlignMode = (justifymode == JustifyMode.Fit)
                                   ? Face.BoxAlignMode.Center // Don't care about the align mode when centering
                                   : (Face.BoxAlignMode) Enum.Parse(typeof (Face.BoxAlignMode), justifymode.ToString());
            Cloud cloud = null;
            Action<Document, Face> action;
            if (treatasone) 
            {
                // If we treat as one, it means we want to align to one great big cloud
                cloud = new Cloud(Document.Selection.GetSelectedFaces().SelectMany(x => x.Vertices).Select(x => x.Location));
            }

            if (justifymode == JustifyMode.Fit)
            {
                action = (d, x) =>
                {
                    var tex = d.GetTextureSize(x.Texture.Name);
                    if (!tex.IsEmpty)
                    {
                        x.FitTextureToPointCloud(tex.Width, tex.Height, cloud ?? new Cloud(x.Vertices.Select(y => y.Location)), tileX, tileY);
                    }
                };
            }
            else
            {
                action = (d, x) =>
                {
                    var tex = d.GetTextureSize(x.Texture.Name);
                    if (!tex.IsEmpty)
                    {
                        x.AlignTextureWithPointCloud(tex.Width, tex.Height, cloud ?? new Cloud(x.Vertices.Select(y => y.Location)), boxAlignMode);
                    }
                };
            }

            Document.PerformAction("Align texture", new EditFace(Document.Selection.GetSelectedFaces(), action, false));
        }

        private void TextureApplied(object sender, string texture)
        {
            Action<Document, Face> action = (document, face) =>
            {
                face.Texture.Name = texture;
            };
            // When the texture changes, the entire list needs to be regenerated, can't do a partial update.
            Document.PerformAction("Apply texture", new EditFace(Document.Selection.GetSelectedFaces(), action, true));

            Mediator.Publish(EditorMediator.TextureSelected, texture);
        }

        private void TextureAligned(object sender, AlignMode align)
        {
            Action<Document, Face> action = (document, face) =>
            {
                if (align == AlignMode.Face) face.AlignTextureToFace();
                else if (align == AlignMode.World) face.AlignTextureToWorld();
            };

            Document.PerformAction("Align texture", new EditFace(Document.Selection.GetSelectedFaces(), action, false));
        }

        private void TexturePropertyChanged(object sender, TextureApplicationForm.CurrentTextureProperties properties)
        {
            if (Document.Selection.IsEmpty()) return;

            Action<Document, Face> action = (document, face) =>
            {
                if (!properties.DifferentXScaleValues) face.Texture.XScale = properties.XScale;
                if (!properties.DifferentYScaleValues) face.Texture.YScale = properties.YScale;
                if (!properties.DifferentXShiftValues) face.Texture.XShift = properties.XShift;
                if (!properties.DifferentYShiftValues) face.Texture.YShift = properties.YShift;
                if (!properties.DifferentRotationValues) face.SetTextureRotation(properties.Rotation);
            };

            Document.PerformAction("Modify texture properties", new EditFace(Document.Selection.GetSelectedFaces(), action, false));
        }

        private void TextureChanged(object sender, string texture)
        {
            Mediator.Publish(EditorMediator.TextureSelected, texture);
        }

        public override Image GetIcon()
        {
            return Resources.Tool_Texture;
        }

        public override string GetName()
        {
            return "Texture Application Tool";
        }

        public override HotkeyTool? GetHotkeyToolType()
        {
            return HotkeyTool.Texture;
        }

        public override string GetContextualHelp()
        {
            return "*Click* a face to select it\n" +
                   "*Ctrl+Click* to select multiple\n" +
                   "*Shift+Click* to select all faces of a solid";
        }

        public override IEnumerable<KeyValuePair<string, Control>> GetSidebarControls()
        {
            yield return new KeyValuePair<string, Control>("Texture Power Tools", _sidebarPanel);
        }

        public override void ToolSelected(bool preventHistory)
        {
            _form.Show(Editor.Instance);
            Editor.Instance.Focus();

            if (!preventHistory)
            {
                Document.History.AddHistoryItem(new HistoryAction("Switch selection mode", new ChangeToFaceSelectionMode(GetType(), Document.Selection.GetSelectedObjects())));
                var currentSelection = Document.Selection.GetSelectedObjects();
                Document.Selection.SwitchToFaceSelection();
                var newSelection = Document.Selection.GetSelectedFaces().Select(x => x.Parent);
                Document.RenderObjects(currentSelection.Union(newSelection));
            }

            _form.SelectionChanged();

            var selection = Document.Selection.GetSelectedFaces().OrderBy(x => String.IsNullOrWhiteSpace(x.Texture.Name) ? 1 : 0).FirstOrDefault();
            if (selection != null)
            {
                var itemToSelect = selection.Texture.Name;
                Mediator.Publish(EditorMediator.TextureSelected, itemToSelect);
            }
            _form.SelectTexture(Document.TextureCollection.SelectedTexture);

            Mediator.Subscribe(EditorMediator.TextureSelected, this);
            Mediator.Subscribe(EditorMediator.DocumentTreeFacesChanged, this);
            Mediator.Subscribe(EditorMediator.SelectionChanged, this);

            base.ToolSelected(preventHistory);
        }

        public override void ToolDeselected(bool preventHistory)
        {
            var selected = Document.Selection.GetSelectedFaces().ToList();

            if (!preventHistory)
            {
                Document.History.AddHistoryItem(new HistoryAction("Switch selection mode", new ChangeToObjectSelectionMode(GetType(), selected)));
                var currentSelection = Document.Selection.GetSelectedFaces().Select(x => x.Parent);
                Document.Selection.SwitchToObjectSelection();
                var newSelection = Document.Selection.GetSelectedObjects();
                Document.RenderObjects(currentSelection.Union(newSelection));
            }

            _form.Clear();
            _form.Hide();
            Mediator.UnsubscribeAll(this);
            base.ToolDeselected(preventHistory);
        }

        private void TextureSelected(string texture)
        {
            _form.SelectTexture(texture);
        }

        private void SelectionChanged()
        {
            _form.SelectionChanged();
            Invalidate();
        }

        private void DocumentTreeFacesChanged()
        {
            _form.SelectionChanged();
            Invalidate();
        }

        protected override void MouseDown(MapViewport viewport, PerspectiveCamera camera, ViewportEvent e)
        {
            var vp = viewport;
            if (vp == null || (e.Button != MouseButtons.Left && e.Button != MouseButtons.Right)) return;

            var behaviour = e.Button == MouseButtons.Left
                                ? _form.GetLeftClickBehaviour(KeyboardState.Ctrl, KeyboardState.Shift, KeyboardState.Alt)
                                : _form.GetRightClickBehaviour(KeyboardState.Ctrl, KeyboardState.Shift, KeyboardState.Alt);

            var ray = vp.CastRayFromScreen(e.X, e.Y);
            var hits = Document.Map.WorldSpawn.GetAllNodesIntersectingWith(ray).OfType<Solid>();
            var clickedFace = hits.SelectMany(f => f.Faces)
                .Select(x => new {Item = x, Intersection = x.GetIntersectionPoint(ray)})
                .Where(x => x.Intersection != null)
                .OrderBy(x => (x.Intersection - ray.Start).VectorMagnitude())
                .Select(x => x.Item)
                .FirstOrDefault();

            if (clickedFace == null) return;

            var faces = new List<Face>();
            if (KeyboardState.Shift) faces.AddRange(clickedFace.Parent.Faces);
            else faces.Add(clickedFace);

            var firstSelected = Document.Selection.GetSelectedFaces().FirstOrDefault();
            var firstClicked = faces.FirstOrDefault(face => !String.IsNullOrWhiteSpace(face.Texture.Name));

            var ac = new ActionCollection();

            var select = new ChangeFaceSelection(
                KeyboardState.Ctrl ? faces.Where(x => !x.IsSelected) : faces,
                KeyboardState.Ctrl ? faces.Where(x => x.IsSelected) : Document.Selection.GetSelectedFaces().Where(x => !faces.Contains(x)));

            Action lift = () =>
            {
                if (firstClicked == null) return;
                var itemToSelect = firstClicked.Texture.Name;
                Mediator.Publish(EditorMediator.TextureSelected, itemToSelect);
            };

            switch (behaviour)
            {
                case SelectBehaviour.Select:
                    ac.Add(select);
                    break;
                case SelectBehaviour.LiftSelect:
                    lift();
                    ac.Add(select);
                    break;
                case SelectBehaviour.Lift:
                    lift();
                    break;
                case SelectBehaviour.Apply:
                case SelectBehaviour.ApplyWithValues:
                    var item = _form.GetFirstSelectedTexture();
                    if (item != null)
                    {
                        ac.Add(new EditFace(faces, (document, face) =>
                                                        {
                                                            face.Texture.Name = item;
                                                            if (behaviour == SelectBehaviour.ApplyWithValues && firstSelected != null)
                                                            {
                                                                // Calculates the texture coordinates
                                                                face.AlignTextureWithFace(firstSelected);
                                                            }
                                                            else if (behaviour == SelectBehaviour.ApplyWithValues)
                                                            {
                                                                face.Texture.XScale = _form.CurrentProperties.XScale;
                                                                face.Texture.YScale = _form.CurrentProperties.YScale;
                                                                face.Texture.XShift = _form.CurrentProperties.XShift;
                                                                face.Texture.YShift = _form.CurrentProperties.YShift;
                                                                face.SetTextureRotation(_form.CurrentProperties.Rotation);
                                                            }
                                                        }, true));
                    }
                    break;
                case SelectBehaviour.AlignToView:
                    var right = camera.GetRight();
                    var up = camera.GetUp();
                    var loc = camera.EyeLocation;
                    var point = new Coordinate((decimal)loc.X, (decimal)loc.Y, (decimal)loc.Z);
                    var uaxis = new Coordinate((decimal) right.X, (decimal) right.Y, (decimal) right.Z);
                    var vaxis = new Coordinate((decimal) up.X, (decimal) up.Y, (decimal) up.Z);
                    ac.Add(new EditFace(faces, (document, face) =>
                                                    {
                                                        face.Texture.XScale = 1;
                                                        face.Texture.YScale = 1;
                                                        face.Texture.UAxis = uaxis;
                                                        face.Texture.VAxis = vaxis;
                                                        face.Texture.XShift = face.Texture.UAxis.Dot(point);
                                                        face.Texture.YShift = face.Texture.VAxis.Dot(point);
                                                        face.Texture.Rotation = 0;
                                                    }, false));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            if (!ac.IsEmpty())
            {
                Document.PerformAction("Texture selection", ac);
            }
        }

        protected override IEnumerable<SceneObject> GetSceneObjects()
        {
            var list = base.GetSceneObjects().ToList();
            
            foreach (var face in Document.Selection.GetSelectedFaces())
            {
                var lineStart = face.BoundingBox.Center + face.Plane.Normal * 0.5m;
                var uEnd = lineStart + face.Texture.UAxis * 20;
                var vEnd = lineStart + face.Texture.VAxis * 20;

                // If we don't want the axis markers to be depth tested, we can use an element instead:
                //list.Add(new LineElement(PositionType.World, Color.Yellow, new List<Position> { new Position(lineStart.ToVector3()), new Position(uEnd.ToVector3()) }));
                //list.Add(new LineElement(PositionType.World, Color.FromArgb(0, 255, 0), new List<Position> { new Position(lineStart.ToVector3()), new Position(vEnd.ToVector3()) }));

                list.Add(new Line(Color.Yellow, lineStart.ToVector3(), uEnd.ToVector3()));
                list.Add(new Line(Color.FromArgb(0, 255, 0), lineStart.ToVector3(), vEnd.ToVector3()));
            }

            return list;
        }

        public override HotkeyInterceptResult InterceptHotkey(HotkeysMediator hotkeyMessage, object parameters)
        {
            switch (hotkeyMessage)
            {
                case HotkeysMediator.OperationsCopy:
                case HotkeysMediator.OperationsCut:
                case HotkeysMediator.OperationsPaste:
                case HotkeysMediator.OperationsPasteSpecial:
                case HotkeysMediator.OperationsDelete:
                    return HotkeyInterceptResult.Abort;
            }
            return HotkeyInterceptResult.Continue;
        }
    }
}

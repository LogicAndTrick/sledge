using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using Sledge.Common;
using Sledge.Common.Mediator;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;
using Sledge.EditorNew.Actions;
using Sledge.EditorNew.Actions.MapObjects.Operations;
using Sledge.EditorNew.Actions.MapObjects.Selection;
using Sledge.EditorNew.Documents;
using Sledge.EditorNew.History;
using Sledge.EditorNew.Properties;
using Sledge.EditorNew.UI;
using Sledge.EditorNew.UI.Viewports;
using Sledge.Graphics.Helpers;
using Sledge.Gui.Containers;
using Sledge.Providers.Texture;
using Sledge.Settings;

namespace Sledge.EditorNew.Tools.TextureTool
{
    public class TextureToolSidebarPanel : VerticalBox
    {
        public delegate void RandomiseXShiftValuesEventHandler(object sender, int min, int max);
        public delegate void RandomiseYShiftValuesEventHandler(object sender, int min, int max);
        public delegate void TileFitEventHandler(object sender, int tileX, int tileY);

        public event RandomiseXShiftValuesEventHandler RandomiseXShiftValues;
        public event RandomiseYShiftValuesEventHandler RandomiseYShiftValues;
        public event TileFitEventHandler TileFit;

        protected virtual void OnRandomiseXShiftValues(int min, int max)
        {
            if (RandomiseXShiftValues != null)
            {
                RandomiseXShiftValues(this, min, max);
            }
        }

        protected virtual void OnRandomiseYShiftValues(int min, int max)
        {
            if (RandomiseYShiftValues != null)
            {
                RandomiseYShiftValues(this, min, max);
            }
        }

        protected virtual void OnTileFit(int tileX, int tileY)
        {
            if (TileFit != null)
            {
                TileFit(this, tileX, tileY);
            }
        }
    }

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
                f.CalculateTextureCoordinates(true);
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
                f.CalculateTextureCoordinates(true);
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
                action = (d, x) => x.FitTextureToPointCloud(cloud ?? new Cloud(x.Vertices.Select(y => y.Location)), tileX, tileY);
            }
            else
            {
                action = (d, x) => x.AlignTextureWithPointCloud(cloud ?? new Cloud(x.Vertices.Select(y => y.Location)), boxAlignMode);
            }

            Document.PerformAction("Align texture", new EditFace(Document.Selection.GetSelectedFaces(), action, false));
        }

        private void TextureApplied(object sender, TextureItem texture)
        {
            var ti = texture.GetTexture();
            Action<Document, Face> action = (document, face) =>
                                      {
                                          face.Texture.Name = texture.Name;
                                          face.Texture.Texture = ti;
                                          face.CalculateTextureCoordinates(false);
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
                face.CalculateTextureCoordinates(false);
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
                face.CalculateTextureCoordinates(false);
            };

            Document.PerformAction("Modify texture properties", new EditFace(Document.Selection.GetSelectedFaces(), action, false));
        }

        private void TextureChanged(object sender, TextureItem texture)
        {
            Mediator.Publish(EditorMediator.TextureSelected, texture);
        }

        public override IEnumerable<string> GetContexts()
        {
            yield return "Texture Tool";
        }

        public override Image GetIcon()
        {
            return Resources.Tool_Texture;
        }

        public override string GetName()
        {
            return "TextureTool";
        }

        public override HotkeyTool? GetHotkeyToolType()
        {
            return HotkeyTool.Texture;
        }

        public override IEnumerable<ToolSidebarControl> GetSidebarControls()
        {
            yield return new ToolSidebarControl { Control = _sidebarPanel, TextKey = "Tools/TextureTool/PowerTools"};
        }

        public override void ToolSelected(bool preventHistory)
        {
            _form.Open();
            // todo _form.Show(Editor.Instance);
            // Editor.Instance.Focus();

            if (!preventHistory)
            {
                Document.History.AddHistoryItem(new HistoryAction("Switch selection mode", new ChangeToFaceSelectionMode(GetType(), Document.Selection.GetSelectedObjects())));
                var currentSelection = Document.Selection.GetSelectedObjects();
                Document.Selection.SwitchToFaceSelection();
                var newSelection = Document.Selection.GetSelectedFaces().Select(x => x.Parent);
                Document.RenderSelection(currentSelection.Union(newSelection));
            }

            _form.SelectionChanged();

            var selection = Document.Selection.GetSelectedFaces().OrderBy(x => x.Texture.Texture == null ? 1 : 0).FirstOrDefault();
            if (selection != null)
            {
                var itemToSelect = Document.TextureCollection.GetItem(selection.Texture.Name)
                                   ?? new TextureItem(null, selection.Texture.Name, TextureFlags.Missing, 64, 64);
                Mediator.Publish(EditorMediator.TextureSelected, itemToSelect);
            }
            _form.SelectTexture(Document.TextureCollection.SelectedTexture);

            Mediator.Subscribe(EditorMediator.TextureSelected, this);
            Mediator.Subscribe(EditorMediator.DocumentTreeFacesChanged, this);
            Mediator.Subscribe(EditorMediator.SelectionChanged, this);
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
                Document.RenderSelection(currentSelection.Union(newSelection));
            }

            _form.Clear();
            _form.Close();
            Mediator.UnsubscribeAll(this);
        }

        private void TextureSelected(TextureItem texture)
        {
            _form.SelectTexture(texture);
        }

        private void SelectionChanged()
        {
            _form.SelectionChanged();
        }

        private void DocumentTreeFacesChanged()
        {
            _form.SelectionChanged();
        }

        protected override void MouseDown(IViewport3D viewport, ViewportEvent e)
        {
            if (e.Button != MouseButton.Left && e.Button != MouseButton.Right) return;
            var behaviour = e.Button == MouseButton.Left
                                ? _form.GetLeftClickBehaviour(Input.Ctrl, Input.Shift, Input.Alt)
                                : _form.GetRightClickBehaviour(Input.Ctrl, Input.Shift, Input.Alt);

            var ray = viewport.CastRayFromScreen(e.X, e.Y);
            var hits = Document.Map.WorldSpawn.GetAllNodesIntersectingWith(ray).OfType<Solid>();
            var clickedFace = hits.SelectMany(f => f.Faces)
                .Select(x => new {Item = x, Intersection = x.GetIntersectionPoint(ray)})
                .Where(x => x.Intersection != null)
                .OrderBy(x => (x.Intersection - ray.Start).VectorMagnitude())
                .Select(x => x.Item)
                .FirstOrDefault();

            if (clickedFace == null) return;

            var faces = new List<Face>();
            if (Input.Shift) faces.AddRange(clickedFace.Parent.Faces);
            else faces.Add(clickedFace);

            var firstSelected = Document.Selection.GetSelectedFaces().FirstOrDefault();
            var firstClicked = faces.FirstOrDefault(face => !String.IsNullOrWhiteSpace(face.Texture.Name));

            var ac = new ActionCollection();

            var select = new ChangeFaceSelection(
                Input.Ctrl ? faces.Where(x => !x.IsSelected) : faces,
                Input.Ctrl ? faces.Where(x => x.IsSelected) : Document.Selection.GetSelectedFaces().Where(x => !faces.Contains(x)));

            Action lift = () =>
            {
                if (firstClicked == null) return;
                var itemToSelect = Document.TextureCollection.GetItem(firstClicked.Texture.Name)
                                   ?? new TextureItem(null, firstClicked.Texture.Name, TextureFlags.Missing, 64, 64);
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
                        var texture = item.GetTexture();
                        ac.Add(new EditFace(faces, (document, face) =>
                                                        {
                                                            face.Texture.Name = item.Name;
                                                            face.Texture.Texture = texture;
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
                                                            else
                                                            {
                                                                face.CalculateTextureCoordinates(true);
                                                            }
                                                        }, true));
                    }
                    break;
                case SelectBehaviour.AlignToView:
                    var right = viewport.Camera.GetRight();
                    var up = viewport.Camera.GetUp();
                    var loc = viewport.Camera.Location;
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
                                                        face.CalculateTextureCoordinates(true);
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

        protected override void KeyDown(IViewport3D viewport, ViewportEvent e)
        {
            //throw new NotImplementedException();
        }

        protected override void Render(IViewport3D viewport)
        {
            if (Document.Map.HideFaceMask) return;

            TextureHelper.Unbind();
            GL.Begin(PrimitiveType.Lines);
            foreach (var face in Document.Selection.GetSelectedFaces())
            {
                var lineStart = face.BoundingBox.Center + face.Plane.Normal * 0.5m;
                var uEnd = lineStart + face.Texture.UAxis * 20;
                var vEnd = lineStart + face.Texture.VAxis * 20;

                GL.Color3(Color.Yellow);
                GL.Vertex3(lineStart.DX, lineStart.DY, lineStart.DZ);
                GL.Vertex3(uEnd.DX, uEnd.DY, uEnd.DZ);

                GL.Color3(Color.FromArgb(0, 255, 0));
                GL.Vertex3(lineStart.DX, lineStart.DY, lineStart.DZ);
                GL.Vertex3(vEnd.DX, vEnd.DY, vEnd.DZ);
            }
            GL.End();
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

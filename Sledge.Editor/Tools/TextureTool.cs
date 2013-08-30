using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Sledge.Common.Mediator;
using Sledge.Editor.Actions;
using Sledge.Editor.Actions.MapObjects.Operations;
using Sledge.Editor.Actions.MapObjects.Selection;
using Sledge.Editor.Documents;
using Sledge.Providers.Texture;
using Sledge.Settings;
using Sledge.UI;
using Sledge.Editor.Properties;
using Sledge.Graphics.Helpers;
using OpenTK.Graphics.OpenGL;
using Sledge.DataStructures.MapObjects;
using Sledge.DataStructures.Geometric;

namespace Sledge.Editor.Tools
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

        public TextureTool()
        {
            Usage = ToolUsage.View3D;
            _form = new TextureApplicationForm();
            _form.PropertyChanged += TexturePropertyChanged;
            _form.TextureAlign += TextureAligned;
            _form.TextureApply += TextureApplied;
            _form.TextureJustify += TextureJustified;
            _form.HideMaskToggled += HideMaskToggled;
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

        private void TextureJustified(object sender, JustifyMode justifymode, bool treatasone)
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
                action = (d, x) => x.FitTextureToPointCloud(cloud ?? new Cloud(x.Vertices.Select(y => y.Location)));
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
                                          face.CalculateTextureCoordinates();
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
                face.CalculateTextureCoordinates();
            };

            Document.PerformAction("Align texture", new EditFace(Document.Selection.GetSelectedFaces(), action, false));
        }

        private void TexturePropertyChanged(object sender, decimal scalex, decimal scaley, int shiftx, int shifty, decimal rotation, int lightmapscale)
        {
            Action<Document, Face> action = (document, face) =>
            {
                face.Texture.XScale = scalex;
                face.Texture.YScale = scaley;
                face.Texture.XShift = shiftx;
                face.Texture.YShift = shifty;
                face.SetTextureRotation(rotation); // This will recalculate the texture coordinates as well
            };

            Document.PerformAction("Modify texture properties", new EditFace(Document.Selection.GetSelectedFaces(), action, false));
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

        public override void ToolSelected()
        {
            _form.Show(Editor.Instance);
            Editor.Instance.Focus();
            Document.Selection.SwitchToFaceSelection();
            Document.UpdateDisplayLists(Document.Selection.GetSelectedFaces());
            _form.SelectionChanged();
            _form.SelectTexture(Editor.Instance.GetSelectedTexture());
            Mediator.Subscribe(EditorMediator.TextureSelected, this);
            Mediator.Subscribe(EditorMediator.DocumentTreeFacesChanged, this);
        }

        public override void ToolDeselected()
        {
            var selected = Document.Selection.GetSelectedFaces().ToList();
            Document.Selection.SwitchToObjectSelection();
            _form.Clear();
            _form.Hide();
            Document.UpdateDisplayLists(selected);
            Mediator.UnsubscribeAll(this);
        }

        private void TextureSelected(TextureItem texture)
        {
            _form.SelectTexture(texture);
        }

        private void DocumentTreeFacesChanged()
        {
            _form.SelectionChanged();
        }

        public override void MouseDown(ViewportBase viewport, ViewportEvent e)
        {
            var vp = viewport as Viewport3D;
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
            var firstClicked = faces.FirstOrDefault(face => face.Texture.Texture != null);

            var ac = new ActionCollection();

            var select = new ChangeFaceSelection(
                KeyboardState.Ctrl ? faces.Where(x => !x.IsSelected) : faces,
                KeyboardState.Ctrl ? faces.Where(x => x.IsSelected) : Document.Selection.GetSelectedFaces().Where(x => !faces.Contains(x)));

            Action lift = () =>
                              {
                                  var itemToSelect = firstClicked != null
                                                         ? Document.TextureCollection.GetItem(firstClicked.Texture.Name)
                                                         : null;
                                  if (itemToSelect != null)
                                  {
                                      Mediator.Publish(EditorMediator.TextureSelected, itemToSelect);
                                  }
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
                                                            if (behaviour == SelectBehaviour.ApplyWithValues)
                                                            {
                                                                // Calculates the texture coordinates
                                                                face.AlignTextureWithFace(firstSelected);
                                                            }
                                                            else
                                                            {
                                                                face.CalculateTextureCoordinates();
                                                            }
                                                        }, true));
                    }
                    break;
                case SelectBehaviour.AlignToView:
                    var right = vp.Camera.GetRight();
                    var up = vp.Camera.GetUp();
                    var loc = vp.Camera.Location;
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
                                                        face.CalculateTextureCoordinates();
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

        public override void KeyDown(ViewportBase viewport, ViewportEvent e)
        {
            //throw new NotImplementedException();
        }

        public override void Render(ViewportBase viewport)
        {
            TextureHelper.DisableTexturing();
            foreach (var face in Document.Selection.GetSelectedFaces())
            {
                var lineStart = face.BoundingBox.Center + face.Plane.Normal * 0.5m;
                var uEnd = lineStart + face.Texture.UAxis * 20;
                var vEnd = lineStart + face.Texture.VAxis * 20;

                GL.Begin(BeginMode.Lines);

                GL.Color3(Color.Yellow);
                GL.Vertex3(lineStart.DX, lineStart.DY, lineStart.DZ);
                GL.Vertex3(uEnd.DX, uEnd.DY, uEnd.DZ);

                GL.Color3(Color.FromArgb(0, 255, 0));
                GL.Vertex3(lineStart.DX, lineStart.DY, lineStart.DZ);
                GL.Vertex3(vEnd.DX, vEnd.DY, vEnd.DZ);

                GL.End();
            }
            TextureHelper.EnableTexturing();
        }

        public override HotkeyInterceptResult InterceptHotkey(HotkeysMediator hotkeyMessage)
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

        public override void MouseEnter(ViewportBase viewport, ViewportEvent e)
        {
            //
        }

        public override void MouseLeave(ViewportBase viewport, ViewportEvent e)
        {
            //
        }

        public override void MouseUp(ViewportBase viewport, ViewportEvent e)
        {
            //
        }

        public override void MouseWheel(ViewportBase viewport, ViewportEvent e)
        {
            //
        }

        public override void MouseMove(ViewportBase viewport, ViewportEvent e)
        {
            //
        }

        public override void KeyPress(ViewportBase viewport, ViewportEvent e)
        {
            //
        }

        public override void KeyUp(ViewportBase viewport, ViewportEvent e)
        {
            //
        }

        public override void UpdateFrame(ViewportBase viewport)
        {
            //
        }
    }
}

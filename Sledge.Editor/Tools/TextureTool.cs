using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Sledge.Providers.Texture;
using Sledge.UI;
using Sledge.Editor.Properties;
using Sledge.Editor.Editing;
using Sledge.Graphics.Helpers;
using OpenTK.Graphics.OpenGL;
using Sledge.DataStructures.MapObjects;
using Sledge.Extensions;
using Sledge.DataStructures.Geometric;

namespace Sledge.Editor.Tools
{
    public class TextureTool : Base3DTool
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
            _form = new TextureApplicationForm();
            _form.PropertyChanged += TexturePropertyChanged;
            _form.TextureAlign += TextureAligned;
            _form.TextureApply += TextureApplied;
            _form.TextureJustify += TextureJustified;
            _form.HideMaskToggled += HideMaskToggled;
        }

        private void HideMaskToggled(object sender, bool hide)
        {
            Document.HideFaceMask = hide;
            Document.UpdateSelectLists();
        }

        private void TextureJustified(object sender, JustifyMode justifymode, bool treatasone)
        {
            if (Selection.IsEmpty()) return;
            var boxAlignMode = (justifymode == JustifyMode.Fit)
                                   ? Face.BoxAlignMode.Center // Don't care about the align mode when centering
                                   : (Face.BoxAlignMode) Enum.Parse(typeof (Face.BoxAlignMode), justifymode.ToString());
            Cloud cloud = null;
            if (treatasone)
            {
                cloud = new Cloud(Selection.GetSelectedFaces().SelectMany(x => x.Vertices).Select(x => x.Location));
            }
            foreach (var face in Selection.GetSelectedFaces())
            {
                if (!treatasone)
                {
                    cloud = new Cloud(face.Vertices.Select(x => x.Location));
                }
                if (justifymode == JustifyMode.Fit)
                {
                    face.FitTextureToPointCloud(cloud);
                }
                else
                {
                    face.AlignTextureWithPointCloud(cloud, boxAlignMode);
                }
            }
            Document.UpdateSelectLists();
            _form.SelectionChanged();
        }

        private void TextureApplied(object sender, TextureItem texture)
        {
            foreach (var face in Selection.GetSelectedFaces())
            {
                face.Texture.Name = texture.Name;
                face.Texture.Texture = texture.GetTexture();
                face.CalculateTextureCoordinates();
            }
            Document.UpdateSelectLists();
            _form.SelectionChanged();
            _form.SelectTexture(texture);
        }

        private void TextureAligned(object sender, AlignMode align)
        {
            foreach (var face in Selection.GetSelectedFaces())
            {
                if (align == AlignMode.Face) face.AlignTextureToFace();
                else if (align == AlignMode.World) face.AlignTextureToWorld();
                face.CalculateTextureCoordinates();
            }
            Document.UpdateSelectLists();
            _form.SelectionChanged();
        }

        private void TexturePropertyChanged(object sender, decimal scalex, decimal scaley, int shiftx, int shifty, decimal rotation, int lightmapscale)
        {
            foreach (var face in Selection.GetSelectedFaces())
            {
                face.Texture.XScale = scalex;
                face.Texture.YScale = scaley;
                face.Texture.XShift = shiftx;
                face.Texture.YShift = shifty;
                face.SetTextureRotation(rotation); // This will recalculate the texture coordinates as well
            }
            Document.UpdateSelectLists();
            _form.SelectionChanged();
        }

        public override Image GetIcon()
        {
            return Resources.Tool_Texture;
        }

        public override string GetName()
        {
            return "Texture Application Tool";
        }

        public override void ToolSelected()
        {
            _form.Show(Editor.Instance);
            Editor.Instance.Focus();
            Selection.SwitchToFaceSelection();
            Document.UpdateSelectLists();
            _form.SelectionChanged();
        }

        public override void ToolDeselected()
        {
            Selection.SwitchToObjectSelection();
            _form.Clear();
            _form.Hide();
            Document.UpdateDisplayLists();
        }

        public override void MouseDown(ViewportBase viewport, MouseEventArgs e)
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
            TextureItem itemToSelect = null;
            if ((behaviour == SelectBehaviour.Select || behaviour == SelectBehaviour.LiftSelect) && !KeyboardState.Ctrl)
            {
                Selection.Clear();
            }
            if (clickedFace != null)
            {
                var faces = new List<Face>();
                if (KeyboardState.Shift) faces.AddRange(clickedFace.Parent.Faces);
                else faces.Add(clickedFace);
                if (behaviour == SelectBehaviour.Select || behaviour == SelectBehaviour.LiftSelect)
                {
                    foreach (var face in faces)
                    {
                        if (face.IsSelected) Selection.Deselect(face);
                        else Selection.Select(face);
                    }
                }
                if (behaviour == SelectBehaviour.Lift || behaviour == SelectBehaviour.LiftSelect)
                {
                    var tex = faces.Where(face => face.Texture.Texture != null).FirstOrDefault();
                    itemToSelect = tex != null ? TexturePackage.GetItem(tex.Texture.Name) : null;
                }
                if (behaviour == SelectBehaviour.Apply || behaviour == SelectBehaviour.ApplyWithValues)
                {
                    var tex = Selection.GetSelectedFaces().FirstOrDefault();
                    var item = tex != null ? TexturePackage.GetItem(tex.Texture.Name) : null;
                    if (item != null)
                    {
                        foreach (var face in faces)
                        {
                            face.Texture.Name = item.Name;
                            face.Texture.Texture = item.GetTexture();
                            if (behaviour == SelectBehaviour.ApplyWithValues)
                            {
                                face.AlignTextureWithFace(tex); // Calculates the texture coordinates
                            }
                            else
                            {
                                face.CalculateTextureCoordinates();
                            }
                        }
                    }
                }
                if (behaviour == SelectBehaviour.AlignToView)
                {
                    // Match the texture normal to the camera normal
                    foreach (var face in faces)
                    {
                        var right = vp.Camera.GetRight();
                        var up = vp.Camera.GetUp();
                        var loc = vp.Camera.Location;
                        var point = new Coordinate((decimal) loc.X, (decimal) loc.Y, (decimal) loc.Z);
                        face.Texture.XScale = 1;
                        face.Texture.YScale = 1;
                        face.Texture.UAxis = new Coordinate((decimal)right.X, (decimal)right.Y, (decimal)right.Z);
                        face.Texture.VAxis = new Coordinate((decimal) up.X, (decimal) up.Y, (decimal) up.Z);
                        face.Texture.XShift = face.Texture.UAxis.Dot(point);
                        face.Texture.YShift = face.Texture.VAxis.Dot(point);
                        face.Texture.Rotation = 0;
                        face.MinimiseTextureShiftValues();
                        face.CalculateTextureCoordinates();
                    }
                }
            }
            Document.UpdateDisplayLists();
            _form.SelectionChanged();
            if (itemToSelect != null)
            {
                _form.SelectTexture(itemToSelect);
            }
        }

        public override void KeyDown(ViewportBase viewport, KeyEventArgs e)
        {
            //throw new NotImplementedException();
        }

        public override void Render(ViewportBase viewport)
        {
            TextureHelper.DisableTexturing();
            foreach (var face in Selection.GetSelectedFaces())
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

        public override void MouseEnter(ViewportBase viewport, EventArgs e)
        {
            //
        }

        public override void MouseLeave(ViewportBase viewport, EventArgs e)
        {
            //
        }

        public override void MouseUp(ViewportBase viewport, MouseEventArgs e)
        {
            //
        }

        public override void MouseWheel(ViewportBase viewport, MouseEventArgs e)
        {
            //
        }

        public override void MouseMove(ViewportBase viewport, MouseEventArgs e)
        {
            //
        }

        public override void KeyPress(ViewportBase viewport, KeyPressEventArgs e)
        {
            //
        }

        public override void KeyUp(ViewportBase viewport, KeyEventArgs e)
        {
            //
        }

        public override void UpdateFrame(ViewportBase viewport)
        {
            //
        }
    }
}

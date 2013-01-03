using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenTK.Graphics.OpenGL;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.History;
using Sledge.Editor.Properties;
using Sledge.Graphics.Helpers;
using Sledge.UI;

namespace Sledge.Editor.Tools
{
    public class EntityTool : BaseBothTool
    {
        public enum EntityState
        {
            None,
            Drawn,
            Moving
        }

        private Coordinate _location;
        private EntityState _state;

        public EntityTool()
        {
            _location = new Coordinate(0, 0, 0);
            _state = EntityState.None;
        }

        public override Image GetIcon()
        {
            return Resources.Tool_Entity;
        }

        public override string GetName()
        {
            return "Entity Tool";
        }

        public override void MouseEnter(ViewportBase viewport, EventArgs e)
        {
            viewport.Cursor = Cursors.Default;
        }

        public override void MouseLeave(ViewportBase viewport, EventArgs e)
        {
            viewport.Cursor = Cursors.Default;
        }

        public override void MouseDown(ViewportBase viewport, MouseEventArgs e)
        {
            if (!(viewport is Viewport2D)) return;
            _state = EntityState.Moving;
            var vp = viewport as Viewport2D;
            var loc = SnapIfNeeded(vp.ScreenToWorld(e.X, vp.Height - e.Y));
            _location = vp.GetUnusedCoordinate(_location) + vp.Expand(loc);
        }

        public override void MouseUp(ViewportBase viewport, MouseEventArgs e)
        {
            if (!(viewport is Viewport2D)) return;
            _state = EntityState.Drawn;
            var vp = viewport as Viewport2D;
            var loc = SnapIfNeeded(vp.ScreenToWorld(e.X, vp.Height - e.Y));
            _location = vp.GetUnusedCoordinate(_location) + vp.Expand(loc);
        }

        public override void MouseWheel(ViewportBase viewport, MouseEventArgs e)
        {
            // Nothing
        }

        public override void MouseMove(ViewportBase viewport, MouseEventArgs e)
        {
            if (!(viewport is Viewport2D)) return;
            if (_state != EntityState.Moving) return;
            var vp = viewport as Viewport2D;
            var loc = SnapIfNeeded(vp.ScreenToWorld(e.X, vp.Height - e.Y));
            _location = vp.GetUnusedCoordinate(_location) + vp.Expand(loc);
        }

        public override void KeyPress(ViewportBase viewport, KeyPressEventArgs e)
        {
            // Nothing
        }

        public override void KeyDown(ViewportBase viewport, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    CreateEntity();
                    _state = EntityState.None;
                    break;
                case Keys.Escape:
                    _state = EntityState.None;
                    break;
            }
        }

        private void CreateEntity()
        {
            var hc = new HistoryCreate("Create entity: ", new MapObject[0]);
            throw new NotImplementedException();
        }

        public override void KeyUp(ViewportBase viewport, KeyEventArgs e)
        {
            //
        }

        public override void UpdateFrame(ViewportBase viewport)
        {
            //
        }

        private static void Coord(Coordinate c)
        {
            GL.Vertex3(c.DX, c.DY, c.DZ);
        }
        private static void Coord(double x, double y, double z)
        {
            GL.Vertex3(x, y, z);
        }

        public override void Render(ViewportBase viewport)
        {
            if (_state == EntityState.None) return;
            TextureHelper.DisableTexturing();
            var high = Document.GameData.MapSizeHigh;
            var low = Document.GameData.MapSizeLow;
            if (viewport is Viewport3D)
            {
                var offset = new Coordinate(20, 20, 20);
                var start = _location - offset;
                var end = _location + offset;
                var box = new Box(start, end);
                GL.Begin(BeginMode.Lines);
                GL.Color3(Color.LimeGreen);
                foreach (var line in box.GetBoxLines())
                {
                    Coord(line.Start);
                    Coord(line.End);
                }
                Coord(low, _location.DY, _location.DZ);
                Coord(high, _location.DY, _location.DZ);
                Coord(_location.DX, low, _location.DZ);
                Coord(_location.DX, high, _location.DZ);
                Coord(_location.DX, _location.DY, low);
                Coord(_location.DX, _location.DY, high);
                GL.End();
            }
            else if (viewport is Viewport2D)
            {
                var vp = viewport as Viewport2D;
                var units = vp.PixelsToUnits(5);
                var offset = new Coordinate(units, units, units);
                var start = vp.Flatten(_location - offset);
                var end = vp.Flatten(_location + offset);
                GL.Begin(BeginMode.LineLoop);
                GL.Color3(Color.LimeGreen);
                Coord(start.DX, start.DY, start.DZ);
                Coord(end.DX, start.DY, start.DZ);
                Coord(end.DX, end.DY, start.DZ);
                Coord(start.DX, end.DY, start.DZ);
                GL.End();
                GL.Begin(BeginMode.Lines);
                var loc = vp.Flatten(_location);
                Coord(low, loc.DY, 0);
                Coord(high, loc.DY, 0);
                Coord(loc.DX, low, 0);
                Coord(loc.DX, high, 0);
                GL.End();
            }
            TextureHelper.EnableTexturing();
        }
    }
}

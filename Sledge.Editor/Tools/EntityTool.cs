using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using OpenTK.Graphics.OpenGL;
using Sledge.Common;
using Sledge.Common.Mediator;
using Sledge.DataStructures.GameData;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Actions;
using Sledge.Editor.Actions.MapObjects.Operations;
using Sledge.Editor.Actions.MapObjects.Selection;
using Sledge.Editor.Properties;
using Sledge.Editor.UI;
using Sledge.Editor.UI.Sidebar;
using Sledge.Settings;
using Sledge.UI;
using Select = Sledge.Settings.Select;

namespace Sledge.Editor.Tools
{
    public class EntityTool : BaseTool
    {
        private enum EntityState
        {
            None,
            Drawn,
            Moving
        }

        private Coordinate _location;
        private EntityState _state;
        private ToolStripItem[] _menu;
        private EntitySidebarPanel _sidebarPanel;

        public EntityTool()
        {
            Usage = ToolUsage.Both;
            _location = new Coordinate(0, 0, 0);
            _state = EntityState.None;
            _sidebarPanel = new EntitySidebarPanel();
        }

        public override Image GetIcon()
        {
            return Resources.Tool_Entity;
        }

        public override string GetName()
        {
            return "Entity Tool";
        }

        public override string GetContextualHelp()
        {
            return "In the 3D view, *click* a face to place an entity there.\n" +
                   "In the 2D view, *click* to place a point and press *enter* to create.\n" +
                   "*Right click* in the 2D view to quickly choose and create any entity type.";
        }

        public override IEnumerable<KeyValuePair<string, Control>> GetSidebarControls()
        {
            yield return new KeyValuePair<string, Control>(GetName(), _sidebarPanel);
        }

        public override void DocumentChanged()
        {
            System.Threading.Tasks.Task.Factory.StartNew(BuildMenu);
            _sidebarPanel.RefreshEntities(Document);
        }

        private void BuildMenu()
        {
            if (_menu != null) foreach (var item in _menu) item.Dispose();
            _menu = null;
            if (Document == null) return;

            var items = new List<ToolStripItem>();
            var classes = Document.GameData.Classes.Where(x => x.ClassType != ClassType.Base && x.ClassType != ClassType.Solid).ToList();
            var groups = classes.GroupBy(x => x.Name.Split('_')[0]);
            foreach (var g in groups)
            {
                var mi = new ToolStripMenuItem(g.Key);
                var l = g.ToList();
                if (l.Count == 1)
                {
                    var cls = l[0];
                    mi.Text = cls.Name;
                    mi.Tag = cls;
                    mi.Click += ClickMenuItem;
                }
                else
                {
                    var subs = l.Select(x =>
                    {
                        var item = new ToolStripMenuItem(x.Name) { Tag = x };
                        item.Click += ClickMenuItem;
                        return item;
                    }).OfType<ToolStripItem>().ToArray();
                    mi.DropDownItems.AddRange(subs);
                }
                items.Add(mi);
            }
            _menu = items.ToArray();
        }

        private void ClickMenuItem(object sender, EventArgs e)
        {
            CreateEntity(_location, ((ToolStripItem)sender).Tag as GameDataObject);
        }

        public override HotkeyTool? GetHotkeyToolType()
        {
            return HotkeyTool.Entity;
        }

        public override void MouseEnter(ViewportBase viewport, ViewportEvent e)
        {
            viewport.Cursor = Cursors.Default;
        }

        public override void MouseLeave(ViewportBase viewport, ViewportEvent e)
        {
            viewport.Cursor = Cursors.Default;
        }

        public override void MouseDown(ViewportBase viewport, ViewportEvent e)
        {
            if (viewport is Viewport3D)
            {
                MouseDown((Viewport3D) viewport, e);
                return;
            }
            if (e.Button != MouseButtons.Left && e.Button != MouseButtons.Right) return;

            _state = EntityState.Moving;
            var vp = (Viewport2D) viewport;
            var loc = SnapIfNeeded(vp.ScreenToWorld(e.X, vp.Height - e.Y));
            _location = vp.GetUnusedCoordinate(_location) + vp.Expand(loc);
        }

        private void MouseDown(Viewport3D vp, ViewportEvent e)
        {
            if (vp == null || e.Button != MouseButtons.Left) return;

            // Get the ray that is cast from the clicked point along the viewport frustrum
            var ray = vp.CastRayFromScreen(e.X, e.Y);

            // Grab all the elements that intersect with the ray
            var hits = Document.Map.WorldSpawn.GetAllNodesIntersectingWith(ray);

            // Sort the list of intersecting elements by distance from ray origin and grab the first hit
            var hit = hits
                .Select(x => new { Item = x, Intersection = x.GetIntersectionPoint(ray) })
                .Where(x => x.Intersection != null)
                .OrderBy(x => (x.Intersection - ray.Start).VectorMagnitude())
                .FirstOrDefault();

            if (hit == null) return; // Nothing was clicked

            CreateEntity(hit.Intersection);
        }

        public override void MouseClick(ViewportBase viewport, ViewportEvent e)
        {
            // Not used
        }

        public override void MouseDoubleClick(ViewportBase viewport, ViewportEvent e)
        {
            // Not used
        }

        public override void MouseUp(ViewportBase viewport, ViewportEvent e)
        {
            if (!(viewport is Viewport2D) || e.Button != MouseButtons.Left) return;
            _state = EntityState.Drawn;
            var vp = viewport as Viewport2D;
            var loc = SnapIfNeeded(vp.ScreenToWorld(e.X, vp.Height - e.Y));
            _location = vp.GetUnusedCoordinate(_location) + vp.Expand(loc);
        }

        public override void MouseWheel(ViewportBase viewport, ViewportEvent e)
        {
            // Nothing
        }

        public override void MouseMove(ViewportBase viewport, ViewportEvent e)
        {
            if (!(viewport is Viewport2D) || !Control.MouseButtons.HasFlag(MouseButtons.Left)) return;
            if (_state != EntityState.Moving) return;
            var vp = viewport as Viewport2D;
            var loc = SnapIfNeeded(vp.ScreenToWorld(e.X, vp.Height - e.Y));
            _location = vp.GetUnusedCoordinate(_location) + vp.Expand(loc);
        }

        public override void KeyPress(ViewportBase viewport, ViewportEvent e)
        {
            // Nothing
        }

        public override void KeyDown(ViewportBase viewport, ViewportEvent e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    CreateEntity(_location);
                    _state = EntityState.None;
                    break;
                case Keys.Escape:
                    _state = EntityState.None;
                    break;
            }
        }

        private void CreateEntity(Coordinate origin, GameDataObject gd = null)
        {
            if (gd == null) gd = _sidebarPanel.GetSelectedEntity();
            if (gd == null) return;

            var col = gd.Behaviours.Where(x => x.Name == "color").ToArray();
            var colour = col.Any() ? col[0].GetColour(0) : Colour.GetDefaultEntityColour();

            var entity = new Entity(Document.Map.IDGenerator.GetNextObjectID())
            {
                EntityData = new EntityData(gd),
                ClassName = gd.Name,
                Colour = colour,
                Origin = origin
            };

            if (Select.SelectCreatedEntity) entity.IsSelected = true;

            IAction action = new Create(Document.Map.WorldSpawn.ID, entity);

            if (Select.SelectCreatedEntity && Select.DeselectOthersWhenSelectingCreation)
            {
                action = new ActionCollection(new ChangeSelection(new MapObject[0], Document.Selection.GetSelectedObjects()), action);
            }

            Document.PerformAction("Create entity: " + gd.Name, action);
            if (Select.SwitchToSelectAfterEntity)
            {
                Mediator.Publish(HotkeysMediator.SwitchTool, HotkeyTool.Selection);
            }
        }

        public override void KeyUp(ViewportBase viewport, ViewportEvent e)
        {
            //
        }

        public override void UpdateFrame(ViewportBase viewport, FrameInfo frame)
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

            var high = Document.GameData.MapSizeHigh;
            var low = Document.GameData.MapSizeLow;

            if (viewport is Viewport3D)
            {
                var offset = new Coordinate(20, 20, 20);
                var start = _location - offset;
                var end = _location + offset;
                var box = new Box(start, end);
                GL.Begin(PrimitiveType.Lines);
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
                GL.Begin(PrimitiveType.LineLoop);
                GL.Color3(Color.LimeGreen);
                Coord(start.DX, start.DY, start.DZ);
                Coord(end.DX, start.DY, start.DZ);
                Coord(end.DX, end.DY, start.DZ);
                Coord(start.DX, end.DY, start.DZ);
                GL.End();
                GL.Begin(PrimitiveType.Lines);
                var loc = vp.Flatten(_location);
                Coord(low, loc.DY, 0);
                Coord(high, loc.DY, 0);
                Coord(loc.DX, low, 0);
                Coord(loc.DX, high, 0);
                GL.End();
            }
        }

        public override HotkeyInterceptResult InterceptHotkey(HotkeysMediator hotkeyMessage, object parameters)
        {
            switch (hotkeyMessage)
            {
                case HotkeysMediator.OperationsPasteSpecial:
                case HotkeysMediator.OperationsPaste:
                    return HotkeyInterceptResult.SwitchToSelectTool;
            }
            return HotkeyInterceptResult.Continue;
        }

        public override void OverrideViewportContextMenu(ViewportContextMenu menu, Viewport2D vp, ViewportEvent e)
        {
            menu.Items.Clear();
            if (_location == null) return;

            var gd = _sidebarPanel.GetSelectedEntity();
            if (gd != null)
            {
                var item = new ToolStripMenuItem("Create " + gd.Name);
                item.Click += (sender, args) => CreateEntity(_location);
                menu.Items.Add(item);
                menu.Items.Add(new ToolStripSeparator());
            }

            if (_menu != null)
            {
                menu.Items.AddRange(_menu);
            }
        }
    }
}

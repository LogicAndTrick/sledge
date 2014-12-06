using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using Sledge.Common;
using Sledge.Common.Mediator;
using Sledge.DataStructures.GameData;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;
using Sledge.EditorNew.Actions;
using Sledge.EditorNew.Actions.MapObjects.Operations;
using Sledge.EditorNew.Actions.MapObjects.Selection;
using Sledge.EditorNew.Documents;
using Sledge.EditorNew.Properties;
using Sledge.EditorNew.UI;
using Sledge.EditorNew.UI.Viewports;
using Sledge.Gui.Components;
using Sledge.Gui.Containers;
using Sledge.Gui.Controls;
using Sledge.Gui.Interfaces;
using Sledge.Gui.Interfaces.Models;
using Sledge.Gui.Models;
using Sledge.Settings;
using Select = Sledge.Settings.Select;

namespace Sledge.EditorNew.Tools.EntityTool
{
    public class EntitySidebarPanel : VerticalBox
    {
        private readonly ComboBox _entityTypeList;

        public EntitySidebarPanel()
        {
            var label = new Label {TextKey = "Tools/EntityTool/Controls/EntityType"};
            _entityTypeList = new ComboBox();

            this.Add(label);
            this.Add(_entityTypeList);
        }

        public void RefreshEntities(Document doc)
        {
            if (doc == null) return;

            _entityTypeList.StartUpdate();

            var si = _entityTypeList.SelectedItem;
            var selEnt = si == null ? null : si.Value as GameDataObject;
            _entityTypeList.Items.Clear();
            var sel = selEnt == null ? null : selEnt.Name;
            var def = doc.Game.DefaultPointEntity;
            IComboBoxItem reselect = null, redef = null;
            foreach (var gdo in doc.GameData.Classes.Where(x => x.ClassType == ClassType.Point).OrderBy(x => x.Name.ToLowerInvariant()))
            {
                var item = new ComboBoxItem {Value = gdo};
                _entityTypeList.Items.Add(item);
                if (String.Equals(sel, gdo.Name, StringComparison.InvariantCultureIgnoreCase)) reselect = item;
                if (String.Equals(def, gdo.Name, StringComparison.InvariantCultureIgnoreCase)) redef = item;
            }
            if (reselect == null && redef == null) {
                var defSel = doc.GameData.Classes
                .Where(x => x.ClassType == ClassType.Point)
                .OrderBy(x => x.Name.StartsWith("info_player_start") ? 0 : 1)
                .FirstOrDefault();
                redef = _entityTypeList.Items.FirstOrDefault(x => x.Value == defSel);
            }
            _entityTypeList.SelectedItem = reselect ?? redef;

            _entityTypeList.EndUpdate();
        }

        public void Clear()
        {
            _entityTypeList.Items.Clear();
        }

        public GameDataObject GetSelectedEntity()
        {
            var si = _entityTypeList.SelectedItem;
            return si == null ? null : si.Value as GameDataObject;
        }
    }

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
        //private ToolStripItem[] _menu;
        private EntitySidebarPanel _sidebarPanel;

        public EntityTool()
        {
            Usage = ToolUsage.Both;
            _location = new Coordinate(0, 0, 0);
            _state = EntityState.None;
            _sidebarPanel = new EntitySidebarPanel();
        }

        public override IEnumerable<string> GetContexts()
        {
            yield return "Entity Tool";
        }

        public override Image GetIcon()
        {
            return Resources.Tool_Entity;
        }

        public override string GetName()
        {
            return "EntityTool";
        }

        public override HotkeyTool? GetHotkeyToolType()
        {
            return HotkeyTool.Entity;
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

        public override IEnumerable<ToolSidebarControl> GetSidebarControls()
        {
            yield return new ToolSidebarControl { TextKey = GetNameTextKey(), Control = _sidebarPanel };
        }

        public override void DocumentChanged()
        {
            System.Threading.Tasks.Task.Factory.StartNew(BuildMenu);
            _sidebarPanel.RefreshEntities(Document);
        }

        //public override void OverrideViewportContextMenu(ViewportContextMenu menu, Viewport2D vp, ViewportEvent e)
        //{
        //    menu.Items.Clear();
        //    if (_location == null) return;

        //    var gd = _sidebarPanel.GetSelectedEntity();
        //    if (gd != null)
        //    {
        //        var item = new ToolStripMenuItem("Create " + gd.Name);
        //        item.Click += (sender, args) => CreateEntity(_location);
        //        menu.Items.Add(item);
        //        menu.Items.Add(new ToolStripSeparator());
        //    }

        //    if (_menu != null)
        //    {
        //        menu.Items.AddRange(_menu);
        //    }
        //}

        private void BuildMenu()
        {
            //if (_menu != null) foreach (var item in _menu) item.Dispose();
            //_menu = null;
            //if (Document == null) return;

            //var items = new List<ToolStripItem>();
            //var classes = Document.GameData.Classes.Where(x => x.ClassType != ClassType.Base && x.ClassType != ClassType.Solid).ToList();
            //var groups = classes.GroupBy(x => x.Name.Split('_')[0]);
            //foreach (var g in groups)
            //{
            //    var mi = new ToolStripMenuItem(g.Key);
            //    var l = g.ToList();
            //    if (l.Count == 1)
            //    {
            //        var cls = l[0];
            //        mi.Text = cls.Name;
            //        mi.Tag = cls;
            //        mi.Click += ClickMenuItem;
            //    }
            //    else
            //    {
            //        var subs = l.Select(x =>
            //        {
            //            var item = new ToolStripMenuItem(x.Name) { Tag = x };
            //            item.Click += ClickMenuItem;
            //            return item;
            //        }).OfType<ToolStripItem>().ToArray();
            //        mi.DropDownItems.AddRange(subs);
            //    }
            //    items.Add(mi);
            //}
            //_menu = items.ToArray();
        }

        //private void ClickMenuItem(object sender, EventArgs e)
        //{
        //    CreateEntity(_location, ((ToolStripItem)sender).Tag as GameDataObject);
        //}

        protected override void MouseEnter(IViewport2D viewport, ViewportEvent e)
        {
            Cursor.SetCursor(viewport, CursorType.Default);
        }

        protected override void MouseLeave(IViewport2D viewport, ViewportEvent e)
        {
            Cursor.SetCursor(viewport, CursorType.Default);
        }

        protected override void MouseDown(IViewport2D viewport, ViewportEvent e)
        {
            if (e.Button != MouseButton.Left && e.Button != MouseButton.Right) return;

            _state = EntityState.Moving;
            var loc = SnapIfNeeded(viewport.ScreenToWorld(e.X, viewport.Height - e.Y));
            _location = viewport.GetUnusedCoordinate(_location) + viewport.Expand(loc);
        }

        protected override void MouseDown(IViewport3D vp, ViewportEvent e)
        {
            if (e.Button != MouseButton.Left) return;

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

        protected override void MouseUp(IViewport2D viewport, ViewportEvent e)
        {
            if (e.Button != MouseButton.Left) return;
            _state = EntityState.Drawn;
            var loc = SnapIfNeeded(viewport.ScreenToWorld(e.X, viewport.Height - e.Y));
            _location = viewport.GetUnusedCoordinate(_location) + viewport.Expand(loc);
        }

        protected override void DragMove(IViewport2D viewport, ViewportEvent e)
        {
            if (e.Button != MouseButton.Left) return;
            if (_state != EntityState.Moving) return;
            var loc = SnapIfNeeded(viewport.ScreenToWorld(e.X, viewport.Height - e.Y));
            _location = viewport.GetUnusedCoordinate(_location) + viewport.Expand(loc);
        }

        protected override void KeyDown(IViewport2D viewport, ViewportEvent e)
        {
            switch (e.KeyValue)
            {
                case Key.Enter:
                    CreateEntity(_location);
                    _state = EntityState.None;
                    break;
                case Key.Escape:
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

        private static void Coord(Coordinate c)
        {
            GL.Vertex3(c.DX, c.DY, c.DZ);
        }
        private static void Coord(double x, double y, double z)
        {
            GL.Vertex3(x, y, z);
        }

        protected override void Render(IViewport2D viewport)
        {
            if (_state == EntityState.None) return;

            var high = Document.GameData.MapSizeHigh;
            var low = Document.GameData.MapSizeLow;

            var units = viewport.PixelsToUnits(5);
            var offset = new Coordinate(units, units, units);
            var start = viewport.Flatten(_location - offset);
            var end = viewport.Flatten(_location + offset);
            GL.Begin(PrimitiveType.LineLoop);
            GL.Color3(Color.LimeGreen);
            Coord(start.DX, start.DY, start.DZ);
            Coord(end.DX, start.DY, start.DZ);
            Coord(end.DX, end.DY, start.DZ);
            Coord(start.DX, end.DY, start.DZ);
            GL.End();
            GL.Begin(PrimitiveType.Lines);
            var loc = viewport.Flatten(_location);
            Coord(low, loc.DY, 0);
            Coord(high, loc.DY, 0);
            Coord(loc.DX, low, 0);
            Coord(loc.DX, high, 0);
            GL.End();
        }

        protected override void Render(IViewport3D viewport)
        {
            if (_state == EntityState.None) return;

            var high = Document.GameData.MapSizeHigh;
            var low = Document.GameData.MapSizeLow;

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
    }
}

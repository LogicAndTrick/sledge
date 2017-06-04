using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using LogicAndTrick.Oy;
using OpenTK;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Modification.Operations.Tree;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.BspEditor.Rendering;
using Sledge.BspEditor.Rendering.Viewport;
using Sledge.BspEditor.Tools.Properties;
using Sledge.Common;
using Sledge.Common.Shell.Components;
using Sledge.Common.Shell.Context;
using Sledge.Common.Translations;
using Sledge.DataStructures.GameData;
using Sledge.DataStructures.Geometric;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Scenes;
using Sledge.Rendering.Scenes.Elements;
using Line = Sledge.Rendering.Scenes.Renderables.Line;

namespace Sledge.BspEditor.Tools.Entity
{
    [Export(typeof(ITool))]
    [OrderHint("F")]
    [AutoTranslate]
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
        private string _activeEntity;

        public string CreateObject { get; set; } = "Create {0}";

        public EntityTool()
        {
            Usage = ToolUsage.Both;
            _location = new Coordinate(0, 0, 0);
            _state = EntityState.None;
        }

        protected override void ContextChanged(IContext context)
        {
            _activeEntity = context.Get<string>("EntityTool:ActiveEntity");
            Invalidate();

            base.ContextChanged(context);
        }

        public override void DocumentChanged()
        {
            Task.Factory.StartNew(BuildMenu);
            base.DocumentChanged();
        }

        private ToolStripItem[] _menu;

        private async void BuildMenu()
        {
            _menu = null;
            if (Document == null) return;

            var gd = await Document.Environment.GetGameData();
            if (gd == null) return;

            var items = new List<ToolStripItem>();
            var classes = gd.Classes.Where(x => x.ClassType != ClassType.Base && x.ClassType != ClassType.Solid).ToList();
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
                    mi.Click += (s, e) => CreateEntity(_location, cls.Name);
                }
                else
                {
                    var subs = l.Select(x =>
                    {
                        var item = new ToolStripMenuItem(x.Name) { Tag = x };
                        item.Click += (s, e) => CreateEntity(_location, x.Name);
                        return item;
                    }).OfType<ToolStripItem>().ToArray();
                    mi.DropDownItems.AddRange(subs);
                }
                items.Add(mi);
            }
            _menu = items.ToArray();
        }

        public override Image GetIcon()
        {
            return Resources.Tool_Entity;
        }

        public override string GetName()
        {
            return "Entity Tool";
        }

        protected override IEnumerable<Subscription> Subscribe()
        {
            yield return Oy.Subscribe<RightClickMenuBuilder>("MapViewport:RightClick", b =>
            {
                b.Clear();
                b.AddCallback(String.Format(CreateObject, _activeEntity), () => CreateEntity(_location));

                if (_menu == null || _menu.Length <= 0) return;

                b.AddSeparator();
                b.Add(_menu);
            });
        }

        // 3D interaction

        private Coordinate GetIntersectionPoint(IMapObject obj, DataStructures.Geometric.Line line)
        {
            // todo !selection opacity/hidden
            //.Where(x => x.Opacity > 0 && !x.IsHidden)
            return obj?.GetPolygons()
                .Select(x => x.GetIntersectionPoint(line))
                .Where(x => x != null)
                .OrderBy(x => (x - line.Start).VectorMagnitude())
                .FirstOrDefault();
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
            if (e.Button != MouseButtons.Left) return;

            // Get the ray that is cast from the clicked point along the viewport frustrum
            var ray = viewport.CastRayFromScreen(e.X, e.Y);

            // Grab all the elements that intersect with the ray
            var hits = GetBoundingBoxIntersections(ray);

            // Sort the list of intersecting elements by distance from ray origin and grab the first hit
            var hit = hits
                .Select(x => new { Item = x, Intersection = GetIntersectionPoint(x, ray) })
                .Where(x => x.Intersection != null)
                .OrderBy(x => (x.Intersection - ray.Start).VectorMagnitude())
                .FirstOrDefault();

            if (hit == null) return; // Nothing was clicked

            CreateEntity(hit.Intersection);
        }

        // 2D interaction

        protected override void MouseEnter(MapViewport viewport, OrthographicCamera camera, ViewportEvent e)
        {
            viewport.Control.Cursor = Cursors.Cross;
        }

        protected override void MouseLeave(MapViewport viewport, OrthographicCamera camera, ViewportEvent e)
        {
            viewport.Control.Cursor = Cursors.Cross;
        }

        protected override void MouseDown(MapViewport viewport, OrthographicCamera camera, ViewportEvent e)
        {
            if (e.Button != MouseButtons.Left && e.Button != MouseButtons.Right) return;

            _state = EntityState.Moving;
            var loc = SnapIfNeeded(viewport.ProperScreenToWorld(e.X, e.Y));
            _location = viewport.GetUnusedCoordinate(_location) + loc;
            Invalidate();
        }

        protected override void MouseUp(MapViewport viewport, OrthographicCamera camera, ViewportEvent e)
        {
            if (e.Button != MouseButtons.Left) return;
            _state = EntityState.Drawn;
            var loc = SnapIfNeeded(viewport.ProperScreenToWorld(e.X, e.Y));
            _location = viewport.GetUnusedCoordinate(_location) + loc;
            Invalidate();
        }

        protected override void MouseMove(MapViewport viewport, OrthographicCamera camera, ViewportEvent e)
        {
            if (!Control.MouseButtons.HasFlag(MouseButtons.Left)) return;
            if (_state != EntityState.Moving) return;
            var loc = SnapIfNeeded(viewport.ProperScreenToWorld(e.X, e.Y));
            _location = viewport.GetUnusedCoordinate(_location) + loc;
            Invalidate();
        }

        protected override void KeyDown(MapViewport viewport, OrthographicCamera camera, ViewportEvent e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    CreateEntity(_location);
                    _state = EntityState.None;
                    Invalidate();
                    break;
                case Keys.Escape:
                    _state = EntityState.None;
                    Invalidate();
                    break;
            }
        }

        private async Task CreateEntity(Coordinate origin, string gd = null)
        {
            if (gd == null) gd = _activeEntity;
            if (gd == null) return;

            var colour = Colour.GetDefaultEntityColour();
            var data = await Document.Environment.GetGameData();
            if (data != null)
            {
                var cls = data.Classes.FirstOrDefault(x => String.Equals(x.Name, gd, StringComparison.InvariantCultureIgnoreCase));
                if (cls != null)
                {
                    var col = cls.Behaviours.Where(x => x.Name == "color").ToArray();
                    if (col.Any()) colour = col[0].GetColour(0);
                }
            }

            var entity = new Primitives.MapObjects.Entity(Document.Map.NumberGenerator.Next("MapObject"))
            {
                Data =
                {
                    new EntityData { Name = gd },
                    new ObjectColor(colour),
                    new Origin(origin),
                },
                IsSelected = false // Select.SelectCreatedEntity
            };

            var action = new Attach(Document.Map.Root.ID, entity);
            await MapDocumentOperation.Perform(Document, action);

            //if (Select.SwitchToSelectAfterEntity)
            {
                //Mediator.Publish(HotkeysMediator.SwitchTool, HotkeyTool.Selection);
            }
        }

        // Rendering

        protected override IEnumerable<SceneObject> GetSceneObjects()
        {
            var list = base.GetSceneObjects().ToList();

            if (_state != EntityState.None)
            {
                var vec = _location.ToVector3();
                var high = (float) 1024 * 1024;
                var low = (float) -high;
                list.Add(new Line(Color.LimeGreen, new Vector3(low, vec.Y, vec.Z), new Vector3(high, vec.Y, vec.Z)));
                list.Add(new Line(Color.LimeGreen, new Vector3(vec.X, low, vec.Z), new Vector3(vec.X, high, vec.Z)));
                list.Add(new Line(Color.LimeGreen, new Vector3(vec.X, vec.Y, low), new Vector3(vec.X, vec.Y, high)));
            }

            return list;
        }

        protected override IEnumerable<Element> GetViewportElements(MapViewport viewport, OrthographicCamera camera)
        {
            var list = base.GetViewportElements(viewport, camera).ToList();

            if (_state != EntityState.None)
            {
                list.Add(new HandleElement(PositionType.World, HandleElement.HandleType.Square, new Position(_location.ToVector3()), 5)
                {
                    Color = Color.Transparent,
                    LineColor = Color.LimeGreen
                });
            }

            return list;
        }
    }
}

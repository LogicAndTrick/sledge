using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows.Forms;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Modification.Operations.Selection;
using Sledge.BspEditor.Modification.Operations.Tree;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.BspEditor.Rendering.Resources;
using Sledge.BspEditor.Rendering.Viewport;
using Sledge.BspEditor.Tools.Properties;
using Sledge.Common;
using Sledge.Common.Shell.Components;
using Sledge.Common.Shell.Context;
using Sledge.Common.Shell.Hotkeys;
using Sledge.Common.Shell.Settings;
using Sledge.Common.Translations;
using Sledge.DataStructures.GameData;
using Sledge.DataStructures.Geometric;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Pipelines;
using Sledge.Rendering.Primitives;
using Sledge.Rendering.Resources;

namespace Sledge.BspEditor.Tools.Entity
{
    [Export(typeof(ITool))]
    [Export(typeof(ISettingsContainer))]
    [OrderHint("F")]
    [AutoTranslate]
    [DefaultHotkey("Shift+E")]
    public class EntityTool : BaseTool, ISettingsContainer
    {
        private enum EntityState
        {
            None,
            Drawn,
            Moving
        }

        private Vector3 _location;
        private EntityState _state;
        private string _activeEntity;

        public string CreateObject { get; set; } = "Create {0}";

        // Settings

        [Setting("SelectCreatedEntity")] private bool _selectCreatedEntity = true;
        [Setting("SwitchToSelectAfterEntityCreation")] private bool _switchToSelectAfterCreation = false;
        [Setting("ResetEntityTypeOnCreation")] private bool _resetEntityTypeOnCreation = false;

        string ISettingsContainer.Name => "Sledge.BspEditor.Tools.EntityTool";

        IEnumerable<SettingKey> ISettingsContainer.GetKeys()
        {
            yield return new SettingKey("Tools/Entity", "SelectCreatedEntity", typeof(bool));
            yield return new SettingKey("Tools/Entity", "SwitchToSelectAfterEntityCreation", typeof(bool));
            yield return new SettingKey("Tools/Entity", "ResetEntityTypeOnCreation", typeof(bool));
        }

        void ISettingsContainer.LoadValues(ISettingsStore store)
        {
            store.LoadInstance(this);
        }

        void ISettingsContainer.StoreValues(ISettingsStore store)
        {
            store.StoreInstance(this);
        }

        public EntityTool()
        {
            Usage = ToolUsage.Both;
            _location = new Vector3(0, 0, 0);
            _state = EntityState.None;
        }

        protected override void ContextChanged(IContext context)
        {
            _activeEntity = context.Get<string>("EntityTool:ActiveEntity");

            base.ContextChanged(context);
        }

        protected override void DocumentChanged()
        {
            Task.Factory.StartNew(BuildMenu);
            base.DocumentChanged();
        }

        private ToolStripItem[] _menu;

        private async void BuildMenu()
        {
            _menu = null;
            var document = GetDocument();
            if (document == null) return;

            var gd = await document.Environment.GetGameData();
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

        protected override void MouseDown(MapDocument document, MapViewport viewport, PerspectiveCamera camera, ViewportEvent e)
        {
            if (e.Button != MouseButtons.Left) return;

            // Get the ray that is cast from the clicked point along the viewport frustrum
            var (rs, re) = camera.CastRayFromScreen(new Vector3(e.X, e.Y, 0));
            var ray = new Line(rs, re);

            // Grab all the elements that intersect with the ray
            var hit = document.Map.Root.GetIntersectionsForVisibleObjects(ray).FirstOrDefault();

            if (hit == null) return; // Nothing was clicked

            CreateEntity(document, hit.Intersection);
        }

        // 2D interaction

        protected override void MouseEnter(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e)
        {
            viewport.Control.Cursor = Cursors.Cross;
        }

        protected override void MouseLeave(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e)
        {
            viewport.Control.Cursor = Cursors.Cross;
        }

        protected override void MouseDown(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e)
        {
            if (e.Button != MouseButtons.Left && e.Button != MouseButtons.Right) return;

            _state = EntityState.Moving;
            var loc = SnapIfNeeded(camera.ScreenToWorld(e.X, e.Y));
            _location = camera.GetUnusedCoordinate(_location) + loc;
        }

        protected override void MouseUp(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e)
        {
            if (e.Button != MouseButtons.Left) return;
            _state = EntityState.Drawn;
            var loc = SnapIfNeeded(camera.ScreenToWorld(e.X, e.Y));
            _location = camera.GetUnusedCoordinate(_location) + loc;
        }

        protected override void MouseMove(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e)
        {
            if (!Control.MouseButtons.HasFlag(MouseButtons.Left)) return;
            if (_state != EntityState.Moving) return;
            var loc = SnapIfNeeded(camera.ScreenToWorld(e.X, e.Y));
            _location = camera.GetUnusedCoordinate(_location) + loc;
        }

        protected override void KeyDown(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    CreateEntity(document, _location);
                    _state = EntityState.None;
                    break;
                case Keys.Escape:
                    _state = EntityState.None;
                    break;
            }
        }

        private Task CreateEntity(Vector3 origin, string gd = null)
        {
            var document = GetDocument();
            return document == null ? Task.CompletedTask : CreateEntity(document, origin, gd);
        }

        private async Task CreateEntity(MapDocument document, Vector3 origin, string gd = null)
        {
            if (gd == null) gd = _activeEntity;
            if (gd == null) return;

            var colour = Colour.GetDefaultEntityColour();
            var data = await document.Environment.GetGameData();
            if (data != null)
            {
                var cls = data.Classes.FirstOrDefault(x => String.Equals(x.Name, gd, StringComparison.InvariantCultureIgnoreCase));
                if (cls != null)
                {
                    var col = cls.Behaviours.Where(x => x.Name == "color").ToArray();
                    if (col.Any()) colour = col[0].GetColour(0);
                }
            }

            var entity = new Primitives.MapObjects.Entity(document.Map.NumberGenerator.Next("MapObject"))
            {
                Data =
                {
                    new EntityData { Name = gd },
                    new ObjectColor(colour),
                    new Origin(origin),
                }
            };

            var transaction = new Transaction();

            transaction.Add(new Attach(document.Map.Root.ID, entity));

            if (_selectCreatedEntity)
            {
                transaction.Add(new Deselect(document.Selection));
                transaction.Add(new Select(entity.FindAll()));
            }

            await MapDocumentOperation.Perform(document, transaction);

            if (_switchToSelectAfterCreation)
            {
                Oy.Publish("ActivateTool", "SelectTool");
            }

            if (_resetEntityTypeOnCreation)
            {
                Oy.Publish("EntityTool:ResetEntityType", this);
            }
        }

        // Rendering

        protected override void Render(MapDocument document, BufferBuilder builder, ResourceCollector resourceCollector)
        {
            if (_state != EntityState.None)
            {
                var vec = _location;
                var high = 1024f * 1024f;
                var low = -high;

                // Draw a box around the point
                var c = new Box(vec - Vector3.One * 10, vec + Vector3.One * 10);

                const uint numVertices = 4 * 6 + 6;
                const uint numWireframeIndices = numVertices * 2;

                var points = new VertexStandard[numVertices];
                var indices = new uint[numWireframeIndices];

                var colour = new Vector4(0, 1, 0, 1);

                var vi = 0u;
                var wi = 0u;
                foreach (var face in c.GetBoxFaces())
                {
                    var offs = vi;

                    foreach (var v in face)
                    {
                        points[vi++] = new VertexStandard { 
                            Position = v,
                            Colour = colour,
                            Tint = Vector4.One
                        };
                    }

                    // Lines - [0 1] ... [n-1 n] [n 0]
                    for (uint i = 0; i < 4; i++)
                    {
                        indices[wi++] = offs + i;
                        indices[wi++] = offs + (i == 4 - 1 ? 0 : i + 1);
                    }
                }

                // Draw 3 lines pinpointing the point
                var lineOffset = vi;

                points[vi++] = new VertexStandard { Position = new Vector3(low , vec.Y, vec.Z), Colour = colour, Tint = Vector4.One };
                points[vi++] = new VertexStandard { Position = new Vector3(high, vec.Y, vec.Z), Colour = colour, Tint = Vector4.One };
                points[vi++] = new VertexStandard { Position = new Vector3(vec.X, low , vec.Z), Colour = colour, Tint = Vector4.One };
                points[vi++] = new VertexStandard { Position = new Vector3(vec.X, high, vec.Z), Colour = colour, Tint = Vector4.One };
                points[vi++] = new VertexStandard { Position = new Vector3(vec.X, vec.Y, low ), Colour = colour, Tint = Vector4.One };
                points[vi++] = new VertexStandard { Position = new Vector3(vec.X, vec.Y, high), Colour = colour, Tint = Vector4.One };

                indices[wi++] = lineOffset++;
                indices[wi++] = lineOffset++;
                indices[wi++] = lineOffset++;
                indices[wi++] = lineOffset++;
                indices[wi++] = lineOffset++;
                indices[wi++] = lineOffset++;
                
                var groups = new[]
                {
                    new BufferGroup(PipelineType.Wireframe, CameraType.Both, 0, numWireframeIndices)
                };

                builder.Append(points, indices, groups);
            }

            base.Render(document, builder, resourceCollector);
        }
    }
}

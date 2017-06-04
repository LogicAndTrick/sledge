using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Modification.Operations.Tree;
using Sledge.BspEditor.Primitives;
using Sledge.BspEditor.Primitives.MapData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.BspEditor.Rendering.Converters;
using Sledge.BspEditor.Rendering.Viewport;
using Sledge.BspEditor.Tools.Draggable;
using Sledge.BspEditor.Tools.Properties;
using Sledge.Common.Shell.Components;
using Sledge.Common.Shell.Context;
using Sledge.Common.Shell.Settings;
using Sledge.Common.Translations;
using Sledge.DataStructures.Geometric;
using Sledge.Rendering.Scenes;
using Sledge.Rendering.Scenes.Renderables;

namespace Sledge.BspEditor.Tools.Brush
{
    [Export(typeof(ITool))]
    [Export(typeof(ISettingsContainer))]
    [OrderHint("H")]
    [AutoTranslate]
    public class BrushTool : BaseDraggableTool, ISettingsContainer
    {
        private bool _updatePreview;
        private List<Face> _preview;
        private BoxDraggableState box;
        private IBrush _activeBrush;

        [Import] private Lazy<MapObjectConverter> _converter;
        
        public string CreateObject { get; set; } = "Create Object";
        
        // Settings

        [Setting("SelectionBoxBackgroundOpacity")] private int _selectionBoxBackgroundOpacity = 64;
        [Setting("SwitchToSelectAfterCreation")] private bool _switchToSelectAfterCreation = false;
        [Setting("ResetBrushTypeOnCreation")] private bool _resetBrushTypeOnCreation = false;

        string ISettingsContainer.Name => "Sledge.BspEditor.Tools.BrushTool";

        IEnumerable<SettingKey> ISettingsContainer.GetKeys()
        {
            yield return new SettingKey("Brush", "SelectionBoxBackgroundOpacity", typeof(int));
            yield return new SettingKey("Brush", "SwitchToSelectAfterCreation", typeof(bool));
            yield return new SettingKey("Brush", "ResetBrushTypeOnCreation", typeof(bool));
        }

        void ISettingsContainer.LoadValues(ISettingsStore store)
        {
            store.LoadInstance(this);
        }

        void ISettingsContainer.StoreValues(ISettingsStore store)
        {
            store.StoreInstance(this);
        }

        public BrushTool()
        {
            box = new BoxDraggableState(this);
            box.BoxColour = Color.Turquoise;
            box.FillColour = Color.FromArgb(_selectionBoxBackgroundOpacity, Color.Green);
            box.State.Changed += BoxChanged;
            States.Add(box);

            UseValidation = true;
        }

        protected override IEnumerable<Subscription> Subscribe()
        {
            yield return Oy.Subscribe<Change>("MapDocument:Changed", x =>
            {
                if (x.Document == Document)
                {
                    TextureSelected();
                }
                return Task.FromResult(0);
            });
            yield return Oy.Subscribe<object>("BrushTool:ValuesChanged", x =>
            {
                _updatePreview = true;
                Invalidate();
            });
            yield return Oy.Subscribe<RightClickMenuBuilder>("MapViewport:RightClick", x =>
            {
                x.Clear();
                x.AddCallback(String.Format(CreateObject, _activeBrush?.Name), () => Confirm(x.Viewport));
            });
        }

        protected override void ContextChanged(IContext context)
        {
            _activeBrush = context.Get<IBrush>("BrushTool:ActiveBrush");
            _updatePreview = true;
            Invalidate();

            base.ContextChanged(context);
        }

        public override void ToolSelected()
        {
            var sel = Document.Selection.OfType<Solid>().ToList();
            if (sel.Any())
            {
                box.RememberedDimensions = new Box(sel.Select(x => x.BoundingBox));
            }
            else if (box.RememberedDimensions == null)
            {
                var gs = Document.Map.Data.GetOne<GridData>()?.Grid;
                var start = Coordinate.Zero;
                var next = gs?.AddStep(Coordinate.Zero, Coordinate.One) ?? Coordinate.One * 64;
                box.RememberedDimensions = new Box(start, next);
            }

            _updatePreview = true;
            base.ToolSelected();
        }

        public override void ToolDeselected()
        {
            _updatePreview = false;
            base.ToolDeselected();
        }

        private void TextureSelected()
        {
            _updatePreview = true;
            Invalidate();
        }

        private void BoxChanged(object sender, EventArgs e)
        {
            _updatePreview = true;
            Invalidate();
        }

        public override Image GetIcon()
        {
            return Resources.Tool_Brush;
        }

        public override string GetName()
        {
            return "BrushTool";
        }

        public override void KeyDown(MapViewport viewport, ViewportEvent e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Confirm(viewport);
            }
            else if (e.KeyCode == Keys.Escape)
            {
                Cancel(viewport);
            }
            base.KeyDown(viewport, e);
        }

        private void CreateBrush(Box bounds)
        {
            var brush = GetBrush(bounds, Document.Map.NumberGenerator);
            if (brush == null) return;

            brush.IsSelected = true;
            brush.FindAll().ForEach(x => x.IsSelected = true);

            MapDocumentOperation.Perform(Document, new Attach(Document.Map.Root.ID, brush));
        }

        private IMapObject GetBrush(Box bounds, UniqueNumberGenerator idg)
        {
            var brush = _activeBrush;
            if (brush == null) return null;

            var ti = Document.Map.Data.GetOne<ActiveTexture>()?.Name ?? "aaatrigger";
            var created = brush.Create(idg, bounds, ti, /*BrushManager.RoundCreatedVertices*/ false ? 0 : 2).ToList();
            if (created.Count > 1)
            {
                var g = new Group(idg.Next("MapObject"));
                created.ForEach(x => x.Hierarchy.Parent = g);
                g.DescendantsChanged();
                return g;
            }
            return created.FirstOrDefault();
        }

        private void Confirm(MapViewport viewport)
        {
            if (box.State.Action != BoxAction.Drawn) return;
            var bbox = new Box(box.State.Start, box.State.End);
            if (!bbox.IsEmpty())
            {
                CreateBrush(bbox);
                box.RememberedDimensions = bbox;
            }
            _preview = null;
            box.State.Action = BoxAction.Idle;
            if (_switchToSelectAfterCreation)
            {
                Oy.Publish("ActivateTool", "SelectTool");
            }
            if (_resetBrushTypeOnCreation)
            {
                Oy.Publish("BrushTool:ResetBrushType", this);
            }
        }

        private void Cancel(MapViewport viewport)
        {
            box.RememberedDimensions = new Box(box.State.Start, box.State.End);
            _preview = null;
            box.State.Action = BoxAction.Idle;
        }

        private List<Face> GetPreview()
        {
            if (_updatePreview)
            {
                var bbox = new Box(box.State.Start, box.State.End);
                var brush = GetBrush(bbox, new UniqueNumberGenerator()).FindAll();
                var converted = brush.Select(x => _converter.Value.Convert(Document, x)).Where(x => x != null);
                Task.WhenAll(converted).ContinueWith(result =>
                {
                    var objects = result.Result.SelectMany(x => x.SceneObjects.Values).ToList();
                    foreach (var o in objects.OfType<RenderableObject>())
                    {
                        o.AccentColor = Color.Turquoise;
                        o.TintColor = Color.FromArgb(64, Color.Turquoise);
                    }
                    _preview = objects.OfType<Face>().ToList();
                });
            }
            _updatePreview = false;
            return _preview ?? new List<Face>();
        }

        protected override IEnumerable<SceneObject> GetSceneObjects()
        {
            var list = base.GetSceneObjects().ToList();

            if (box.State.Action != BoxAction.Idle)
            {
                list.AddRange(GetPreview());
            }

            return list;
        }
    }
}
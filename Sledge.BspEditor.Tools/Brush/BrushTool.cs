using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Primitives;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.BspEditor.Rendering.Converters;
using Sledge.BspEditor.Rendering.Viewport;
using Sledge.BspEditor.Tools.Brush.Brushes;
using Sledge.BspEditor.Tools.Draggable;
using Sledge.Common.Logging;
using Sledge.Common.Shell.Components;
using Sledge.Common.Shell.Context;
using Sledge.DataStructures.Geometric;
using Sledge.Rendering.Scenes;
using Sledge.Rendering.Scenes.Renderables;

namespace Sledge.BspEditor.Tools.Brush
{
    [Export(typeof(ITool))]
    public class BrushTool : BaseDraggableTool
    {
        private bool _updatePreview;
        private List<Face> _preview;
        private BoxDraggableState box;
        private IBrush _activeBrush;

        [Import] private Lazy<MapObjectConverter> _converter;

        public BrushTool()
        {
            //_propertiesControl = new BrushPropertiesControl();
            //_propertiesControl.ValuesChanged += ValuesChanged;

            box = new BoxDraggableState(this);
            box.BoxColour = Color.Turquoise;
            box.FillColour = Color.FromArgb(/*View.SelectionBoxBackgroundOpacity*/ 64, Color.Green);
            box.State.Changed += BoxChanged;
            States.Add(box);

            UseValidation = true;
        }

        protected override void ContextChanged(IContext context)
        {
            _activeBrush = context.Get<IBrush>("BrushTool:ActiveBrush");
            _updatePreview = true;
            Invalidate();

            base.ContextChanged(context);
        }

        public override void ToolSelected(bool preventHistory)
        {
            //BrushManager.ValuesChanged += ValuesChanged;
            //var sel = Document.Selection.GetSelectedObjects().OfType<Solid>().ToList();
            //if (sel.Any())
            //{
            //    box.RememberedDimensions = new Box(sel.Select(x => x.BoundingBox));
            //}
            //else if (box.RememberedDimensions == null)
            //{
            //    var gs = Document.Map.GridSpacing;
            //    box.RememberedDimensions = new Box(Coordinate.Zero, new Coordinate(gs, gs, gs));
            //}

            //Mediator.Subscribe(EditorMediator.TextureSelected, this);

            _updatePreview = true;
            base.ToolSelected(preventHistory);
        }

        public override void ToolDeselected(bool preventHistory)
        {
            //BrushManager.ValuesChanged -= ValuesChanged;
            //Mediator.UnsubscribeAll(this);
            _updatePreview = false;
            base.ToolDeselected(preventHistory);
        }

        private void TextureSelected(string texture)
        {
            _updatePreview = true;
            Invalidate();
        }

        private void ValuesChanged(IBrush brush)
        {
            //if (BrushManager.CurrentBrush == brush) _updatePreview = true;
            Invalidate();
        }

        private void BoxChanged(object sender, EventArgs e)
        {
            _updatePreview = true;
            Invalidate();
        }

        public override IEnumerable<KeyValuePair<string, Control>> GetSidebarControls()
        {
            yield break;
            //yield return new KeyValuePair<string, Control>(GetName(), BrushManager.SidebarControl);
        }

        public override string GetContextualHelp()
        {
            return "Draw a box in the 2D view to define the size of the brush.\n" +
                   "Select the type of the brush to create in the sidebar.\n" +
                   "Press *enter* in the 2D view to create the brush.";
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

            MapDocumentOperation.Perform(Document, new Attach(Document.Map.Root.ID, brush));

            Log.Info(nameof(BrushTool),
                "Brush create requested: " + JsonConvert.SerializeObject(brush, Formatting.None,
                    new JsonSerializerSettings {ReferenceLoopHandling = ReferenceLoopHandling.Ignore}));

            //brush.IsSelected = Select.SelectCreatedBrush;
            //IAction action = new Create(Document.Map.WorldSpawn.ID, brush);
            //if (Select.SelectCreatedBrush && Select.DeselectOthersWhenSelectingCreation)
            //{
            //    action = new ActionCollection(new ChangeSelection(new MapObject[0], Document.Selection.GetSelectedObjects()), action);
            //}

            //Document.PerformAction("Create " + BrushManager.CurrentBrush.Name, action);
        }

        private IMapObject GetBrush(Box bounds, UniqueNumberGenerator idg)
        {
            var brush = _activeBrush;
            if (brush == null) return null;

            var ti = "aaatrigger"; //Document.TextureCollection.SelectedTexture ?? "";
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
            //if (Select.SwitchToSelectAfterCreation)
            //{
            //    Mediator.Publish(HotkeysMediator.SwitchTool, HotkeyTool.Selection);
            //}
            //if (Select.ResetBrushTypeOnCreation)
            //{
            //    Mediator.Publish(EditorMediator.ResetSelectedBrushType);
            //}
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
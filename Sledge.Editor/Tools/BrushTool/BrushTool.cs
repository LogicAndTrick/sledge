using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sledge.Common.Mediator;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Actions;
using Sledge.Editor.Actions.MapObjects.Operations;
using Sledge.Editor.Actions.MapObjects.Selection;
using Sledge.Editor.Brushes;
using Sledge.Editor.Properties;
using Sledge.Editor.Rendering;
using Sledge.Editor.Tools.DraggableTool;
using Sledge.Providers.Texture;
using Sledge.Rendering.Materials;
using Sledge.Rendering.Scenes;
using Sledge.Settings;
using Select = Sledge.Settings.Select;
using View = Sledge.Settings.View;

namespace Sledge.Editor.Tools.BrushTool
{
    public class BrushTool : BaseDraggableTool
    {
        private bool _updatePreview;
        private List<Sledge.Rendering.Scenes.Renderables.Face> _preview;
        private BoxDraggableState box;
        //private BrushPropertiesControl _propertiesControl;

        public BrushTool()
        {
            //_propertiesControl = new BrushPropertiesControl();
            //_propertiesControl.ValuesChanged += ValuesChanged;

            box = new BoxDraggableState(this);
            box.BoxColour = Color.Turquoise;
            box.FillColour = Color.FromArgb(View.SelectionBoxBackgroundOpacity, Color.Green);
            box.State.Changed += BoxChanged;
            States.Add(box);

            UseValidation = true;
        }

        public override void ToolSelected(bool preventHistory)
        {
            BrushManager.ValuesChanged += ValuesChanged;
            var sel = Document.Selection.GetSelectedObjects().OfType<Solid>().ToList();
            if (sel.Any())
            {
                box.RememberedDimensions = new Box(sel.Select(x => x.BoundingBox));
            }
            else if (box.RememberedDimensions == null)
            {
                var gs = Document.Map.GridSpacing;
                box.RememberedDimensions = new Box(Coordinate.Zero, new Coordinate(gs, gs, gs));
            }

            Mediator.Subscribe(EditorMediator.TextureSelected, this);

            _updatePreview = true;
            base.ToolSelected(preventHistory);
        }

        public override void ToolDeselected(bool preventHistory)
        {
            BrushManager.ValuesChanged -= ValuesChanged;
            Mediator.UnsubscribeAll(this);
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
            if (BrushManager.CurrentBrush == brush) _updatePreview = true;
            Invalidate();
        }

        private void BoxChanged(object sender, EventArgs e)
        {
            _updatePreview = true;
            Invalidate();
        }

        public override IEnumerable<KeyValuePair<string, Control>> GetSidebarControls()
        {
            yield return new KeyValuePair<string, Control>(GetName(), BrushManager.SidebarControl);
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
            var brush = GetBrush(bounds, Document.Map.IDGenerator);
            if (brush == null) return;

            brush.IsSelected = Select.SelectCreatedBrush;
            IAction action = new Create(Document.Map.WorldSpawn.ID, brush);
            if (Select.SelectCreatedBrush && Select.DeselectOthersWhenSelectingCreation)
            {
                action = new ActionCollection(new ChangeSelection(new MapObject[0], Document.Selection.GetSelectedObjects()), action);
            }

            Document.PerformAction("Create " + BrushManager.CurrentBrush.Name, action);
        }

        private MapObject GetBrush(Box bounds, IDGenerator idg)
        {
            var brush = BrushManager.CurrentBrush;
            if (brush == null) return null;
            var ti = Document.TextureCollection.SelectedTexture ?? "";
            var created = brush.Create(idg, bounds, ti, BrushManager.RoundCreatedVertices ? 0 : 2).ToList();
            if (created.Count > 1)
            {
                var g = new Group(idg.GetNextObjectID());
                created.ForEach(x => x.SetParent(g));
                g.UpdateBoundingBox();
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
            if (Select.SwitchToSelectAfterCreation)
            {
                Mediator.Publish(HotkeysMediator.SwitchTool, HotkeyTool.Selection);
            }
            if (Select.ResetBrushTypeOnCreation)
            {
                Mediator.Publish(EditorMediator.ResetSelectedBrushType);
            }
        }

        private void Cancel(MapViewport viewport)
        {
            box.RememberedDimensions = new Box(box.State.Start, box.State.End);
            _preview = null;
            box.State.Action = BoxAction.Idle;
        }

        private List<Sledge.Rendering.Scenes.Renderables.Face> GetPreview()
        {
            if (_updatePreview)
            {
                var bbox = new Box(box.State.Start, box.State.End);
                var brush = GetBrush(bbox, new IDGenerator()).FindAll();
                var converted = brush.Select(x => MapObjectConverter.Convert(Document, x)).Where(x => x != null);
                Task.WhenAll(converted).ContinueWith(result =>
                {
                    var objects = result.Result.SelectMany(x => x.SceneObjects.Values).ToList();
                    foreach (var o in _preview)
                    {
                        o.AccentColor = Color.Turquoise;
                        o.TintColor = Color.FromArgb(64, Color.Turquoise);
                    }
                    _preview = objects.OfType<Sledge.Rendering.Scenes.Renderables.Face>().ToList();
                });
            }
            _updatePreview = false;
            return _preview ?? new List<Sledge.Rendering.Scenes.Renderables.Face>();
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

        public override HotkeyTool? GetHotkeyToolType()
        {
            return HotkeyTool.Brush;
        }

        public override HotkeyInterceptResult InterceptHotkey(HotkeysMediator hotkeyMessage, object parameters)
        {
            return HotkeyInterceptResult.Continue;
        }
    }
}
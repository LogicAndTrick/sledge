using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using Sledge.Common.Mediator;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;
using Sledge.EditorNew.Actions;
using Sledge.EditorNew.Actions.MapObjects.Operations;
using Sledge.EditorNew.Actions.MapObjects.Selection;
using Sledge.EditorNew.Brushes;
using Sledge.EditorNew.Properties;
using Sledge.EditorNew.Rendering.Immediate;
using Sledge.EditorNew.Tools.DraggableTool;
using Sledge.EditorNew.UI.Viewports;
using Sledge.Graphics.Helpers;
using Sledge.Gui.Structures;
using Sledge.Settings;
using Select = Sledge.Settings.Select;

namespace Sledge.EditorNew.Tools.BrushTool
{
    public class BrushTool : BaseDraggableTool
    {
        private bool _updatePreview;
        private List<Face> _preview;
        private BoxDraggableState box;
        private BrushPropertiesControl _propertiesControl;

        public BrushTool()
        {
            _propertiesControl = new BrushPropertiesControl();

            box = new BoxDraggableState(this);
            box.BoxColour = Color.Turquoise;
            box.FillColour = Color.FromArgb(View.SelectionBoxBackgroundOpacity, Color.Green);
            States.Add(box);
        }

        public override void ToolSelected(bool preventHistory)
        {
            _propertiesControl.ValuesChanged += ValuesChanged;
            box.State.Changed += BoxChanged;
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
            _updatePreview = true;
        }

        public override void ToolDeselected(bool preventHistory)
        {
            box.State.Changed -= BoxChanged;
            _propertiesControl.ValuesChanged -= ValuesChanged;
            _updatePreview = false;
        }

        private void ValuesChanged(IBrush brush)
        {
            if (_propertiesControl.CurrentBrush == brush) _updatePreview = true;
        }

        private void BoxChanged(object sender, EventArgs e)
        {
            _updatePreview = true;
        }

        public override IEnumerable<ToolSidebarControl> GetSidebarControls()
        {
            yield return new ToolSidebarControl{Control = _propertiesControl, TextKey = GetNameTextKey()};
        }

        public override IEnumerable<string> GetContexts()
        {
            yield return "Brush Tool";
        }

        public override Image GetIcon()
        {
            return Resources.Tool_Brush;
        }

        public override string GetName()
        {
            return "BrushTool";
        }

        public override void KeyDown(IMapViewport viewport, ViewportEvent e)
        {
            if (e.KeyValue == Key.Enter || e.KeyValue == Key.KeypadEnter)
            {
                Confirm();
            }
            else if (e.KeyValue == Key.Escape)
            {
                Cancel();
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

            Document.PerformAction("Create " + brush, action);
        }

        private MapObject GetBrush(Box bounds, IDGenerator idg)
        {
            var brush = _propertiesControl.CurrentBrush;
            if (brush == null) return null;
            var ti = Document.TextureCollection.SelectedTexture;
            var texture = ti != null ? ti.GetTexture() : null;
            var created = brush.Create(idg, bounds, texture, _propertiesControl.RoundVertices ? 0 : 2).ToList();
            if (created.Count > 1)
            {
                var g = new Group(idg.GetNextObjectID());
                created.ForEach(x => x.SetParent(g));
                g.UpdateBoundingBox();
                return g;
            }
            return created.FirstOrDefault();
        }

        private void Confirm()
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

        private void Cancel()
        {
            box.RememberedDimensions = new Box(box.State.Start, box.State.End);
            _preview = null;
            box.State.Action = BoxAction.Idle;
        }

        public override void UpdateFrame(IMapViewport viewport, Frame frame)
        {
            if (_updatePreview && box.State.Action != BoxAction.Idle)
            {
                var bbox = new Box(box.State.Start, box.State.End);
                var brush = GetBrush(bbox, new IDGenerator());
                _preview = new List<Face>();
                CollectFaces(_preview, new[] { brush });
                var color = box.BoxColour;
                _preview.ForEach(x => { x.Colour = color; });
            }
            _updatePreview = false;
        }

        protected override void Render(IViewport2D viewport)
        {
            base.Render(viewport);
            if (box.State.Action == BoxAction.Idle || _preview == null) return;

            GL.Color3(Color.FromArgb(128, box.BoxColour));
            Graphics.Helpers.Matrix.Push();
            var matrix = viewport.GetModelViewMatrix();
            GL.MultMatrix(ref matrix);
            MapObjectRenderer.DrawWireframe(_preview, true, false);
            Graphics.Helpers.Matrix.Pop();
        }

        protected override void Render(IViewport3D viewport)
        {
            base.Render(viewport);
            if (box.State.Action == BoxAction.Idle || _preview == null) return;

            GL.Disable(EnableCap.CullFace);
            TextureHelper.Unbind();
            if (viewport.Type != ViewType.Flat) MapObjectRenderer.EnableLighting();
            MapObjectRenderer.DrawFilled(_preview, Color.FromArgb(128, box.BoxColour), false);
            MapObjectRenderer.DisableLighting();
            GL.Color4(Color.GreenYellow);
            MapObjectRenderer.DrawWireframe(_preview, true, false);
        }

        private static void CollectFaces(List<Face> faces, IEnumerable<MapObject> list)
        {
            foreach (var mo in list)
            {
                if (mo is Solid)
                {
                    faces.AddRange(((Solid)mo).Faces);
                }
                else if (mo is Entity || mo is Group)
                {
                    CollectFaces(faces, mo.GetChildren());
                }
            }
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
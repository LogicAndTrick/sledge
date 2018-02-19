using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Modification.Operations.Selection;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.BspEditor.Rendering.Converters;
using Sledge.BspEditor.Rendering.Viewport;
using Sledge.BspEditor.Tools.Draggable;
using Sledge.BspEditor.Tools.Properties;
using Sledge.BspEditor.Tools.Vertex.Selection;
using Sledge.BspEditor.Tools.Vertex.Tools;
using Sledge.Common;
using Sledge.Common.Shell.Components;
using Sledge.Common.Shell.Documents;
using Sledge.Common.Shell.Hooks;
using Sledge.Common.Translations;
using Sledge.DataStructures.Geometric;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Scenes;
using Sledge.Rendering.Scenes.Renderables;
using Sledge.Shell.Input;
using Line = Sledge.DataStructures.Geometric.Line;

namespace Sledge.BspEditor.Tools.Vertex
{
    [Export(typeof(ITool))]
    [Export(typeof(IInitialiseHook))]
    [OrderHint("P")]
    [AutoTranslate]
    public class VertexTool : BaseDraggableTool, IInitialiseHook
    {
        //private readonly VMSidebarPanel _controlPanel;
        //private readonly VMErrorsSidebarPanel _errorPanel;
        [ImportMany] private IEnumerable<Lazy<VertexSubtool>> _subTools;
        [Import] private Lazy<MapObjectConverter> _converter;

        public Task OnInitialise()
        {
            foreach (var st in _subTools.OrderBy(x => x.Value.OrderHint))
            {
                st.Value.Selection = _selection;
                st.Value.Active = Children.Count == 0;
                Children.Add(st.Value);
            }

            return Task.FromResult(false);
        }
        
        private readonly BoxDraggableState _boxState;

        private readonly VertexSelection _selection;
        
        public VertexTool()
        {
            //_controlPanel = new VMSidebarPanel();
            //_controlPanel.ToolSelected += SubToolSelected;
            //_controlPanel.DeselectAll += x => DeselectAll();
            //_controlPanel.Reset += Reset;

            //_errorPanel = new VMErrorsSidebarPanel();

            Usage = ToolUsage.Both;

            _boxState = new BoxDraggableState(this);
            _boxState.BoxColour = Color.Orange;
            _boxState.FillColour = Color.FromArgb(64, Color.DodgerBlue);
            _boxState.DragStarted += (sender, args) =>
            {
                if (!KeyboardState.Ctrl)
                {
                    DeselectAll();
                }
            };
            
            States.Add(_boxState);

            UseValidation = true;

            _selection = new VertexSelection();
        }

        public override Image GetIcon()
        {
            return Resources.Tool_VM;
        }

        public override string GetName()
        {
            return "Vertex Manipulation Tool";
        }

        protected override IEnumerable<Subscription> Subscribe()
        {
            yield return Oy.Subscribe<IDocument>("MapDocument:SelectionChanged", x =>
            {
                if (x == Document) SelectionChanged();
            });
        }

        public override async Task ToolSelected()
        {
            await SelectionChanged();
            var ct = CurrentSubTool;
            if (ct != null) await ct.ToolSelected();
            await base.ToolSelected();
        }

        public override async Task ToolDeselected()
        {
            await _selection.Clear(Document);
            var ct = CurrentSubTool;
            if (ct != null) await ct.ToolDeselected();
            await base.ToolDeselected();
        }

        private async Task SelectionChanged()
        {
            await _selection.Commit(Document);
            await _selection.Update(Document);
            var ct = CurrentSubTool;
            if (ct != null) await ct.SelectionChanged();
            Invalidate();
        }

        #region Tool switching
        
        private VertexSubtool CurrentSubTool
        {
            get { return Children.OfType<VertexSubtool>().FirstOrDefault(x => x.Active); }
            set
            {
                foreach (var tool in Children.Where(x => x != value && x.Active))
                {
                    tool.ToolDeselected();
                    tool.Active = false;
                }
                if (value != null)
                {
                    value.Active = true;
                    value.ToolSelected();
                }
            }
        }

        #endregion
        
        #region 3D interaction
        
        private Coordinate GetIntersectionPoint(Solid obj, Line line)
        {
            // todo !selection opacity/hidden
            //.Where(x => x.Opacity > 0 && !x.IsHidden)
            return obj?.GetPolygons()
                .Select(x => x.GetIntersectionPoint(line))
                .Where(x => x != null)
                .OrderBy(x => (x - line.Start).VectorMagnitude())
                .FirstOrDefault();
        }

        private IEnumerable<Solid> GetBoundingBoxIntersections(Line ray)
        {
            return Document.Map.Root.Collect(
                x => x is Root || (x.BoundingBox != null && x.BoundingBox.IntersectsWith(ray)),
                x => x is Solid && x.Hierarchy.Parent != null && !x.Hierarchy.HasChildren
            ).OfType<Solid>();
        }

        /// <summary>
        /// When the mouse is pressed in the 3D view, we want to select the clicked object.
        /// </summary>
        /// <param name="viewport">The viewport that was clicked</param>
        /// <param name="camera"></param>
        /// <param name="e">The click event</param>
        protected override void MouseDown(MapViewport viewport, PerspectiveCamera camera, ViewportEvent e)
        {
            // First, get the ray that is cast from the clicked point along the viewport frustrum
            var ray = viewport.CastRayFromScreen(e.X, e.Y);

            // Grab all the elements that intersect with the ray
            var hits = GetBoundingBoxIntersections(ray);

            // Sort the list of intersecting elements by distance from ray origin
            var closestObject = hits
                .Select(x => new { Item = x, Intersection = GetIntersectionPoint(x, ray) })
                .Where(x => x.Intersection != null)
                .OrderBy(x => (x.Intersection - ray.Start).VectorMagnitude())
                .Select(x => x.Item)
                .FirstOrDefault();

            // Nothing was clicked, don't change the selection
            if (closestObject == null) return;
            
            var operation = new Transaction();

            // Ctrl doesn't toggle selection, only adds to it.
            // Ctrl+clicking a selected solid will do nothing.

            if (!KeyboardState.Ctrl)
            {
                // Ctrl isn't down, so we want to clear the selection
                operation.Add(new Deselect(Document.Selection.Where(x => !ReferenceEquals(x, closestObject)).ToList()));
            }

            if (!closestObject.IsSelected)
            {
                // The clicked object isn't selected yet, select it.
                operation.Add(new Select(closestObject));
            }

            if (!operation.IsEmpty)
            {
                MapDocumentOperation.Perform(Document, operation);
            }
        }

        #endregion

        private void Reset(object sender)
        {
            
        }

        private void DeselectAll()
        {
            
        }

        public override void KeyDown(MapViewport viewport, ViewportEvent e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Confirm();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                Cancel();
                e.Handled = true;
            }
            base.KeyDown(viewport, e);
        }

        private void Cancel()
        {
            
        }

        private void Confirm()
        {
            
        }

        protected override IEnumerable<SceneObject> GetSceneObjects()
        {
            var tasks = _selection.Select(x => _converter.Value.Convert(Document, x.Copy)).ToList();
            Task.WaitAll(tasks.OfType<Task>().ToArray());
            var objects = tasks.Select(x => x.Result).Where(x => x != null).SelectMany(x => x.SceneObjects.Values).ToList();
            foreach (var o in objects.OfType<RenderableObject>())
            {
                o.ForcedRenderFlags |= RenderFlags.Wireframe;
                o.TintColor = Colour.Blend(Color.FromArgb(128, Color.Green), o.TintColor);
                o.AccentColor = Color.White;
            }
            objects.AddRange(Children.OfType<VertexSubtool>().SelectMany(x => GetSceneObjects()));
            objects.AddRange(base.GetSceneObjects());
            return objects;
        }

        public new void Invalidate()
        {
            //_errorPanel.SetErrorList(GetErrors());
            base.Invalidate();
        }
    }
}

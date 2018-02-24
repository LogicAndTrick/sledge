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
using Sledge.BspEditor.Tools.Vertex.Errors;
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
    [Export]
    [OrderHint("P")]
    [AutoTranslate]
    public class VertexTool : BaseDraggableTool, IInitialiseHook
    {
        [ImportMany] private IEnumerable<Lazy<VertexSubtool>> _subTools;
        [ImportMany] private IEnumerable<Lazy<IVertexErrorCheck>> _errorChecks;
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

        private readonly VertexSelection _selection;
        
        public VertexTool()
        {
            Usage = ToolUsage.Both;

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
            yield return Oy.Subscribe<Type>("VertexTool:SetSubTool", t =>
            {
                CurrentSubTool = _subTools.Select(x => x.Value).FirstOrDefault(x => x.GetType() == t);
            });
            yield return Oy.Subscribe<String>("VertexTool:Reset", async _ =>
            {
                await _selection.Reset(Document);
                CurrentSubTool?.Update();
                Invalidate();
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
            await _selection.Commit(Document);
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
        
        internal VertexSubtool CurrentSubTool
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

                Oy.Publish("VertexTool:SubToolChanged", value?.GetType());
            }
        }

        #endregion

        private void SelectObject(Solid closestObject)
        {
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

            SelectObject(closestObject);
        }

        #endregion

        #region 2D interaction

        private IEnumerable<IMapObject> GetLineIntersections(Box box)
        {
            return Document.Map.Root.Collect(
                x => x is Root || (x.BoundingBox != null && x.BoundingBox.IntersectsWith(box)),
                x => x.Hierarchy.Parent != null && !x.Hierarchy.HasChildren && x is Solid && x.GetPolygons().Any(p => p.GetLines().Any(box.IntersectsWith))
            );
        }

        private IMapObject SelectionTest(MapViewport viewport, ViewportEvent e)
        {
            // Create a box to represent the click, with a tolerance level
            var unused = viewport.GetUnusedCoordinate(new Coordinate(100000, 100000, 100000));
            var tolerance = 4 / (decimal) viewport.Zoom; // Selection tolerance of four pixels
            var used = viewport.Expand(new Coordinate(tolerance, tolerance, 0));
            var add = used + unused;
            var click = viewport.ProperScreenToWorld(e.X, e.Y);
            var box = new Box(click - add, click + add);
            return GetLineIntersections(box).FirstOrDefault();
        }

        protected override void MouseDown(MapViewport viewport, OrthographicCamera camera, ViewportEvent e)
        {
            // Get the first element that intersects with the box, selecting or deselecting as needed
            var closestObject = SelectionTest(viewport, e) as Solid;
            SelectObject(closestObject);
        }

        #endregion

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
            objects.AddRange(base.GetSceneObjects());
            return objects;
        }

        public new void Invalidate()
        {
            Oy.Publish("VertexTool:Updated", _selection);
            base.Invalidate();
        }
    }
}

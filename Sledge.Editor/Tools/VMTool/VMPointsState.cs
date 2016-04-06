using System.Collections.Generic;
using System.Linq;
using Sledge.DataStructures.Geometric;
using Sledge.Editor.Rendering;
using Sledge.Editor.Tools.DraggableTool;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Scenes;
using Sledge.Rendering.Scenes.Elements;

namespace Sledge.Editor.Tools.VMTool
{
    class VMPointsState : BaseDraggable, IDraggableState
    {
        private VMTool _tool;

        public VMPointsState(VMTool tool)
        {
            _tool = tool;
        }

        public override void Click(MapViewport viewport, ViewportEvent e, Coordinate position)
        {

        }

        public override bool CanDrag(MapViewport viewport, ViewportEvent e, Coordinate position)
        {
            return false;
        }

        public override void Highlight(MapViewport viewport)
        {

        }

        public override void Unhighlight(MapViewport viewport)
        {

        }

        public override IEnumerable<SceneObject> GetSceneObjects()
        {
            yield break;
        }

        public override IEnumerable<Element> GetViewportElements(MapViewport viewport, PerspectiveCamera camera)
        {
            yield break;
        }

        public override IEnumerable<Element> GetViewportElements(MapViewport viewport, OrthographicCamera camera)
        {
            yield break;
        }

        public IEnumerable<IDraggable> GetDraggables()
        {
            var sub = _tool.CurrentSubTool;

            return sub == null
                ? _tool.GetVisiblePoints().OrderBy(x => x.IsSelected ? 1 : 0)
                : sub.GetDraggables();
        }
    }
}

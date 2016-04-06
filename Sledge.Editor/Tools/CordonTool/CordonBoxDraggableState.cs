using Sledge.DataStructures.Geometric;
using Sledge.Editor.Rendering;
using Sledge.Editor.Tools.DraggableTool;

namespace Sledge.Editor.Tools.CordonTool
{
    public class CordonBoxDraggableState : BoxDraggableState
    {
        public CordonBoxDraggableState(BaseDraggableTool tool) : base(tool)
        {
            // todo render the outer edges?
            // the old cordon tool used: Color.FromArgb(128, Color.Orange)
        }

        public void Update()
        {
            if (Tool.Document == null)
            {
                State.Action = BoxAction.Idle;
            }
            else
            {
                State.Start = Tool.Document.Map.CordonBounds.Start;
                State.End = Tool.Document.Map.CordonBounds.End;
                State.Action = BoxAction.Drawn;
            }
        }

        public override void Click(MapViewport viewport, ViewportEvent e, Coordinate position)
        {
            //
        }

        public override bool CanDrag(MapViewport viewport, ViewportEvent e, Coordinate position)
        {
            return false;
        }
    }
}

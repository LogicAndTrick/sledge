using Sledge.BspEditor.Primitives.MapData;
using Sledge.BspEditor.Rendering.Viewport;
using Sledge.BspEditor.Tools.Draggable;
using Sledge.DataStructures.Geometric;

namespace Sledge.BspEditor.Tools.Cordon
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
                var cordon = Tool.Document.Map.Data.GetOne<CordonBounds>() ?? new CordonBounds {Enabled = false};
                State.Start = cordon.Box.Start;
                State.End = cordon.Box.End;
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

using Sledge.EditorNew.Tools.DraggableTool;

namespace Sledge.EditorNew.Tools.SelectTool
{
    public class SelectionBoxDraggableState : BoxDraggableState
    {
        public SelectionBoxDraggableState(BaseTool tool) : base(tool)
        {
        }

        protected override void CreateBoxHandles()
        {
            BoxHandles = new[]
            {
                new BoxResizeHandle(this, ResizeHandle.TopLeft),
                new BoxResizeHandle(this, ResizeHandle.TopRight),
                new BoxResizeHandle(this, ResizeHandle.BottomLeft),
                new BoxResizeHandle(this, ResizeHandle.BottomRight),
                
                new BoxResizeHandle(this, ResizeHandle.Top),
                new BoxResizeHandle(this, ResizeHandle.Left),
                new BoxResizeHandle(this, ResizeHandle.Right),
                new BoxResizeHandle(this, ResizeHandle.Bottom),

                new InternalBoxResizeHandle(this, ResizeHandle.Center), 
            };
        }

        public override bool CanDrag(UI.Viewports.IViewport2D viewport, UI.Viewports.ViewportEvent e, DataStructures.Geometric.Coordinate position)
        {
            return false;
        }
    }
}
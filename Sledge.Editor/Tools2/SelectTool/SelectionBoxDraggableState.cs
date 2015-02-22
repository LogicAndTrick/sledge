using System.Collections.Generic;
using OpenTK;
using Sledge.DataStructures.Geometric;
using Sledge.Editor.Documents;
using Sledge.Editor.Rendering;
using Sledge.Editor.Tools2.DraggableTool;
using Sledge.Editor.Tools2.SelectTool.TransformationHandles;
using Sledge.Rendering.Cameras;

namespace Sledge.Editor.Tools2.SelectTool
{
    public class SelectionBoxDraggableState : BoxDraggableState
    {
        private List<IDraggable>[] _handles;
        private int _currentIndex;
        private RotationOrigin _rotationOrigin;

        public SelectionBoxDraggableState(BaseDraggableTool tool) : base(tool)
        {

        }

        protected override void CreateBoxHandles()
        {
            var resize = new List<IDraggable>
            {
                new ResizeTransformHandle(this, ResizeHandle.TopLeft),
                new ResizeTransformHandle(this, ResizeHandle.TopRight),
                new ResizeTransformHandle(this, ResizeHandle.BottomLeft),
                new ResizeTransformHandle(this, ResizeHandle.BottomRight),

                new ResizeTransformHandle(this, ResizeHandle.Top),
                new ResizeTransformHandle(this, ResizeHandle.Right),
                new ResizeTransformHandle(this, ResizeHandle.Bottom),
                new ResizeTransformHandle(this, ResizeHandle.Left),

                new ResizeTransformHandle(this, ResizeHandle.Center), 
            };
            _rotationOrigin = new RotationOrigin();
            var rotate = new List<IDraggable>
            {
                _rotationOrigin,

                new RotateTransformHandle(this, ResizeHandle.TopLeft, _rotationOrigin),
                new RotateTransformHandle(this, ResizeHandle.TopRight, _rotationOrigin),
                new RotateTransformHandle(this, ResizeHandle.BottomLeft, _rotationOrigin),
                new RotateTransformHandle(this, ResizeHandle.BottomRight, _rotationOrigin),

                new ResizeTransformHandle(this, ResizeHandle.Center), 
            };
            var skew = new List<IDraggable>
            {
                new SkewTransformHandle(this, ResizeHandle.Top),
                new SkewTransformHandle(this, ResizeHandle.Right),
                new SkewTransformHandle(this, ResizeHandle.Bottom),
                new SkewTransformHandle(this, ResizeHandle.Left),

                new ResizeTransformHandle(this, ResizeHandle.Center), 
            };

            _handles = new [] { resize, rotate, skew };
        }

        public override IEnumerable<IDraggable> GetDraggables()
        {
            if (State.Action == BoxAction.Idle || State.Action == BoxAction.Drawing) return new IDraggable[0];
            return _handles[_currentIndex];
        }

        public override bool CanDrag(MapViewport viewport, ViewportEvent e, Coordinate position)
        {
            return false;
        }

        public Matrix4? GetTransformationMatrix(MapViewport viewport, OrthographicCamera camera, Document document)
        {
            var tt = Tool.CurrentDraggable as ITransformationHandle;
            return tt != null ? tt.GetTransformationMatrix(viewport, camera, State, document) : null;
        }

        public void Cycle()
        {
            _currentIndex = (_currentIndex + 1) % _handles.Length;
            if (State.Start != null) _rotationOrigin.Position = new Box(State.Start, State.End).Center;
            else _rotationOrigin.Position = Coordinate.Zero;
        }
    }
}
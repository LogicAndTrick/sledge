using System.Drawing;
using System.Linq;
using System.Numerics;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Primitives.MapData;
using Sledge.BspEditor.Rendering.Viewport;
using Sledge.BspEditor.Tools.Draggable;
using Sledge.DataStructures.Geometric;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Overlay;
using Sledge.Rendering.Viewports;

namespace Sledge.BspEditor.Tools.Selection.TransformationHandles
{
    public class ResizeTransformHandle : BoxResizeHandle, ITransformationHandle
    {
        public string Name => Handle == ResizeHandle.Center ? "Move" : "Resize";

        public ResizeTransformHandle(BoxDraggableState state, ResizeHandle handle) : base(state, handle)
        {
        }

        public override bool CanDrag(MapDocument document, MapViewport viewport, OrthographicCamera camera, ViewportEvent e, Vector3 position)
        {
            if (Handle == ResizeHandle.Center)
            {
                const int padding = 2;
                var box = new Box(camera.Flatten(BoxState.Start), camera.Flatten(BoxState.End));
                var c = position;
                return c.X >= box.Start.X - padding && c.Y >= box.Start.Y - padding && c.Z >= box.Start.Z - padding
                       && c.X <= box.End.X + padding && c.Y <= box.End.Y + padding && c.Z <= box.End.Z + padding;
            }
            return base.CanDrag(document, viewport, camera, e, position);
        }

        protected override Vector3 GetResizeOrigin(MapViewport viewport, OrthographicCamera camera, Vector3 position)
        {
            if (Handle == ResizeHandle.Center)
            {
                var st = camera.Flatten(BoxState.Start);
                var ed = camera.Flatten(BoxState.End);
                var points = new[] {st, ed, new Vector3(st.X, ed.Y, 0), new Vector3(ed.X, st.Y, 0)};
                return points.OrderBy(x => (position - x).LengthSquared()).First();
            }
            return base.GetResizeOrigin(viewport, camera, position);
        }

        public override void Render(IViewport viewport, OrthographicCamera camera, Vector3 worldMin, Vector3 worldMax, I2DRenderer im)
        {
            if (Handle == ResizeHandle.Center)
            {
                if (HighlightedViewport != viewport) return;

                var start = camera.WorldToScreen(BoxState.Start);
                var end = camera.WorldToScreen(BoxState.End);

                im.AddRectFilled(start.ToVector2(), end.ToVector2(), State.FillColour);

                if (SnappedMoveOrigin != null)
                {
                    const int size = 4;
                    var orig = camera.WorldToScreen(camera.Expand(SnappedMoveOrigin.Value));

                    im.AddLine(new Vector2(orig.X - size, orig.Y - size), new Vector2(orig.X + size, orig.Y + size), Color.Yellow, 1, false);
                    im.AddLine(new Vector2(orig.X + size, orig.Y - size), new Vector2(orig.X - size, orig.Y + size), Color.Yellow, 1, false);
                }
            }
            else
            {
                base.Render(viewport, camera, worldMin, worldMax, im);
            }
        }

        public Matrix4x4? GetTransformationMatrix(MapViewport viewport, OrthographicCamera camera, BoxState state, MapDocument doc)
        {
            Matrix4x4 resizeMatrix;
            if (Handle == ResizeHandle.Center)
            {
                var movement = state.Start - state.OrigStart;
                resizeMatrix = Matrix4x4.CreateTranslation(movement.X, movement.Y, movement.Z);
            }
            else
            {
                var resize = (state.OrigStart - state.Start) + (state.End - state.OrigEnd);
                resize = Vector3.Divide(resize, state.OrigEnd - state.OrigStart);
                resize += new Vector3(1, 1, 1);
                var offset = -GetOriginForTransform(camera, state);
                var trans = Matrix4x4.CreateTranslation(offset.X, offset.Y, offset.Z);
                var scale = Matrix4x4.Multiply(trans, Matrix4x4.CreateScale(resize.X, resize.Y, resize.Z));
                var inv = Matrix4x4.Invert(trans, out var i) ? i : Matrix4x4.Identity;
                resizeMatrix = Matrix4x4.Multiply(scale, inv);
            }
            return resizeMatrix;
        }

        public TextureTransformationType GetTextureTransformationType(MapDocument doc)
        {
            var tl = doc.Map.Data.GetOne<TransformationFlags>() ?? new TransformationFlags();
            if (Handle == ResizeHandle.Center && tl.TextureLock) return TextureTransformationType.Uniform;
            if (Handle != ResizeHandle.Center && tl.TextureScaleLock) return TextureTransformationType.Scale;
            return TextureTransformationType.None;
        }

        private Vector3 GetOriginForTransform(ICamera camera, BoxState state)
        {
            float x = 0;
            float y = 0;
            var cstart = camera.Flatten(state.OrigStart);
            var cend = camera.Flatten(state.OrigEnd);
            switch (Handle)
            {
                case ResizeHandle.TopLeft:
                case ResizeHandle.Top:
                case ResizeHandle.TopRight:
                case ResizeHandle.Left:
                case ResizeHandle.Right:
                    y = cstart.Y;
                    break;
                case ResizeHandle.BottomLeft:
                case ResizeHandle.Bottom:
                case ResizeHandle.BottomRight:
                    y = cend.Y;
                    break;
            }
            switch (Handle)
            {
                case ResizeHandle.Top:
                case ResizeHandle.TopRight:
                case ResizeHandle.Right:
                case ResizeHandle.BottomRight:
                case ResizeHandle.Bottom:
                    x = cstart.X;
                    break;
                case ResizeHandle.TopLeft:
                case ResizeHandle.Left:
                case ResizeHandle.BottomLeft:
                    x = cend.X;
                    break;
            }
            return camera.Expand(new Vector3(x, y, 0));
        }
    }
}
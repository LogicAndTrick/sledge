using System;
using System.Linq;
using Sledge.DataStructures.Geometric;
using Sledge.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using System.Drawing;
using Sledge.Graphics.Helpers;

namespace Sledge.UI
{
    public class Viewport2D : ViewportBase
    {
        public enum ViewDirection
        {
            /// <summary>
            /// The XY view
            /// </summary>
            Top,

            /// <summary>
            /// The YZ view
            /// </summary>
            Front,

            /// <summary>
            /// The XZ view
            /// </summary>
            Side
        }

        private static readonly Matrix4 TopMatrix = Matrix4.Identity;
        private static readonly Matrix4 FrontMatrix = new Matrix4(Vector4.UnitZ, Vector4.UnitX, Vector4.UnitY, Vector4.UnitW);
        private static readonly Matrix4 SideMatrix = new Matrix4(Vector4.UnitX, Vector4.UnitZ, Vector4.UnitY, Vector4.UnitW);

        private static Matrix4 GetMatrixFor(ViewDirection dir)
        {
            switch (dir)
            {
                case ViewDirection.Top:
                    return TopMatrix;
                case ViewDirection.Front:
                    return FrontMatrix;
                case ViewDirection.Side:
                    return SideMatrix;
                default:
                    throw new ArgumentOutOfRangeException("dir");
            }
        }


        private static Coordinate Flatten(Coordinate c, ViewDirection direction)
        {
            switch (direction)
            {
                case ViewDirection.Top:
                    return new Coordinate(c.X, c.Y, 0);
                case ViewDirection.Front:
                    return new Coordinate(c.Y, c.Z, 0);
                case ViewDirection.Side:
                    return new Coordinate(c.X, c.Z, 0);
                default:
                    throw new ArgumentOutOfRangeException("direction");
            }
        }

        private static Coordinate Expand(Coordinate c, ViewDirection direction)
        {
            switch (direction)
            {
                case ViewDirection.Top:
                    return new Coordinate(c.X, c.Y, 0);
                case ViewDirection.Front:
                    return new Coordinate(0, c.X, c.Y);
                case ViewDirection.Side:
                    return new Coordinate(c.X, 0, c.Y);
                default:
                    throw new ArgumentOutOfRangeException("direction");
            }
        }

        private static Coordinate GetUnusedCoordinate(Coordinate c, ViewDirection direction)
        {
            switch (direction)
            {
                case ViewDirection.Top:
                    return new Coordinate(0, 0, c.Z);
                case ViewDirection.Front:
                    return new Coordinate(c.X, 0, 0);
                case ViewDirection.Side:
                    return new Coordinate(0, c.Y, 0);
                default:
                    throw new ArgumentOutOfRangeException("direction");
            }
        }

        private static Coordinate ZeroUnusedCoordinate(Coordinate c, ViewDirection direction)
        {
            switch (direction)
            {
                case ViewDirection.Top:
                    return new Coordinate(c.X, c.Y, 0);
                case ViewDirection.Front:
                    return new Coordinate(0, c.Y, c.Z);
                case ViewDirection.Side:
                    return new Coordinate(c.X, 0, c.Z);
                default:
                    throw new ArgumentOutOfRangeException("direction");
            }
        }

        public ViewDirection Direction { get; private set; }

        private Coordinate _position;
        public Coordinate Position
        {
            get { return _position; }
            set
            {
                var old = _position;
                _position = value;
                Listeners.OfType<IViewport2DEventListener>()
                    .ToList().ForEach(l => l.PositionChanged(old, _position));
            }
        }

        private decimal _zoom;
        public decimal Zoom
        {
            get { return _zoom; }
            set
            {
                var old = _zoom;
                _zoom = value;
                Listeners.OfType<IViewport2DEventListener>()
                    .ToList().ForEach(l => l.ZoomChanged(old, _zoom));
            }
        }

        private Coordinate CenterScreen { get; set; }

        public Viewport2D(ViewDirection direction)
        {
            Zoom = 1;
            Position = new Coordinate(0, 0, 0);
            Direction = direction;
            CenterScreen = new Coordinate(Width / 2m, Height / 2m, 0);
        }
        
        public Viewport2D(ViewDirection direction, RenderContext context) : base(context)
        {
            Zoom = 1;
            Position = new Coordinate(0, 0, 0);
            Direction = direction;
            CenterScreen = new Coordinate(Width / 2m, Height / 2m, 0);
        }

        public Coordinate Flatten(Coordinate c)
        {
            return Flatten(c, Direction);
        }

        public Coordinate Expand(Coordinate c)
        {
            return Expand(c, Direction);
        }

        public Coordinate GetUnusedCoordinate(Coordinate c)
        {
            return GetUnusedCoordinate(c, Direction);
        }

        public Coordinate ZeroUnusedCoordinate(Coordinate c)
        {
            return ZeroUnusedCoordinate(c, Direction);
        }

        public override void SetViewport()
        {
            base.SetViewport();
            Viewport.Orthographic(0, 0, Width, Height, -50000, 50000);
        }

        public override Matrix4 GetViewportMatrix()
        {
            const float near = -1000000;
            const float far = 1000000;
            return Matrix4.CreateOrthographic(Width, Height, near, far);
        }

        public override Matrix4 GetCameraMatrix()
        {
            var translate = Matrix4.CreateTranslation((float)-Position.X, (float)-Position.Y, 0);
            var scale = Matrix4.Scale(new Vector3((float)Zoom, (float)Zoom, 0));
            return translate * scale;
        }

        public override Matrix4 GetModelViewMatrix()
        {
            return GetMatrixFor(Direction);
        }

        protected override void OnResize(EventArgs e)
        {
            CenterScreen = new Coordinate(Width / 2m, Height / 2m, 0);
            base.OnResize(e);
        }

        public Coordinate ScreenToWorld(Point location)
        {
            return ScreenToWorld(location.X, location.Y);
        }

        public Coordinate ScreenToWorld(decimal x, decimal y)
        {
            return ScreenToWorld(new Coordinate(x, y, 0));
        }

        public Coordinate ScreenToWorld(Coordinate location)
        {
            return Position + ((location - CenterScreen) / Zoom);
        }

        public Coordinate WorldToScreen(Coordinate location)
        {
            return CenterScreen + ((location - Position) * Zoom);
        }

        public decimal UnitsToPixels(decimal units)
        {
            return units * Zoom;
        }

        public decimal PixelsToUnits(decimal pixels)
        {
            return pixels / Zoom;
        }

        protected override void UpdateBeforeRender()
        {
            GL.Scale(new Vector3((float) Zoom, (float) Zoom, 0));
            GL.Translate((float)-Position.X, (float)-Position.Y, 0);
            base.UpdateBeforeRender();
        }

        protected override void UpdateAfterRender()
        {
            Listeners.ForEach(x => x.Render2D());
            base.UpdateAfterRender();
        }
    }
}

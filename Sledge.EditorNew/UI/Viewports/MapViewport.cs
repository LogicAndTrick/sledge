using System;
using System.Collections.Generic;
using System.Diagnostics;
using OpenTK;
using OpenTK.Graphics;
using Sledge.DataStructures.Geometric;
using Sledge.Graphics;
using Sledge.Gui.Controls;
using Sledge.Gui.Events;
using Sledge.Gui.Structures;
using Matrix = Sledge.Graphics.Helpers.Matrix;
using MatrixMode = OpenTK.Graphics.OpenGL.MatrixMode;

namespace Sledge.EditorNew.UI.Viewports
{
    public class MapViewport : Viewport, IViewport2D, IViewport3D
    {
        public RenderContext RenderContext { get; set; }
        public List<IViewportEventListener> Listeners { get; set; }

        #region Input Locking

        private object _inputLock;

        public bool IsUnlocked(object context)
        {
            return _inputLock == null || _inputLock == context;
        }

        public bool AquireInputLock(object context)
        {
            if (_inputLock == null) _inputLock = context;
            return _inputLock == context;
        }

        public bool ReleaseInputLock(object context)
        {
            if (_inputLock == context) _inputLock = null;
            return _inputLock == null;
        }

        #endregion
        
        public MapViewport(ViewDirection direction, RenderContext context = null) : this(context)
        {
            Is3D = false;
            Direction = direction;
            Zoom = 1;
            Position = new Coordinate(0, 0, 0);
        }

        public MapViewport(ViewType type, RenderContext context = null) : this(context)
        {
            Is3D = true;
            Type = type;
        }

        protected MapViewport(RenderContext context)
        {
            RenderContext = context ?? new RenderContext();
            Camera = new Camera();
            Listeners = new List<IViewportEventListener>();

            ActualSizeChanged += OnResize;
            Render += OnRender;

            MouseWheel += OnMouseWheel;
            MouseEnter += OnMouseEnter;
            MouseLeave += OnMouseLeave;
            MouseMove += OnMouseMove;
            MouseUp += OnMouseUp;
            MouseDown += OnMouseDown;
            MouseDoubleClick += OnMouseDoubleClick;

            // todo key events
        }

        public override void Dispose()
        {
            RenderContext.Dispose();
            base.Dispose();
        }

        #region Standard Wireup

        private void OnResize(object sender, EventArgs e)
        {
            if (!Is3D)
            {
                CenterScreen = new Coordinate(ActualSize.Width / 2m, ActualSize.Height / 2m, 0);
            }
        }

        private void OnRender(object sender, Frame frame)
        {
            SetCamera();
            ListenerDo(x => x.PreRender());
            RenderContext.Render(this);
            ListenerDo(x => x.PostRender());
            if (Is3D)
            {
                Listeners.ForEach(x => x.Render3D());
                Matrix.Set(MatrixMode.Modelview);
                Matrix.Identity();
                Graphics.Helpers.Viewport.Orthographic(0, 0, ActualSize.Width, ActualSize.Height);
            }
            Listeners.ForEach(x => x.Render2D());
        }

        private void SetCamera()
        {
            if (Is3D)
            {
                var fov = Camera == null ? 60 : Camera.FOV;
                var clip = Camera == null ? 6000 : Camera.ClipDistance;
                Graphics.Helpers.Viewport.Perspective(0, 0, ActualSize.Width, ActualSize.Height, fov, 0.1f, clip);
                if (Camera != null) Camera.Position();
            }
            else
            {
                Graphics.Helpers.Viewport.Orthographic(0, 0, ActualSize.Width, ActualSize.Height, -50000, 50000);
            }
        }

        #endregion

        #region Listeners

        public event ListenerExceptionEventHandler ListenerException;

        private void OnListenerException(Exception ex)
        {
            if (ListenerException != null)
            {
                var st = new StackTrace();
                var frames = st.GetFrames() ?? new StackFrame[0];
                var msg = "Listener exception: " + ex.Message;
                foreach (var frame in frames)
                {
                    var method = frame.GetMethod();
                    msg += "\r\n    " + method.ReflectedType.FullName + "." + method.Name;
                }
                ListenerException(this, new Exception(msg, ex));
            }
        }

        private void ListenerDo(Action<IViewportEventListener> action)
        {
            foreach (var listener in Listeners)
            {
                try
                {
                    action(listener);
                }
                catch (Exception ex)
                {
                    OnListenerException(ex);
                }
            }
        }

        private void ListenerDoEvent(ViewportEvent e, Action<IViewportEventListener, ViewportEvent> action)
        {
            foreach (var listener in Listeners)
            {
                try
                {
                    action(listener, e);
                }
                catch (Exception ex)
                {
                    OnListenerException(ex);
                }
                if (e.Handled)
                {
                    break;
                }
            }
        }


        private void OnMouseWheel(object sender, IMouseEvent e)
        {
            ListenerDoEvent(new ViewportEvent(this, e), (l, v) => l.MouseWheel(v));
        }

        private void OnMouseEnter(object sender, EventArgs e)
        {
            ListenerDoEvent(new ViewportEvent(this, e), (l, v) => l.MouseEnter(v));
        }

        private void OnMouseLeave(object sender, EventArgs e)
        {
            ListenerDoEvent(new ViewportEvent(this, e), (l, v) => l.MouseLeave(v));
        }

        private bool _dragging = false;
        private Point _mouseDragLocation = new Point(-1, -1);
        private Point _mouseDownLocation = new Point(-1, -1);

        private void OnMouseMove(object sender, IMouseEvent e)
        {
            var ve = new ViewportEvent(this, e)
            {
                Dragging = _dragging,
                StartX = _mouseDownLocation.X,
                StartY = _mouseDownLocation.Y,
                LastX = _mouseDragLocation.X,
                LastY = _mouseDragLocation.Y,
            };
            if (!_dragging
                && (Math.Abs(_mouseDownLocation.X - e.Location.X) > 1
                    || Math.Abs(_mouseDownLocation.Y - e.Location.Y) > 1))
            {
                _dragging = ve.Dragging = true;
                ListenerDoEvent(ve, (l, v) => l.DragStart(v));
            }
            ListenerDoEvent(ve, (l, v) => l.MouseMove(v));
            if (_dragging)
            {
                ListenerDoEvent(ve, (l, v) => l.DragMove(v));
            }
        }

        private void OnMouseUp(object sender, IMouseEvent e)
        {
            var ve = new ViewportEvent(this, e)
            {
                Dragging = _dragging,
                StartX = _mouseDownLocation.X,
                StartY = _mouseDownLocation.Y,
                LastX = _mouseDragLocation.X,
                LastY = _mouseDragLocation.Y,
            };
            if (_dragging)
            {
                ListenerDoEvent(ve, (l, v) => l.DragEnd(v));
            }
            ListenerDoEvent(ve, (l, v) => l.MouseUp(v));
            if (!_dragging
                && Math.Abs(_mouseDownLocation.X - e.Location.X) <= 1
                && Math.Abs(_mouseDownLocation.Y - e.Location.Y) <= 1)
            {
                // Mouse hasn't moved very much, trigger the click event
                ListenerDoEvent(ve, (l, v) => l.MouseClick(v));
            }
            _mouseDownLocation = new Point(-1, -1);
            _mouseDragLocation = new Point(-1, -1);
        }

        private void OnMouseDown(object sender, IMouseEvent e)
        {
            _mouseDownLocation = new Point(e.X, e.Y);
            _mouseDragLocation = new Point(e.X, e.Y);
            _dragging = false;
            ListenerDoEvent(new ViewportEvent(this), (l, v) => l.MouseDown(v));
        }

        private void OnMouseDoubleClick(object sender, EventArgs e)
        {
            ListenerDoEvent(new ViewportEvent(this, e), (l, v) => l.MouseDoubleClick(v));
        }
        
        #endregion

        #region Shared Methods

        public bool Is3D { get; set; }

        public bool Is2D
        {
            get { return !Is3D; }
            set { Is3D = !value; }
        }

        public int Width { get { return ActualSize.Width; } }
        public int Height { get { return ActualSize.Height; } }

        public void FocusOn(Box box)
        {
            if (Is3D)
            {

                var dist = System.Math.Max(System.Math.Max(box.Width, box.Length), box.Height);
                var normal = Camera.Location - Camera.LookAt;
                var v = new Vector(new Coordinate((decimal) normal.X, (decimal) normal.Y, (decimal) normal.Z), dist);
                FocusOn(box.Center, new Coordinate(v.X, v.Y, v.Z));
            }
            else
            {
                FocusOn(box.Center);
            }
        }

        public void FocusOn(Coordinate coordinate)
        {
            if (Is3D)
            {
                FocusOn(coordinate, Coordinate.UnitY * -100);
            }
            else
            {
                Position = Flatten(coordinate);
            }
        }

        public Matrix4 GetViewportMatrix()
        {
            if (Is3D)
            {
                const float near = 0.1f;
                var ratio = ActualSize.Width / (float) ActualSize.Height;
                if (ratio <= 0) ratio = 1;
                return Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(Camera.FOV), ratio, near, Camera.ClipDistance);
            }
            else
            {
                const float near = -1000000;
                const float far = 1000000;
                return Matrix4.CreateOrthographic(ActualSize.Width, ActualSize.Height, near, far);
            }
        }

        public Matrix4 GetCameraMatrix()
        {
            if (Is3D)
            {
                return Matrix4.LookAt(Camera.Location, Camera.LookAt, Vector3.UnitZ);
            }
            else
            {
                var translate = Matrix4.CreateTranslation((float) -Position.X, (float) -Position.Y, 0);
                var scale = Matrix4.Scale(new Vector3((float) Zoom, (float) Zoom, 0));
                return translate * scale;
            }
        }

        public Matrix4 GetModelViewMatrix()
        {
            if (Is3D)
            {
                return Matrix4.Identity;
            }
            else
            {
                switch (Direction)
                {
                    case ViewDirection.Top:
                        return TopMatrix;
                    case ViewDirection.Front:
                        return FrontMatrix;
                    case ViewDirection.Side:
                        return SideMatrix;
                    default:
                        throw new ArgumentOutOfRangeException("Direction");
                }
            }
        }

        /// <summary>
        /// Convert a screen space coordinate into a world space coordinate.
        /// The resulting coordinate will be quite a long way from the camera.
        /// </summary>
        /// <param name="screen">The screen coordinate (with Y in OpenGL space)</param>
        /// <returns>The world coordinate</returns>
        public Coordinate ScreenToWorld(Coordinate screen)
        {
            if (Is3D)
            {
                screen = new Coordinate(screen.X, screen.Y, 1);
                var viewport = new[] {0, 0, ActualSize.Width, ActualSize.Height};
                var pm = Matrix4d.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(Camera.FOV), ActualSize.Width / (float) ActualSize.Height, 0.1f, 50000);
                var vm = Matrix4d.LookAt(
                    new Vector3d(Camera.Location.X, Camera.Location.Y, Camera.Location.Z),
                    new Vector3d(Camera.LookAt.X, Camera.LookAt.Y, Camera.LookAt.Z),
                    Vector3d.UnitZ);
                return MathFunctions.Unproject(screen, viewport, pm, vm);
            }
            else
            {
                return Position + ((screen - CenterScreen) / Zoom);
            }
        }

        /// <summary>
        /// Convert a world space coordinate into a screen space coordinate.
        /// </summary>
        /// <param name="world">The world coordinate</param>
        /// <returns>The screen coordinate</returns>
        public Coordinate WorldToScreen(Coordinate world)
        {
            if (Is3D)
            {
                var viewport = new[] {0, 0, ActualSize.Width, ActualSize.Height};
                var pm = Matrix4d.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(Camera.FOV), ActualSize.Width / (float) ActualSize.Height, 0.1f, 50000);
                var vm = Matrix4d.LookAt(
                    new Vector3d(Camera.Location.X, Camera.Location.Y, Camera.Location.Z),
                    new Vector3d(Camera.LookAt.X, Camera.LookAt.Y, Camera.LookAt.Z),
                    Vector3d.UnitZ);
                return MathFunctions.Project(world, viewport, pm, vm);
            }
            else
            {
                return CenterScreen + ((world - Position) * Zoom);
            }
        }

        #endregion

        #region 2D Methods

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

        public ViewDirection Direction { get; set; }
        private Coordinate CenterScreen { get; set; }

        private Coordinate _position;
        public Coordinate Position
        {
            get { return _position; }
            set
            {
                var old = _position;
                _position = value;
                ListenerDo(x => x.PositionChanged(new ViewportEvent(this) { CameraPosition = _position }));
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
                ListenerDo(x => x.ZoomChanged(new ViewportEvent(this) { CameraZoom = _zoom }));
            }
        }

        public Coordinate Flatten(Coordinate c)
        {
            switch (Direction)
            {
                case ViewDirection.Top:
                    return new Coordinate(c.X, c.Y, 0);
                case ViewDirection.Front:
                    return new Coordinate(c.Y, c.Z, 0);
                case ViewDirection.Side:
                    return new Coordinate(c.X, c.Z, 0);
                default:
                    throw new ArgumentOutOfRangeException("Direction");
            }
        }

        public Coordinate Expand(Coordinate c)
        {
            switch (Direction)
            {
                case ViewDirection.Top:
                    return new Coordinate(c.X, c.Y, 0);
                case ViewDirection.Front:
                    return new Coordinate(0, c.X, c.Y);
                case ViewDirection.Side:
                    return new Coordinate(c.X, 0, c.Y);
                default:
                    throw new ArgumentOutOfRangeException("Direction");
            }
        }

        public Coordinate GetUnusedCoordinate(Coordinate c)
        {
            switch (Direction)
            {
                case ViewDirection.Top:
                    return new Coordinate(0, 0, c.Z);
                case ViewDirection.Front:
                    return new Coordinate(c.X, 0, 0);
                case ViewDirection.Side:
                    return new Coordinate(0, c.Y, 0);
                default:
                    throw new ArgumentOutOfRangeException("Direction");
            }
        }

        public Coordinate ZeroUnusedCoordinate(Coordinate c)
        {
            switch (Direction)
            {
                case ViewDirection.Top:
                    return new Coordinate(c.X, c.Y, 0);
                case ViewDirection.Front:
                    return new Coordinate(0, c.Y, c.Z);
                case ViewDirection.Side:
                    return new Coordinate(c.X, 0, c.Z);
                default:
                    throw new ArgumentOutOfRangeException("Direction");
            }
        }

        public decimal UnitsToPixels(decimal units)
        {
            return units * Zoom;
        }

        public decimal PixelsToUnits(decimal pixels)
        {
            return pixels / Zoom;
        }

        #endregion

        #region 3D Methods

        public Camera Camera { get; set; }
        public ViewType Type { get; set; }

        public void FocusOn(Coordinate coordinate, Coordinate distance)
        {
            var pos = coordinate + distance;
            Camera.Location = new Vector3((float)pos.X, (float)pos.Y, (float)pos.Z);
            Camera.LookAt = new Vector3((float)coordinate.X, (float)coordinate.Y, (float)coordinate.Z);
        }

        /// <summary>
        /// Project the 2D coordinates from the screen coordinates outwards
        /// from the camera along the lookat vector, taking the frustrum
        /// into account. The resulting line will be run from the camera
        /// position along the view axis and end at the back clipping pane.
        /// </summary>
        /// <param name="x">The X coordinate on screen</param>
        /// <param name="y">The Y coordinate on screen</param>
        /// <returns>A line beginning at the camera location and tracing
        /// along the 3D projection for at least 1,000,000 units.</returns>
        public Line CastRayFromScreen(int x, int y)
        {
            var near = new Coordinate(x, ActualSize.Height - y, 0);
            var far = new Coordinate(x, ActualSize.Height - y, 1);
            var pm = Matrix4d.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(Camera.FOV), ActualSize.Width / (float) ActualSize.Height, 0.1f, 50000);
            var vm = Matrix4d.LookAt(
                new Vector3d(Camera.Location.X, Camera.Location.Y, Camera.Location.Z),
                new Vector3d(Camera.LookAt.X, Camera.LookAt.Y, Camera.LookAt.Z),
                Vector3d.UnitZ);
            var viewport = new[] { 0, 0, ActualSize.Width, ActualSize.Height };
            var un = MathFunctions.Unproject(near, viewport, pm, vm);
            var uf = MathFunctions.Unproject(far, viewport, pm, vm);
            return (un == null || uf == null) ? null : new Line(un, uf);
        }

        #endregion
    }
}
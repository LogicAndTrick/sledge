using OpenTK;
using OpenTK.Graphics.OpenGL;
using Sledge.DataStructures.Geometric;
using Sledge.Graphics;
using Sledge.Gui.Interfaces.Viewports;

namespace Sledge.Gui.Gtk.Viewports
{
    public class Viewport : ViewportBase
    {
        #region 3D Methods

        public Camera Camera { get; set; }
        public ViewType Type { get; set; }

        public Viewport(ViewType type)
        {
            Is3D = true;
            Type = type;
            Camera = new Camera();
        }

        public override void FocusOn(Box box)
        {
            var dist = System.Math.Max(System.Math.Max(box.Width, box.Length), box.Height);
            var normal = Camera.Location - Camera.LookAt;
            var v = new Vector(new Coordinate((decimal)normal.X, (decimal)normal.Y, (decimal)normal.Z), dist);
            FocusOn(box.Center, new Coordinate(v.X, v.Y, v.Z));
        }

        public override void FocusOn(Coordinate coordinate)
        {
            FocusOn(coordinate, Coordinate.UnitY * -100);
        }

        public void FocusOn(Coordinate coordinate, Coordinate distance)
        {
            var pos = coordinate + distance;
            Camera.Location = new Vector3((float)pos.X, (float)pos.Y, (float)pos.Z);
            Camera.LookAt = new Vector3((float)coordinate.X, (float)coordinate.Y, (float)coordinate.Z);
        }

        public override Matrix4 GetViewportMatrix()
        {
            const float near = 0.1f;
            var ratio = Width / (float)Height;
            if (ratio <= 0) ratio = 1;
            return Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(Camera.FOV), ratio, near, Camera.ClipDistance);
        }

        public override Matrix4 GetCameraMatrix()
        {
             return Matrix4.LookAt(Camera.Location, Camera.LookAt, Vector3.UnitZ);
        }

        public override void SetViewport()
        {
            base.SetViewport();
            var fov = Camera == null ? 60 : Camera.FOV;
            var clip = Camera == null ? 6000 : Camera.ClipDistance;
            Sledge.Graphics.Helpers.Viewport.Perspective(0, 0, Width, Height, fov, 0.1f, clip);
        }

        protected override void UpdateBeforeClearViewport()
        {
            Camera.Position();
            base.UpdateBeforeClearViewport();
        }

        protected override void UpdateAfterRender()
        {
            base.UpdateAfterRender();
            Listeners.ForEach(x => x.Render3D());

            Sledge.Graphics.Helpers.Matrix.Set(MatrixMode.Modelview);
            Sledge.Graphics.Helpers.Matrix.Identity();
            Sledge.Graphics.Helpers.Viewport.Orthographic(0, 0, Width, Height);
            Listeners.ForEach(x => x.Render2D());
        }

        /// <summary>
        /// Convert a screen space coordinate into a world space coordinate.
        /// The resulting coordinate will be quite a long way from the camera.
        /// </summary>
        /// <param name="screen">The screen coordinate (with Y in OpenGL space)</param>
        /// <returns>The world coordinate</returns>
        public override Coordinate ScreenToWorld(Coordinate screen)
        {
            screen = new Coordinate(screen.X, screen.Y, 1);
            var viewport = new[] { 0, 0, Width, Height };
            var pm = Matrix4d.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(Camera.FOV), Width / (float)Height, 0.1f, 50000);
            var vm = Matrix4d.LookAt(
                new Vector3d(Camera.Location.X, Camera.Location.Y, Camera.Location.Z),
                new Vector3d(Camera.LookAt.X, Camera.LookAt.Y, Camera.LookAt.Z),
                Vector3d.UnitZ);
            return MathFunctions.Unproject(screen, viewport, pm, vm);
        }

        /// <summary>
        /// Convert a world space coordinate into a screen space coordinate.
        /// </summary>
        /// <param name="world">The world coordinate</param>
        /// <returns>The screen coordinate</returns>
        public override Coordinate WorldToScreen(Coordinate world)
        {
            var viewport = new[] { 0, 0, Width, Height };
            var pm = Matrix4d.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(Camera.FOV), Width / (float)Height, 0.1f, 50000);
            var vm = Matrix4d.LookAt(
                new Vector3d(Camera.Location.X, Camera.Location.Y, Camera.Location.Z),
                new Vector3d(Camera.LookAt.X, Camera.LookAt.Y, Camera.LookAt.Z),
                Vector3d.UnitZ);
            return MathFunctions.Project(world, viewport, pm, vm);
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
            var near = new Coordinate(x, Height - y, 0);
            var far = new Coordinate(x, Height - y, 1);
            var pm = Matrix4d.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(Camera.FOV), Width / (float)Height, 0.1f, 50000);
            var vm = Matrix4d.LookAt(
                new Vector3d(Camera.Location.X, Camera.Location.Y, Camera.Location.Z),
                new Vector3d(Camera.LookAt.X, Camera.LookAt.Y, Camera.LookAt.Z),
                Vector3d.UnitZ);
            var viewport = new[] { 0, 0, Width, Height };
            var un = MathFunctions.Unproject(near, viewport, pm, vm);
            var uf = MathFunctions.Unproject(far, viewport, pm, vm);
            return (un == null || uf == null) ? null : new Line(un, uf);
        }
        #endregion
    }
}

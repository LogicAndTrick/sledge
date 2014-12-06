using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Sledge.DataStructures.Geometric;
using Sledge.EditorNew.Tools.DraggableTool;
using Sledge.EditorNew.UI;
using Sledge.EditorNew.UI.Viewports;
using Sledge.Graphics;
using Camera = Sledge.DataStructures.MapObjects.Camera;

namespace Sledge.EditorNew.Tools.CameraTool
{
    public interface ICameraDraggable : IDraggable
    {
        Camera Camera { get; }
    }

    public class DraggableCameraEye : DraggableCoordinate, ICameraDraggable
    {
        public Camera Camera { get; set; }

        public DraggableCameraEye(Camera camera)
        {
            Camera = camera;
            Position = camera.EyePosition.Clone();
        }

        public override bool CanDrag(IViewport2D viewport, ViewportEvent e, Coordinate position)
        {
            var pos = viewport.Flatten(Camera.EyePosition);
            var diff = (pos - position).Absolute();
            return diff.X < 5 && diff.Y < 5;
        }

        public override void Drag(IViewport2D viewport, ViewportEvent e, Coordinate lastPosition, Coordinate position)
        {
            var pos = viewport.Expand(position) + viewport.GetUnusedCoordinate(Camera.EyePosition);
            if (Input.Ctrl)
            {
                var offset = Camera.EyePosition - Camera.LookPosition;
                Camera.LookPosition.X = pos.X - offset.X;
                Camera.LookPosition.Y = pos.Y - offset.Y;
                Camera.LookPosition.Z = pos.Z - offset.Z;
            }
            Camera.EyePosition.X = pos.X;
            Camera.EyePosition.Y = pos.Y;
            Camera.EyePosition.Z = pos.Z;
            base.Drag(viewport, e, lastPosition, position);
        }

        public override void Render(IViewport2D viewport)
        {
            var p1 = viewport.Flatten(Camera.EyePosition);

            // Position circle
            GL.Begin(PrimitiveType.Polygon);
            GL.Color3(Highlighted ? Color.DarkOrange : Color.LawnGreen);
            GLX.Circle(new Vector2d(p1.DX, p1.DY), 4, (double) viewport.Zoom, loop: true);
            GL.End();
        }
    }

    public class DraggableCameraLook : DraggableCoordinate, ICameraDraggable
    {
        public Camera Camera { get; set; }

        public DraggableCameraLook(Camera camera)
        {
            Camera = camera;
            Position = camera.LookPosition.Clone();
        }

        public override bool CanDrag(IViewport2D viewport, ViewportEvent e, Coordinate position)
        {
            var pos = viewport.Flatten(Camera.LookPosition);
            var diff = (pos - position).Absolute();
            return diff.X < 5 && diff.Y < 5;
        }

        public override void Drag(IViewport2D viewport, ViewportEvent e, Coordinate lastPosition, Coordinate position)
        {
            var pos = viewport.Expand(position) + viewport.GetUnusedCoordinate(Camera.LookPosition);
            if (Input.Ctrl)
            {
                var offset = Camera.EyePosition - Camera.LookPosition;
                Camera.EyePosition.X = pos.X + offset.X;
                Camera.EyePosition.Y = pos.Y + offset.Y;
                Camera.EyePosition.Z = pos.Z + offset.Z;
            }
            Camera.LookPosition.X = pos.X;
            Camera.LookPosition.Y = pos.Y;
            Camera.LookPosition.Z = pos.Z;
            base.Drag(viewport, e, lastPosition, position);
        }

        private static void Coord(Coordinate c)
        {
            GL.Vertex3(c.DX, c.DY, c.DZ);
        }

        public override void Render(IViewport2D viewport)
        {
            GL.Color3(Highlighted ? Color.Cyan : Color.Red);

            GL.Enable(EnableCap.LineSmooth);
            GL.Enable(EnableCap.PolygonSmooth);

            GL.Hint(HintTarget.LineSmoothHint, HintMode.Nicest);
            GL.Hint(HintTarget.PolygonSmoothHint, HintMode.Nicest);

            var p1 = viewport.Flatten(Camera.EyePosition);
            var p2 = viewport.Flatten(Camera.LookPosition);

            // Line between position and look
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex2(p1.DX, p1.DY);
            GL.Vertex2(p2.DX, p2.DY);
            GL.Vertex2(p2.DX, p2.DY);
            GL.Vertex2(p1.DX, p1.DY);
            GL.End();

            // Direction Triangle
            var multiplier = 4 / viewport.Zoom;
            var dir = (p2 - p1).Normalise();
            var cp = new Coordinate(-dir.Y, dir.X, 0).Normalise();

            GL.Begin(PrimitiveType.Triangles);
            Coord(p2 - (dir - cp) * multiplier);
            Coord(p2 - (dir + cp) * multiplier);
            Coord(p2 + dir * 1.5m * multiplier);
            GL.End();

            GL.Disable(EnableCap.LineSmooth);
            GL.Disable(EnableCap.PolygonSmooth);
        }
    }
}
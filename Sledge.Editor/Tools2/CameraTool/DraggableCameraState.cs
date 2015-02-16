using System;
using System.Collections.Generic;
using System.Linq;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;
using Sledge.EditorNew.Documents;
using Sledge.EditorNew.Tools.DraggableTool;
using Sledge.EditorNew.UI.Viewports;

namespace Sledge.EditorNew.Tools.CameraTool
{
    public class DraggableCameraState : IDraggableState
    {
        public CameraTool CameraTool { get; set; }
        public Document Document { get { return CameraTool.Document; } }

        private List<ICameraDraggable> _draggables;

        public DraggableCameraState(CameraTool cameraTool)
        {
            CameraTool = cameraTool;
            _draggables = new List<ICameraDraggable>();
        }

        public void UpdateDraggables()
        {
            _draggables.Clear();
            foreach (var cam in GetCameras())
            {
                var look = new DraggableCameraLook(cam);
                var eye = new DraggableCameraEye(cam);
                _draggables.Add(look);
                _draggables.Add(eye);
            }
        }

        private Tuple<Coordinate, Coordinate> GetViewportCamera()
        {
            var cam = ViewportManager.Viewports.Where(x => x.Is3D).OfType<IViewport3D>().Select(x => x.Camera).FirstOrDefault();
            if (cam == null) return null;

            var pos = new Coordinate((decimal)cam.Location.X, (decimal)cam.Location.Y, (decimal)cam.Location.Z);
            var look = new Coordinate((decimal)cam.LookAt.X, (decimal)cam.LookAt.Y, (decimal)cam.LookAt.Z);

            var dir = (look - pos).Normalise() * 20;
            return Tuple.Create(pos, pos + dir);
        }

        private IEnumerable<Camera> GetCameras()
        {
            var c = GetViewportCamera();
            if (!Document.Map.Cameras.Any())
            {
                Document.Map.Cameras.Add(new Camera { EyePosition = c.Item1, LookPosition = c.Item2 });
            }
            if (Document.Map.ActiveCamera == null || !Document.Map.Cameras.Contains(Document.Map.ActiveCamera))
            {
                Document.Map.ActiveCamera = Document.Map.Cameras.First();
            }
            var len = Document.Map.ActiveCamera.Length;
            Document.Map.ActiveCamera.EyePosition = c.Item1;
            Document.Map.ActiveCamera.LookPosition = c.Item1 + (c.Item2 - c.Item1).Normalise() * len;
            foreach (var camera in Document.Map.Cameras)
            {
                var dir = camera.LookPosition - camera.EyePosition;
                camera.LookPosition = camera.EyePosition + dir.Normalise() * Math.Max(Document.Map.GridSpacing * 1.5m, dir.VectorMagnitude());
                yield return camera;
            }
        }

        public IEnumerable<IDraggable> GetDraggables(IViewport2D viewport)
        {
            return _draggables;
        }

        public void Click(IViewport2D viewport, ViewportEvent e, Coordinate position)
        {

        }

        public bool CanDrag(IViewport2D viewport, ViewportEvent e, Coordinate position)
        {
            return true;
        }

        public void Highlight(IViewport2D viewport)
        {

        }

        public void Unhighlight(IViewport2D viewport)
        {

        }

        public void StartDrag(IViewport2D viewport, ViewportEvent e, Coordinate position)
        {

        }

        public void Drag(IViewport2D viewport, ViewportEvent e, Coordinate lastPosition, Coordinate position)
        {

        }

        public void EndDrag(IViewport2D viewport, ViewportEvent e, Coordinate position)
        {

        }

        public void Render(IViewport2D viewport)
        {

        }
    }
}
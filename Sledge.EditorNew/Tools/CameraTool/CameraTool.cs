using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using OpenTK;
using Sledge.Common.Mediator;
using Sledge.DataStructures.Geometric;
using Sledge.EditorNew.Properties;
using Sledge.EditorNew.Tools.DraggableTool;
using Sledge.EditorNew.UI.Viewports;
using Sledge.Settings;

namespace Sledge.EditorNew.Tools.CameraTool
{
    public class CameraTool : BaseDraggableTool
    {
        private DraggableCameraState _state;
        public CameraTool()
        {
            _state = new DraggableCameraState(this);
            States.Add(_state);
        }

        public override IEnumerable<string> GetContexts()
        {
            yield return "Camera Tool";
        }

        public override Image GetIcon()
        {
            return Resources.Tool_Camera;
        }

        public override string GetName()
        {
            return "CameraTool";
        }

        public override HotkeyTool? GetHotkeyToolType()
        {
            return HotkeyTool.Camera;
        }

        public override HotkeyInterceptResult InterceptHotkey(HotkeysMediator hotkeyMessage, object parameters)
        {
            if (hotkeyMessage == HotkeysMediator.OperationsDelete)
            {
                CameraDelete();
                return HotkeyInterceptResult.Abort;
            }
            return HotkeyInterceptResult.Continue;
        }

        public override void ToolSelected(bool preventHistory)
        {
            Mediator.Subscribe(HotkeysMediator.CameraNext, this);
            Mediator.Subscribe(HotkeysMediator.CameraPrevious, this);

            _state.UpdateDraggables();
        }

        private bool _currentlyDragging;

        protected override void OnDraggableDragStarted(IViewport2D viewport, ViewportEvent e, Coordinate position, IDraggable draggable)
        {
            _currentlyDragging = true;
        }

        protected override void OnDraggableDragMoved(IViewport2D viewport, ViewportEvent e, Coordinate previousPosition, Coordinate position, IDraggable draggable)
        {
            var cameraDraggable = draggable as ICameraDraggable;
            if (cameraDraggable != null)
            {
                var cam = cameraDraggable.Camera;
                SetViewportCamera(cam.EyePosition, cam.LookPosition);
            }
        }

        protected override void OnDraggableDragEnded(IViewport2D viewport, ViewportEvent e, Coordinate position, IDraggable draggable)
        {
            _currentlyDragging = false;
        }

        private void CameraNext()
        {
            if (_currentlyDragging || Document.Map.Cameras.Count < 2) return;
            var idx = Document.Map.Cameras.IndexOf(Document.Map.ActiveCamera);
            idx = (idx + 1) % Document.Map.Cameras.Count;
            Document.Map.ActiveCamera = Document.Map.Cameras[idx];
            SetViewportCamera(Document.Map.ActiveCamera.EyePosition, Document.Map.ActiveCamera.LookPosition);
        }

        private void CameraPrevious()
        {
            if (_currentlyDragging || Document.Map.Cameras.Count < 2) return;
            var idx = Document.Map.Cameras.IndexOf(Document.Map.ActiveCamera);
            idx = (idx + Document.Map.Cameras.Count - 1) % Document.Map.Cameras.Count;
            Document.Map.ActiveCamera = Document.Map.Cameras[idx];
            SetViewportCamera(Document.Map.ActiveCamera.EyePosition, Document.Map.ActiveCamera.LookPosition);
        }

        private void CameraDelete()
        {
            if (_currentlyDragging || Document.Map.Cameras.Count < 2) return;
            var del = Document.Map.ActiveCamera;
            CameraPrevious();
            if (del != Document.Map.ActiveCamera) Document.Map.Cameras.Remove(del);
        }

        private void SetViewportCamera(Coordinate position, Coordinate look)
        {
            var cam = ViewportManager.Viewports.Where(x => x.Is3D).OfType<IViewport3D>().Select(x => x.Camera).FirstOrDefault();
            if (cam == null) return;

            look = (look - position).Normalise() + position;
            cam.Location = new Vector3((float)position.X, (float)position.Y, (float)position.Z);
            cam.LookAt = new Vector3((float)look.X, (float)look.Y, (float)look.Z);
        }
    }
}

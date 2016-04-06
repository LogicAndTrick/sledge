using System;
using System.Collections.Generic;
using System.Drawing;
using OpenTK;
using Sledge.DataStructures.Geometric;
using Sledge.Editor.Extensions;
using Sledge.Editor.Rendering;
using Sledge.Editor.Tools.SelectTool;
using Sledge.Rendering.Cameras;

namespace Sledge.Editor.Documents
{
    internal class DocumentMemory
    {
        private readonly Dictionary<string, string> _positions;
        private Vector3 _cameraLookat;
        private Vector3 _cameraLocation;
        public Type SelectedTool { get; set; }
        public PointF SplitterPosition { get; set; }

        private Dictionary<string, object> _store; 

        public DocumentMemory()
        {
            _positions = new Dictionary<string, string>();
            _cameraLocation = new Vector3(0, 0, 0);
            _cameraLookat = new Vector3(1, 0, 0);
            SelectedTool = typeof (SelectTool);
            _store = new Dictionary<string, object>();
        }

        public void SetCamera(Coordinate position, Coordinate look)
        {
            _cameraLocation = position.ToVector3();
            _cameraLookat = look.ToVector3();
        }

        public void RememberViewports(IEnumerable<MapViewport> viewports)
        {
            _positions.Clear();
            var foundPerspective = false;
            foreach (var vp in viewports)
            {
                _positions.Add(vp.Viewport.ViewportHandle, Camera.Serialise(vp.Viewport.Camera));
                if (vp.Viewport.Camera is PerspectiveCamera && !foundPerspective)
                {
                    var cam = vp.Viewport.Camera as PerspectiveCamera;
                    _cameraLookat = cam.LookAt;
                    _cameraLocation = cam.Position;
                    foundPerspective = true;
                }
            }
        }

        public void RestoreViewports(IEnumerable<MapViewport> viewports)
        {
            foreach (var vp in viewports)
            {
                if (_positions.ContainsKey(vp.Viewport.ViewportHandle))
                {
                    try
                    {
                        var cam = Camera.Deserialise(_positions[vp.Viewport.ViewportHandle]);
                        vp.Viewport.Camera = cam;
                        continue;
                    }
                    catch
                    {
                        // 
                    }
                }
                if (vp.Viewport.Camera is PerspectiveCamera)
                {
                    var cam = vp.Viewport.Camera as PerspectiveCamera;
                    cam.LookAt = _cameraLookat;
                    cam.Position = _cameraLocation;
                }
            }
        }

        public void Set<T>(string name, T state)
        {
            if (_store.ContainsKey(name)) _store.Remove(name);
            _store.Add(name, state);
        }

        public T Get<T>(string name, T def = default (T))
        {
            if (!_store.ContainsKey(name)) return def;
            var obj = _store[name];
            return obj is T ? (T) obj : def;
        }
    }
}
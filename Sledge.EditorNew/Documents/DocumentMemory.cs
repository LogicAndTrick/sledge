using System;
using System.Collections.Generic;
using System.Drawing;
using OpenTK;
using Sledge.DataStructures.Geometric;
using Sledge.EditorNew.UI.Viewports;

namespace Sledge.EditorNew.Documents
{
    internal class DocumentMemory
    {
        private readonly Dictionary<ViewDirection, Tuple<Coordinate, decimal>> _positions;
        private Vector3 _cameraLookat;
        private Vector3 _cameraLocation;
        public Type SelectedTool { get; set; }
        public PointF SplitterPosition { get; set; }

        private Dictionary<string, object> _store; 

        public DocumentMemory()
        {
            _positions = new Dictionary<ViewDirection, Tuple<Coordinate, decimal>>();
            _cameraLocation = new Vector3(0, 0, 0);
            _cameraLookat = new Vector3(1, 0, 0);
            // todo SelectedTool = typeof (SelectTool);
            _store = new Dictionary<string, object>();
        }

        public void SetCamera(Coordinate position, Coordinate look)
        {
            _cameraLocation = new Vector3((float)position.X, (float)position.Y, (float)position.Z);
            _cameraLookat = new Vector3((float)look.X, (float)look.Y, (float)look.Z);
        }

        public void RememberViewports(IEnumerable<IMapViewport> viewports)
        {
            // Todo viewport: remember types and positions
            _positions.Clear();
            foreach (var vp in viewports)
            {
                if (vp.Is2D)
                {
                    var vp2 = vp as IViewport2D;
                    if (!_positions.ContainsKey(vp2.Direction))
                    {
                        _positions.Add(vp2.Direction, Tuple.Create(vp2.Position, vp2.Zoom));
                    }
                }
                if (vp.Is3D)
                {
                    var vp3 = vp as IViewport3D;
                    var cam = vp3.Camera;
                    _cameraLookat = cam.LookAt;
                    _cameraLocation = cam.Location;
                }
            }
        }

        public void RestoreViewports(IEnumerable<IMapViewport> viewports)
        {
            foreach (var vp in viewports)
            {
                if (vp.Is2D)
                {
                    var vp2 = vp as IViewport2D;
                    if (_positions.ContainsKey(vp2.Direction))
                    {
                        vp2.Position = _positions[vp2.Direction].Item1;
                        vp2.Zoom = _positions[vp2.Direction].Item2;
                    }
                }
                if (vp.Is3D)
                {
                    var vp3 = vp as IViewport3D;
                    vp3.Camera.Location = _cameraLocation;
                    vp3.Camera.LookAt = _cameraLookat;
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
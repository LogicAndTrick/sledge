using System;
using System.Collections.Generic;
using System.Drawing;
using OpenTK;
using Sledge.DataStructures.Geometric;
using Sledge.Editor.Tools;
using Sledge.UI;

namespace Sledge.Editor.Documents
{
    internal class DocumentMemory
    {
        private readonly Dictionary<Viewport2D.ViewDirection, Tuple<Coordinate, decimal>> _positions;
        private Vector3 _cameraLookat;
        private Vector3 _cameraLocation;
        public Type SelectedTool { get; set; }
        public PointF SplitterPosition { get; set; }

        public DocumentMemory()
        {
            _positions = new Dictionary<Viewport2D.ViewDirection, Tuple<Coordinate, decimal>>();
            _cameraLocation = new Vector3(0, 0, 0);
            _cameraLookat = new Vector3(1, 0, 0);
            SelectedTool = typeof (SelectTool);
        }

        public void RememberViewports(IEnumerable<ViewportBase> viewports)
        {
            // Todo viewport: remember types and positions
            _positions.Clear();
            foreach (var vp in viewports)
            {
                var vp3 = vp as Viewport3D;
                var vp2 = vp as Viewport2D;
                if (vp2 != null)
                {
                    if (!_positions.ContainsKey(vp2.Direction))
                    {
                        _positions.Add(vp2.Direction, Tuple.Create(vp2.Position, vp2.Zoom));
                    }
                }
                if (vp3 != null)
                {
                    var cam = vp3.Camera;
                    _cameraLookat = cam.LookAt;
                    _cameraLocation = cam.Location;
                }
            }
        }

        public void RestoreViewports(IEnumerable<ViewportBase> viewports)
        {
            foreach (var vp in viewports)
            {
                var vp3 = vp as Viewport3D;
                var vp2 = vp as Viewport2D;
                if (vp2 != null)
                {
                    if (_positions.ContainsKey(vp2.Direction))
                    {
                        vp2.Position = _positions[vp2.Direction].Item1;
                        vp2.Zoom = _positions[vp2.Direction].Item2;
                    }
                }
                if (vp3 != null)
                {
                    vp3.Camera.Location = _cameraLocation;
                    vp3.Camera.LookAt = _cameraLookat;
                }
            }
        }
    }
}
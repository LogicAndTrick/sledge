using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows.Forms;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Components;
using Sledge.BspEditor.Documents;
using Sledge.Common.Shell.Documents;
using Sledge.DataStructures.Geometric;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Engine;
using Sledge.Rendering.Viewports;

namespace Sledge.BspEditor.Rendering.Viewport
{
    public class ViewportMapDocumentControl : IMapDocumentControl
    {
        private readonly EngineInterface _engine;
        private readonly IEnumerable<IViewportEventListenerFactory> _listeners;
        private readonly Control _panel;
        private IViewport _viewport;
        private MapViewport _mapViewport;

        private readonly List<Subscription> _subscriptions;
        private ICamera _camera;

        public string Type => "MapViewport";
        public Control Control => _panel;

        public ICamera Camera
        {
            get => _camera;
            set
            {
                _camera = value;
                if (_viewport != null) _viewport.Camera = value;
            }
        }

        public ViewportMapDocumentControl(EngineInterface engine, IEnumerable<IViewportEventListenerFactory> listeners)
        {
            _engine = engine;
            _listeners = listeners;
            _camera = new PerspectiveCamera();
            _panel = new Panel {Dock = DockStyle.Fill, BackColor = Color.Black};
            _subscriptions = new List<Subscription>
            {
                Oy.Subscribe<Vector3>("MapDocument:Viewport:Focus2D", Focus2D),
                Oy.Subscribe<Vector3>("MapDocument:Viewport:Focus3D", Focus3D),
                Oy.Subscribe<Box>("MapDocument:Viewport:Focus2D", Focus2D),
                Oy.Subscribe<Box>("MapDocument:Viewport:Focus3D", Focus3D),
                Oy.Subscribe<Tuple<Vector3, Vector3>>("MapDocument:Viewport:Set3D", Set3D),
                Oy.Subscribe<Vector3>("MapDocument:Viewport:Set2D", Set2D),
            };
            _viewport = _engine.CreateViewport();
            _viewport.Camera = _camera;
            _viewport.Control.Dock = DockStyle.Fill;
            _panel.Controls.Add(_viewport.Control);
            _mapViewport = new MapViewport(_viewport);
            _mapViewport.Listeners.AddRange(_listeners.SelectMany(x => x.Create(_mapViewport)));
            _mapViewport.ListenerException += ListenerException;
            Oy.Publish("MapViewport:Created", _mapViewport);
        }

        private void ListenerException(object sender, Exception exception)
        {
            Oy.Publish("Shell:UnhandledException", exception);
        }

        public string GetSerialisedSettings()
        {
            return Sledge.Rendering.Cameras.Camera.Serialise(Camera);
        }

        public void SetSerialisedSettings(string settings)
        {
            try
            {
                Camera = Sledge.Rendering.Cameras.Camera.Deserialise(settings);
            }
            catch
            {
                //
            }
        }

        #region Camera manipulation

        private Task Set3D(Tuple<Vector3, Vector3> pair)
        {
            if (Camera is PerspectiveCamera cam)
            {
                var position = pair.Item1;
                var look = pair.Item2;
                look = (look - position).Normalise() + position;
                cam.Position = position;
                cam.Direction = look - position;
            }
            return Task.FromResult(0);
        }

        private Task Set2D(Vector3 center)
        {
            if (Camera is OrthographicCamera cam)
            {
                cam.Position = center;
            }
            return Task.FromResult(0);
        }

        private Task Focus2D(Vector3 c)
        {
            if (Camera is OrthographicCamera cam)
            {
                cam.Position = cam.Flatten(c);
            }
            return Task.FromResult(0);
        }

        private Task Focus3D(Vector3 c)
        {
            if (Camera is PerspectiveCamera cam)
            {
                FocusOn(cam, c, Vector3.UnitY * -100);
            }
            return Task.FromResult(0);
        }

        private Task Focus2D(Box c)
        {
            if (Camera is OrthographicCamera cam)
            {
                cam.Position = cam.Flatten(c.Center);
            }
            return Task.FromResult(0);
        }

        private Task Focus3D(Box c)
        {
            if (Camera is PerspectiveCamera cam)
            {
                var dist = Math.Max(Math.Max(c.Width, c.Length), c.Height);
                var normal = cam.Direction;
                FocusOn(cam, c.Center, normal * -dist * 1.2f);
            }
            return Task.FromResult(0);
        }

        private void FocusOn(PerspectiveCamera cam, Vector3 coordinate, Vector3 distance)
        {
            var pos = coordinate + distance;
            cam.Position = pos;
            cam.Direction = coordinate - pos;
        }
        
        #endregion

        public void Dispose()
        {
            _subscriptions.ForEach(Oy.Unsubscribe);
            _viewport?.Dispose();
        }
    }
}
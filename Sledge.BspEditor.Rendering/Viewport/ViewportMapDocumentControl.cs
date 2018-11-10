using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows.Forms;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Components;
using Sledge.Common.Shell.Hotkeys;
using Sledge.DataStructures.Geometric;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Engine;
using Sledge.Rendering.Overlay;
using Sledge.Rendering.Viewports;

namespace Sledge.BspEditor.Rendering.Viewport
{
    public class ViewportMapDocumentControl : IMapDocumentControl, IHotkeyFilter
    {
        private readonly EngineInterface _engine;
        private readonly IViewport _viewport;
        private readonly MapViewport _mapViewport;
        private readonly ListenerOverlayRenderable _overlayRenderable;

        private readonly List<Subscription> _subscriptions;
        private ICamera _camera;

        public bool IsFocused => _viewport.IsFocused;

        public string OrderHint => "M";

        public string Type => "MapViewport";
        public Control Control { get; }

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
            _camera = new PerspectiveCamera();
            Control = new Panel {Dock = DockStyle.Fill, BackColor = Color.Black};

            _subscriptions = new List<Subscription>
            {
                Oy.Subscribe<Vector3>("MapDocument:Viewport:Focus2D", Focus2D),
                Oy.Subscribe<Vector3>("MapDocument:Viewport:Focus3D", Focus3D),
                Oy.Subscribe<Box>("MapDocument:Viewport:Focus2D", Focus2D),
                Oy.Subscribe<Box>("MapDocument:Viewport:Focus3D", Focus3D),
                Oy.Subscribe<Tuple<Vector3, Vector3>>("MapDocument:Viewport:Set3D", Set3D),
                Oy.Subscribe<Vector3>("MapDocument:Viewport:Set2D", Set2D),
                Oy.Subscribe<float>("MapDocument:Viewport:SetZoom", SetZoom),
                Oy.Subscribe<int>("MapDocument:Viewport:SetFOV", SetFOV),
            };

            _viewport = _engine.CreateViewport();
            _viewport.Camera = _camera;
            _viewport.Control.Dock = DockStyle.Fill;
            Control.Controls.Add(_viewport.Control);

            _mapViewport = new MapViewport(_viewport);
            _mapViewport.Listeners.AddRange(listeners.SelectMany(x => x.Create(_mapViewport)));
            _mapViewport.ListenerException += ListenerException;

            _overlayRenderable = new ListenerOverlayRenderable(_mapViewport);
            _engine.Add(_overlayRenderable);

            Oy.Publish("MapViewport:Created", _mapViewport);
            Oy.Publish<IHotkeyFilter>("Hotkeys:AddFilter", this);
        }

        public bool Filter(string hotkey, int keys)
        {
            return _mapViewport.Listeners.OrderBy(x => x.OrderHint).Any(l => l.Filter(hotkey, keys));
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

        private Task SetZoom(float zoom)
        {
            if (Camera is OrthographicCamera cam)
            {
                cam.Zoom = zoom;
            }
            return Task.FromResult(0);
        }

        private Task SetFOV(int fov)
        {
            if (Camera is PerspectiveCamera cam)
            {
                cam.FOV = fov;
            }
            return Task.FromResult(0);
        }

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
            Oy.Publish<IHotkeyFilter>("Hotkeys:RemoveFilter", this);

            _mapViewport.Listeners.ForEach(x => x.Dispose());
            _mapViewport.Listeners.Clear();

            _engine.Remove(_overlayRenderable);
            _subscriptions.ForEach(Oy.Unsubscribe);

            _viewport?.Dispose();
        }

        private class ListenerOverlayRenderable : IOverlayRenderable
        {
            private readonly MapViewport _control;

            public ListenerOverlayRenderable(MapViewport control)
            {
                _control = control;
            }
            
            public void Render(IViewport viewport, OrthographicCamera camera, Vector3 worldMin, Vector3 worldMax, I2DRenderer im)
            {
                foreach (var r in _control.Listeners.OfType<IOverlayRenderable>())
                {
                    r.Render(viewport, camera, worldMin, worldMax, im);
                }
            }

            public void Render(IViewport viewport, PerspectiveCamera camera, I2DRenderer im)
            {
                foreach (var r in _control.Listeners.OfType<IOverlayRenderable>())
                {
                    r.Render(viewport, camera, im);
                }
            }
        }
    }
}
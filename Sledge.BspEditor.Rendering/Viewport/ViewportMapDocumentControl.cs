using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Components;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Primitives.MapData;
using Sledge.Common.Shell.Documents;
using Sledge.DataStructures.Geometric;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Engine;
using Sledge.Rendering.Interfaces;
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
            _panel = new Panel {Dock = DockStyle.Fill, BackColor = Color.LightCyan};
            _subscriptions = new List<Subscription>
            {
                Oy.Subscribe<IDocument>("Document:Activated", DocumentActivated),
                //Oy.Subscribe<Coordinate>("MapDocument:Viewport:Focus2D", Focus2D),
                //Oy.Subscribe<Coordinate>("MapDocument:Viewport:Focus3D", Focus3D),
                //Oy.Subscribe<Box>("MapDocument:Viewport:Focus2D", Focus2D),
                //Oy.Subscribe<Box>("MapDocument:Viewport:Focus3D", Focus3D),
                //Oy.Subscribe<Tuple<Coordinate, Coordinate>>("MapDocument:Viewport:Set3D", Set3D),
                //Oy.Subscribe<Coordinate>("MapDocument:Viewport:Set2D", Set2D),
            };
        }

        //private Task Set3D(Tuple<Coordinate, Coordinate> pair)
        //{
        //    if (Camera is PerspectiveCamera cam)
        //    {
        //        var position = pair.Item1;
        //        var look = pair.Item2;
        //        look = (look - position).Normalise() + position;
        //        cam.Position = position.ToVector3();
        //        cam.LookAt = look.ToVector3();
        //    }
        //    return Task.FromResult(0);
        //}

        //private Task Set2D(Coordinate center)
        //{
        //    if (Camera is OrthographicCamera cam)
        //    {
        //        cam.Position = center.ToVector3();
        //    }
        //    return Task.FromResult(0);
        //}

        //private Task Focus2D(Coordinate c)
        //{
        //    if (Camera is OrthographicCamera) _mapViewport.FocusOn(c);
        //    return Task.FromResult(0);
        //}

        //private Task Focus3D(Coordinate c)
        //{
        //    if (Camera is PerspectiveCamera) _mapViewport.FocusOn(c);
        //    return Task.FromResult(0);
        //}

        //private Task Focus2D(Box c)
        //{
        //    if (Camera is OrthographicCamera) _mapViewport.FocusOn(c);
        //    return Task.FromResult(0);
        //}

        //private Task Focus3D(Box c)
        //{
        //    if (Camera is PerspectiveCamera) _mapViewport.FocusOn(c);
        //    return Task.FromResult(0);
        //}

        private async Task DocumentActivated(IDocument doc)
        {
            var mapDoc = doc as MapDocument;

            if (mapDoc == null) return;
            if (_viewport == null)
            {
                _viewport = _engine.CreateViewport();
                _viewport.Camera = _camera;
                _viewport.Control.Dock = DockStyle.Fill;
                _panel.Controls.Add(_viewport.Control);
                _mapViewport = new MapViewport(_viewport);
                _mapViewport.Listeners.AddRange(_listeners.SelectMany(x => x.Create(_mapViewport)));
                await Oy.Publish("MapViewport:Created", _mapViewport);
            }
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
                
            }
        }

        public void Dispose()
        {
            _subscriptions.ForEach(Oy.Unsubscribe);
            _viewport?.Dispose();
        }
    }
}
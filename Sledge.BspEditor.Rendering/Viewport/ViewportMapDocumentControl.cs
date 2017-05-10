using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Components;
using Sledge.BspEditor.Documents;
using Sledge.Common.Shell.Documents;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Interfaces;

namespace Sledge.BspEditor.Rendering.Viewport
{
    public class ViewportMapDocumentControl : IMapDocumentControl
    {
        private readonly IEnumerable<IViewportEventListenerFactory> _listeners;
        private readonly Control _panel;
        private IViewport _viewport;
        private MapViewport _mapViewport;

        private readonly List<Subscription> _subscriptions;
        private Camera _camera;

        public string Type => "MapViewport";
        public Control Control => _panel;

        public Camera Camera
        {
            get => _camera;
            set
            {
                _camera = value;
                if (_viewport != null) _viewport.Camera = value;
            }
        }

        public ViewportMapDocumentControl(IEnumerable<IViewportEventListenerFactory> listeners)
        {
            _listeners = listeners;
            _camera = new PerspectiveCamera();
            _panel = new Panel {Dock = DockStyle.Fill};
            _subscriptions = new List<Subscription>
            {
                Oy.Subscribe<IDocument>("Document:Activated", DocumentActivated)
            };
        }

        private async Task DocumentActivated(IDocument doc)
        {
            var mapDoc = doc as MapDocument;

            if (mapDoc == null) return;
            if (_viewport == null)
            {
                _viewport = Renderer.Instance.Engine.CreateViewport(_camera);
                _viewport.Control.Dock = DockStyle.Fill;
                _panel.Controls.Add(_viewport.Control);
                _mapViewport = new MapViewport(_viewport);
                _mapViewport.Listeners.AddRange(_listeners.SelectMany(x => x.Create(_mapViewport)));
                await Oy.Publish("MapViewport:Created", _mapViewport);
            }
        }

        public string GetSerialisedSettings()
        {
            return Camera.Serialise(Camera);
        }

        public void SetSerialisedSettings(string settings)
        {
            try
            {
                Camera = Camera.Deserialise(settings);
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
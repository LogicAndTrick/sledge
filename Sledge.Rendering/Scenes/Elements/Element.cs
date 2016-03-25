using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Interfaces;

namespace Sledge.Rendering.Scenes.Elements
{
    public abstract class Element : SceneObject
    {
        private CameraFlags _cameraFlags;

        public CameraFlags CameraFlags
        {
            get { return _cameraFlags; }
            set
            {
                _cameraFlags = value;
                OnPropertyChanged("CameraFlags");
                OnPropertyChanged("RenderCritical");
            }
        }

        public IViewport Viewport { get; set; }
        public PositionType PositionType { get; set; }
        public bool DepthTested { get; set; }
        public int ZIndex { get; set; }
        public bool Smooth { get; set; }
        public abstract string ElementGroup { get; }

        protected Element(PositionType positionType)
        {
            PositionType = positionType;
            DepthTested = true;
        }

        private readonly Dictionary<string, Dictionary<string, object>> _storage = new Dictionary<string, Dictionary<string, object>>();

        protected T GetValue<T>(IViewport viewport, string name, T defaultValue = default(T))
        {
            if (!_storage.ContainsKey(name)) return defaultValue;
            var dict = _storage[name];
            if (!dict.ContainsKey(viewport.ViewportHandle)) return defaultValue;
            return (T) dict[viewport.ViewportHandle];
        }

        protected void SetValue<T>(IViewport viewport, string name, T value)
        {
            if (!_storage.ContainsKey(name)) _storage[name] = new Dictionary<string, object>();
            _storage[name][viewport.ViewportHandle] = value;
        }

        protected void ClearValue(IViewport viewport, string name)
        {
            if (!_storage.ContainsKey(name)) return;
            var dict = _storage[name];
            if (!dict.ContainsKey(viewport.ViewportHandle)) return;
            dict.Remove(viewport.ViewportHandle);
        }

        protected void ClearValue(string name)
        {
            if (!_storage.ContainsKey(name)) return;
            _storage.Remove(name);
        }

        public abstract bool RequiresValidation(IViewport viewport, IRenderer renderer);
        public abstract void Validate(IViewport viewport, IRenderer renderer);

        public abstract IEnumerable<LineElement> GetLines(IViewport viewport, IRenderer renderer);
        public abstract IEnumerable<FaceElement> GetFaces(IViewport viewport, IRenderer renderer);
    }
}

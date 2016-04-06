using System.Drawing;
using OpenTK;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.DataStructures;
using Sledge.Rendering.Interfaces;
using Sledge.Rendering.Materials;

namespace Sledge.Rendering.Scenes.Renderables
{
    public abstract class RenderableObject : SceneObject, IBounded
    {
        private Material _material;
        private Box _boundingBox;
        private LightingFlags _lighting;
        private RenderFlags _renderFlags;
        private RenderFlags _forcedRenderFlags;
        private CameraFlags _cameraFlags;
        private Color _accentColor;
        private Color _pointColor;
        private Color _tintColor;

        public Material Material
        {
            get { return _material; }
            set
            {
                var uid = _material == null ? null : _material.UniqueIdentifier;
                _material = value;
                var nuid = _material == null ? null : _material.UniqueIdentifier;
                if (uid != nuid) OnPropertyChanged("RenderCritical");
                OnPropertyChanged("Material");
            }
        }

        public Color AccentColor
        {
            get { return _accentColor; }
            set
            {
                _accentColor = value;
                OnPropertyChanged("AccentColor");
            }
        }

        public Color PointColor
        {
            get { return _pointColor; }
            set
            {
                _pointColor = value;
                OnPropertyChanged("PointColor");
            }
        }

        public Color TintColor
        {
            get { return _tintColor; }
            set
            {
                _tintColor = value;
                OnPropertyChanged("TintColor");
            }
        }

        public Vector3 Origin { get { return BoundingBox.Center; } }

        public Box BoundingBox
        {
            get { return _boundingBox; }
            set
            {
                _boundingBox = value;
                OnPropertyChanged("BoundingBox");
                OnPropertyChanged("Origin");
                OnPropertyChanged("RenderCritical");
            }
        }

        public LightingFlags Lighting
        {
            get { return _lighting; }
            set
            {
                _lighting = value;
                OnPropertyChanged("Lighting");
            }
        }

        public RenderFlags RenderFlags
        {
            get { return _renderFlags; }
            set
            {
                _renderFlags = value;
                OnPropertyChanged("RenderFlags");
                OnPropertyChanged("RenderCritical");
            }
        }

        public RenderFlags ForcedRenderFlags
        {
            get { return _forcedRenderFlags; }
            set
            {
                _forcedRenderFlags = value;
                OnPropertyChanged("ForcedRenderFlags");
                OnPropertyChanged("RenderCritical");
            }
        }

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

        protected RenderableObject()
        {
            _lighting = LightingFlags.Dynamic;
            _forcedRenderFlags = RenderFlags.None;
            _accentColor = Color.White;
            _tintColor = Color.White;
            _pointColor = Color.Transparent;
        }
    }
}
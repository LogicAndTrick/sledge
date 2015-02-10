using System.Drawing;
using Sledge.DataStructures.Geometric;
using Sledge.Rendering.Interfaces;
using Sledge.Rendering.Materials;

namespace Sledge.Rendering.Scenes.Renderables
{
    public abstract class RenderableObject : SceneObject, IBounded
    {
        private Material _material;
        private Box _boundingBox;
        private LightingType _lighting;
        private RenderFlags _renderFlags;
        private CameraFlags _cameraFlags;
        private Color _accentColor;
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

        public Color TintColor
        {
            get { return _tintColor; }
            set
            {
                _tintColor = value;
                OnPropertyChanged("TintColor");
            }
        }

        public Coordinate Origin { get { return BoundingBox.Center; } }

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

        public LightingType Lighting
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
    }
}
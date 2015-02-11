using System.Collections.Generic;
using System.Drawing;
using Sledge.DataStructures.Geometric;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Interfaces;
using Sledge.Rendering.Materials;

namespace Sledge.Rendering.Scenes.Renderables
{
    public abstract class RenderableObject : SceneObject, IBounded
    {
        private Box _boundingBox;
        private LightingFlags _lighting;
        private RenderFlags _renderFlags;
        private RenderFlags _forcedRenderFlags;
        private CameraFlags _cameraFlags;
        private Color _accentColor;
        private Color _tintColor;

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
    }
}
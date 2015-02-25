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

        protected Element(PositionType positionType)
        {
            PositionType = positionType;
        }

        public abstract IEnumerable<LineElement> GetLines(IRenderer renderer);
        public abstract IEnumerable<FaceElement> GetFaces(IRenderer renderer);
    }
}

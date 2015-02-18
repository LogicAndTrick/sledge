using System;
using System.Linq;
using System.Text;
using OpenTK;
using Sledge.Rendering.Cameras;

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
    }
}

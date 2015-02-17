using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
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

    public class LineElement : Element
    {
        public float Width { get; set; }
        public Color Color { get; private set; }
        public List<Position> Vertices { get; set; }

        public LineElement(Color color, List<Position> vertices)
        {
            Color = color;
            Vertices = vertices;
            Width = 1; // todo change line widths?
        }
    }
}

using System.ComponentModel.Composition;
using Sledge.BspEditor.Components;
using Sledge.Rendering.Cameras;

namespace Sledge.BspEditor.Rendering.Viewport
{
    [Export(typeof(IMapDocumentControlFactory))]
    public class MapViewportControlFactory : IMapDocumentControlFactory
    {
        public string Type => "MapViewport";

        public IMapDocumentControl Create(string serialised)
        {
            Camera camera;
            if (serialised == null)
            {
                camera = new PerspectiveCamera();
            }
            else
            {
                switch (serialised)
                {
                    case "OrthographicCamera/Top":
                        camera = new OrthographicCamera(OrthographicCamera.OrthographicType.Top);
                        break;
                    case "OrthographicCamera/Front":
                        camera = new OrthographicCamera(OrthographicCamera.OrthographicType.Front);
                        break;
                    case "OrthographicCamera/Side":
                        camera = new OrthographicCamera(OrthographicCamera.OrthographicType.Side);
                        break;
                    case "PerspectiveCamera":
                    default:
                        camera = new PerspectiveCamera();
                        break;
                }
            }
            return new ViewportMapDocumentControl {Camera = camera};
        }
    }
}
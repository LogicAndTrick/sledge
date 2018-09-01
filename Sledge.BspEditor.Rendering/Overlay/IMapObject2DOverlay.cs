using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Viewports;

namespace Sledge.BspEditor.Rendering.Overlay
{
    public interface IMapObject2DOverlay
    {
        void Render(IViewport viewport, ICollection<IMapObject> objects, OrthographicCamera camera, Vector3 worldMin, Vector3 worldMax, Graphics graphics);
    }
}
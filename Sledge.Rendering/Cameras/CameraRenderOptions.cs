using Sledge.Rendering.Scenes.Renderables;

namespace Sledge.Rendering.Cameras
{
    public class CameraRenderOptions
    {
        public bool RenderFacePolygons { get; set; }
        public bool RenderFacePolygonTextures { get; set; }
        public LightingFlags RenderFacePolygonLighting { get; set; }

        public bool RenderFaceWireframe { get; set; }
        public bool RenderFacePoints { get; set; }

        public bool RenderLineWireframe { get; set; }
        public bool RenderLinePoints { get; set; }
    }
}
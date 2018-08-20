using System.Numerics;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Rendering.Viewport;
using Sledge.BspEditor.Tools.Draggable;
using Sledge.Rendering.Cameras;

namespace Sledge.BspEditor.Tools.Selection.TransformationHandles
{
    public interface ITransformationHandle : IDraggable
    {
        string Name { get; }
        Matrix4x4? GetTransformationMatrix(MapViewport viewport, OrthographicCamera camera, BoxState state, MapDocument doc);
        TextureTransformationType GetTextureTransformationType(MapDocument doc);
    }
}

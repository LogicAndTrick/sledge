using OpenTK;
using Sledge.Editor.Documents;
using Sledge.Editor.Rendering;
using Sledge.Editor.Tools.DraggableTool;
using Sledge.Rendering.Cameras;

namespace Sledge.Editor.Tools.SelectTool.TransformationHandles
{
    public interface ITransformationHandle : IDraggable
    {
        string Name { get; }
        Matrix4? GetTransformationMatrix(MapViewport viewport, OrthographicCamera camera, BoxState state, Document doc);
    }
}

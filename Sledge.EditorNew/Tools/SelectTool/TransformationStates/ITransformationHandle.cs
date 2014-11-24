using OpenTK;
using Sledge.EditorNew.Documents;
using Sledge.EditorNew.Tools.DraggableTool;
using Sledge.EditorNew.UI.Viewports;

namespace Sledge.EditorNew.Tools.SelectTool.TransformationStates
{
    public interface ITransformationHandle : IDraggable
    {
        string Name { get; }
        Matrix4? GetTransformationMatrix(IViewport2D viewport, BoxState state, Document doc);
    }
}

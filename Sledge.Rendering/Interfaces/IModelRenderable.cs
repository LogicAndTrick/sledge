using Sledge.Rendering.Engine;
using Sledge.Rendering.Renderables;
using Sledge.Rendering.Resources;

namespace Sledge.Rendering.Interfaces
{
    public interface IModelRenderable : IRenderable, IUpdateable, IResource
    {
        IModel Model { get; }
    }
}
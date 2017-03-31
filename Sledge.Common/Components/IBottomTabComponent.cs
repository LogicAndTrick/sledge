using Sledge.Common.Context;

namespace Sledge.Common.Components
{
    public interface IBottomTabComponent
    {
        string Title { get; }
        object Control { get; }
        bool IsInContext(IContext context);
    }
}
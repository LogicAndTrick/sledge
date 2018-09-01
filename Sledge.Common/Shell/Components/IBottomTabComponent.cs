using Sledge.Common.Shell.Context;

namespace Sledge.Common.Shell.Components
{
    public interface IBottomTabComponent : IContextAware
    {
        string Title { get; }
        object Control { get; }
    }
}
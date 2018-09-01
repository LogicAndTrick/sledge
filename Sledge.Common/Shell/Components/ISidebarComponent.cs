using Sledge.Common.Shell.Context;

namespace Sledge.Common.Shell.Components
{
    public interface ISidebarComponent : IContextAware
    {
        string Title { get; }
        object Control { get; }
    }
}

using Sledge.Common.Shell.Context;

namespace Sledge.Common.Shell.Components
{
    /// <summary>
    /// A component that is hosted in the sidebar of the shell.
    /// </summary>
    public interface ISidebarComponent : IContextAware
    {
        /// <summary>
        /// The title of the sidebar component
        /// </summary>
        string Title { get; }

        /// <summary>
        /// The control to host in the sidebar
        /// </summary>
        object Control { get; }
    }
}

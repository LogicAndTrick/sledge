using Sledge.Common.Shell.Context;

namespace Sledge.Common.Shell.Components
{
    /// <summary>
    /// A component that will appear in the bottom tab control of the shell.
    /// </summary>
    public interface IBottomTabComponent : IContextAware
    {
        /// <summary>
        /// The title of the tab
        /// </summary>
        string Title { get; }

        /// <summary>
        /// The control to host within the tab
        /// </summary>
        object Control { get; }
    }
}
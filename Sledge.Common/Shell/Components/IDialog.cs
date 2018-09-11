using Sledge.Common.Shell.Context;

namespace Sledge.Common.Shell.Components
{
    /// <summary>
    /// A dialog window which will be hosted by the shell.
    /// </summary>
    public interface IDialog : IContextAware
    {
        /// <summary>
        /// Set the visibility status of the dialog
        /// </summary>
        /// <param name="context">The shell context</param>
        /// <param name="visible">The visibility state to set</param>
        void SetVisible(IContext context, bool visible);
    }
}
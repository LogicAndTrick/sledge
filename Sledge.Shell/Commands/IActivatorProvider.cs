using System.Collections.Generic;

namespace Sledge.Shell.Commands
{
    /// <summary>
    /// An activator provider for the shell command box
    /// </summary>
    public interface IActivatorProvider
    {
        /// <summary>
        /// The search function for activators
        /// </summary>
        /// <param name="keywords">The text typed into the command box</param>
        /// <returns></returns>
        IEnumerable<IActivator> SearchActivators(string keywords);
    }
}
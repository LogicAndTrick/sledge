using System.Threading.Tasks;

namespace Sledge.Shell.Commands
{
    /// <summary>
    /// An activator are meta-commands exposed by the command box.
    /// They are usually wrappers for commands, open documents, and so on.
    /// </summary>
    public interface IActivator
    {
        /// <summary>
        /// The activator group
        /// </summary>
        string Group { get; }

        /// <summary>
        /// The activator name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The activator description
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Invoke the activator
        /// </summary>
        /// <returns>The running task</returns>
        Task Activate();
    }
}

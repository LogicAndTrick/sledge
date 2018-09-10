using System.Threading.Tasks;
using Sledge.Common.Shell.Context;

namespace Sledge.Common.Shell.Commands
{
    /// <summary>
    /// A command is a context-aware operation that is executed by an external trigger
    /// like a hotkey, mouse click, timer, network callback, etc. It will usually
    /// perform some sort of change operation, sometimes as simple as opening a
    /// window, other times it could modify the open document or write to the file system.
    ///
    /// The command is the main interface to all operations within the shell.
    /// </summary>
    public interface ICommand : IContextAware
    {
        /// <summary>
        /// The name of the command
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Details of what the command does
        /// </summary>
        string Details { get; }
        
        /// <summary>
        /// Perform the command
        /// </summary>
        /// <param name="context">The current context</param>
        /// <param name="parameters">The command parameters</param>
        /// <returns>Completion task</returns>
        Task Invoke(IContext context, CommandParameters parameters);
    }
}

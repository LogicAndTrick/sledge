using System.Threading.Tasks;
using Sledge.Common.Shell.Context;

namespace Sledge.Common.Shell.Hotkeys
{
    /// <summary>
    /// An action that can be run by a hotkey sequence in the shell.
    /// </summary>
    public interface IHotkey : IContextAware
    {
        /// <summary>
        /// The ID of the hotkey
        /// </summary>
        string ID { get; }

        /// <summary>
        /// The name of the hotkey command
        /// </summary>
        string Name { get; }

        /// <summary>
        /// A description of the hotkey's command
        /// </summary>
        string Description { get; }

        /// <summary>
        /// The default hotkey for this command
        /// </summary>
        string DefaultHotkey { get; }
        
        /// <summary>
        /// Invoke the hotkey's command
        /// </summary>
        /// <returns>Completion task</returns>
        Task Invoke(); 
    }
}

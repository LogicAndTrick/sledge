using System;
using Sledge.Common.Shell.Context;

namespace Sledge.Common.Shell.Components
{
    /// <summary>
    /// A control which is hosted in the status bar of the shell.
    /// </summary>
    public interface IStatusItem : IContextAware
    {
        /// <summary>
        /// The ID of the status item
        /// </summary>
        string ID { get; }

        /// <summary>
        /// The preferred width of the status item. Set to 0 to autosize.
        /// </summary>
        int Width { get; }

        /// <summary>
        /// True to draw a border around this status item
        /// </summary>
        bool HasBorder { get; }

        /// <summary>
        /// The text to display in the status item
        /// </summary>
        string Text { get; }

        /// <summary>
        /// Fires when the status text has changed and needs to be updated in the shell
        /// </summary>
        event EventHandler<string> TextChanged;
    }
}

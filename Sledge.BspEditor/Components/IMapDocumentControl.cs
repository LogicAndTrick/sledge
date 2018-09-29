using System;
using System.Windows.Forms;

namespace Sledge.BspEditor.Components
{
    /// <summary>
    /// A control that displays a visual into a map document.
    /// Typically this is a 2D or 3D viewport.
    /// </summary>
    public interface IMapDocumentControl : IDisposable
    {
        /// <summary>
        /// If the control is "active", or the user is focusing on it.s
        /// </summary>
        bool IsFocused { get; }

        /// <summary>
        /// The type of the control
        /// </summary>
        string Type { get; }

        /// <summary>
        /// The control itself
        /// </summary>
        Control Control { get; }

        /// <summary>
        /// Get a string that represents the viewport type and any additional settings that should be persisted
        /// </summary>
        string GetSerialisedSettings();

        /// <summary>
        /// Set the serialised settings for this control
        /// </summary>
        void SetSerialisedSettings(string settings);
    }
}
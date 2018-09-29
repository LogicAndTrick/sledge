using System.Collections.Generic;

namespace Sledge.BspEditor.Components
{
    /// <summary>
    /// A factory that creates and manages map document controls
    /// </summary>
    public interface IMapDocumentControlFactory
    {
        /// <summary>
        /// Get the type of controls this factory creates
        /// </summary>
        string Type { get; }

        /// <summary>
        /// Create a control
        /// </summary>
        IMapDocumentControl Create();

        /// <summary>
        /// Check if a control is owned by this factory
        /// </summary>
        /// <param name="control">The control to test</param>
        bool IsType(IMapDocumentControl control);

        /// <summary>
        /// Get a list of control styles this factory knows about.
        /// The list of styles is used when creating default controls for a new window or when there's no settings.
        /// </summary>
        Dictionary<string, string> GetStyles();

        /// <summary>
        /// Test if a given control is a particular style
        /// </summary>
        /// <param name="control">The control to test. Must be a control of this factory's type</param>
        /// <param name="style">The style to test for</param>
        bool IsStyle(IMapDocumentControl control, string style);
    }
}
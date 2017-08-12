using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common.Shell.Context;

namespace Sledge.BspEditor.Editing.Components.Properties
{
    /// <summary>
    /// A tab in the object properties dialog
    /// </summary>
    public interface IObjectPropertyEditorTab : IContextAware, INotifyPropertyChanged
    {
        /// <summary>
        /// The suggested order of the tab
        /// </summary>
        string OrderHint { get; }

        /// <summary>
        /// The name of the tab
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The actual control of the editor tab
        /// </summary>
        Control Control { get; }

        /// <summary>
        /// True if the tab properties have been modified by the user since the last object set
        /// </summary>
        bool HasChanges { get; }

        /// <summary>
        /// Sets the list of objects that this tab should show the properties of.
        /// This resets the <code>HasChanges</code> flag to false.
        /// </summary>
        /// <param name="document">The document</param>
        /// <param name="objects">The list of objects</param>
        /// <returns>Completion task</returns>
        Task SetObjects(MapDocument document, List<IMapObject> objects);

        /// <summary>
        /// Get the list of changes that have been applied by the user.
        /// </summary>
        /// <param name="document">The document</param>
        /// <param name="objects">The list of objects</param>
        /// <returns>The list of changes</returns>
        IEnumerable<IOperation> GetChanges(MapDocument document, List<IMapObject> objects);
    }
}

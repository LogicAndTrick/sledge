using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common.Shell.Context;

namespace Sledge.BspEditor.Editing.Components.Properties
{
    public interface IObjectPropertyEditorTab : IContextAware
    {
        string OrderHint { get; }

        string Name { get; }

        Control Control { get; }

        bool HasChanges { get; }

        Task SetObjects(MapDocument document, List<IMapObject> objects);

        IEnumerable<MapDocumentOperation> GetChanges(MapDocument document);
    }
}

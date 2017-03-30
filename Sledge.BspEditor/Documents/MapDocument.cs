using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using LogicAndTrick.Oy;
using Newtonsoft.Json;
using Sledge.BspEditor.Components;
using Sledge.BspEditor.Controls;
using Sledge.BspEditor.Primitives;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common.Documents;

namespace Sledge.BspEditor.Documents
{
    public class MapDocument : IDocument
    {
        public string Name { get; }
        public object Control => MapDocumentControlHost.Instance;

        public Map Map { get; set; }

        public MapDocument(Map map)
        {
            Name = "Untitled";
            Map = map;
            
            Oy.Subscribe<IDocument>("Document:RequestClose", IfThis(RequestClose));
        }

        private Action<IDocument> IfThis(Func<Task> callback)
        {
            return d =>
            {
                if (d == this) callback();
            };
        }

        private async Task RequestClose()
        {
            await Oy.Publish("Document:Closed", this);
        }
    }
}
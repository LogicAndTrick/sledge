using System;
using System.Threading.Tasks;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Components;
using Sledge.BspEditor.Environment;
using Sledge.BspEditor.Primitives;
using Sledge.Common.Documents;

namespace Sledge.BspEditor.Documents
{
    public class MapDocument : IDocument
    {
        public string Name { get; }
        public object Control => MapDocumentControlHost.Instance;

        public Map Map { get; set; }
        public IEnvironment Environment { get; }

        public MapDocument(Map map, IEnvironment environment)
        {
            Name = "Untitled";
            Map = map;

            Environment = environment;

            Oy.Subscribe<IDocument>("Document:RequestClose", IfThis(RequestClose));

            // Subscribe to map changes
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
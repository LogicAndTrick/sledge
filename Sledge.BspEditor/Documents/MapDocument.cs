using System;
using System.Linq;
using System.Threading.Tasks;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Components;
using Sledge.BspEditor.Environment;
using Sledge.BspEditor.Primitives;
using Sledge.BspEditor.Primitives.MapData;
using Sledge.Common.Shell.Documents;

namespace Sledge.BspEditor.Documents
{
    public class MapDocument : IDocument
    {
        public string Name { get; set; }
        public string FileName { get; set; }
        public object Control => MapDocumentControlHost.Instance;

        public Map Map { get; set; }
        public IEnvironment Environment { get; }

        public Selection Selection
        {
            get
            {
                var sel = Map.Data.Get<Selection>().FirstOrDefault();
                if (sel != null) return sel;

                sel = new Selection();
                Map.Data.Add(sel);
                return sel;
            }
        }

        public MapDocument(Map map, IEnvironment environment)
        {
            FileName = null;
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
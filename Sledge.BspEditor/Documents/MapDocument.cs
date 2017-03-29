using System.ComponentModel;
using Sledge.BspEditor.Primitives;
using Sledge.Common.Documents;

namespace Sledge.BspEditor.Documents
{
    public class MapDocument : IDocument
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public string Name { get; }
        public object Control { get; }

        public Map Map { get; set; }

        public MapDocument(Map map)
        {
        }
    }
}
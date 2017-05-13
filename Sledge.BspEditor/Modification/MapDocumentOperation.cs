using System.Threading.Tasks;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Documents;

namespace Sledge.BspEditor.Modification
{
    public class MapDocumentOperation
    {
        public MapDocument Document { get; set; }
        public IOperation Operation { get; set; }

        public MapDocumentOperation(MapDocument document, IOperation operation)
        {
            Document = document;
            Operation = operation;
        }

        public static async Task Bypass(MapDocument document, IOperation operation)
        {
            await Oy.Publish("MapDocument:Perform:Bypass", new MapDocumentOperation(document, operation));
        }

        public static async Task Perform(MapDocument document, IOperation operation)
        {
            await Oy.Publish("MapDocument:Perform", new MapDocumentOperation(document, operation));
        }

        public static async Task Reverse(MapDocument document, IOperation operation)
        {
            await Oy.Publish("MapDocument:Reverse", new MapDocumentOperation(document, operation));
        }
    }
}
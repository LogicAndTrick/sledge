using System.Threading.Tasks;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Documents;

namespace Sledge.BspEditor.Modification
{
    /// <summary>
    /// A simple wrapper class to easily handle document operations.
    /// </summary>
    public class MapDocumentOperation
    {
        public MapDocument Document { get; set; }
        public IOperation Operation { get; set; }

        public MapDocumentOperation(MapDocument document, IOperation operation)
        {
            Document = document;
            Operation = operation;
        }

        /// <summary>
        /// Performs an operation without posting to the history handler.
        /// Any operation performed in a bypass cannot be reversed by the user.
        /// Use with caution, only when necessary.
        /// </summary>
        /// <param name="document">The document to operate on</param>
        /// <param name="operation">The operation to perform</param>
        public static async Task Bypass(MapDocument document, IOperation operation)
        {
            await Oy.Publish("MapDocument:Perform:Bypass", new MapDocumentOperation(document, operation));
        }

        /// <summary>
        /// Perform an operation on a document.
        /// </summary>
        /// <param name="document">The document to operate on</param>
        /// <param name="operation">The operation to perform</param>
        public static async Task Perform(MapDocument document, IOperation operation)
        {
            await Oy.Publish("MapDocument:Perform", new MapDocumentOperation(document, operation));
        }

        /// <summary>
        /// Reverse an operation on a document.
        /// The operation being reversed should be the last operation that was performed on the document.
        /// </summary>
        /// <param name="document">The document to operate on</param>
        /// <param name="operation">The operation to reverse</param>
        public static async Task Reverse(MapDocument document, IOperation operation)
        {
            await Oy.Publish("MapDocument:Reverse", new MapDocumentOperation(document, operation));
        }
    }
}
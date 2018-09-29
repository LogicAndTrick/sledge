using System.Threading.Tasks;
using Sledge.BspEditor.Documents;

namespace Sledge.BspEditor.Modification
{
    /// <summary>
    /// An operation is an action that modifies a document.
    /// All changes made on a document should be done via an operation.
    /// </summary>
    public interface IOperation
    {
        /// <summary>
        /// An operation is trivial if it doesn't change the document in any meaningful way.
        /// Trivial operations should not appear in undo/redo stacks.
        /// </summary>
        bool Trivial { get; }

        /// <summary>
        /// Perform the operation. This should only ever be called first, or after Reverse has been called.
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        Task<Change> Perform(MapDocument document);

        /// <summary>
        /// Reverse the operation. This should only ever be called after the operation has been performed.
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        Task<Change> Reverse(MapDocument document);
    }
}
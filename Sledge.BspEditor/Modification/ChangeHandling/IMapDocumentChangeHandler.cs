using System.Threading.Tasks;

namespace Sledge.BspEditor.Modification.ChangeHandling
{
    /// <summary>
    /// Implementations of this class can pre-process a change before it it published.
    /// The change can be modified or other operations can be made and appended to the change.
    /// </summary>
    public interface IMapDocumentChangeHandler
    {
        /// <summary>The order this operation will run in</summary>
        string OrderHint { get; }

        /// <summary>
        /// Process the change.
        /// </summary>
        /// <param name="change">The change being made</param>
        /// <returns>Running task</returns>
        Task Changed(Change change);
    }
}
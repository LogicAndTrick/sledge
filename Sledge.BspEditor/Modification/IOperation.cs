using System.Threading.Tasks;
using Sledge.BspEditor.Documents;

namespace Sledge.BspEditor.Modification
{
    public interface IOperation
    {
        Task<Change> Perform(MapDocument document);
        Task<Change> Reverse(MapDocument document);
    }
}
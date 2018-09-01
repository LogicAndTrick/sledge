using System.Threading.Tasks;

namespace Sledge.BspEditor.Modification
{
    public interface IMapDocumentChangeHandler
    {
        Task Changed(Change change);
    }
}
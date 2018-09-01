using System.Threading.Tasks;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Primitives.MapObjects;

namespace Sledge.BspEditor.Modification.Operations.Data
{
    public class EditEntityDataName : IOperation
    {
        private readonly long _id;
        private readonly string _newName;
        private string _oldName;
        public bool Trivial => false;

        public EditEntityDataName(long id, string newName)
        {
            _id = id;
            _newName = newName;
        }

        public async Task<Change> Perform(MapDocument document)
        {
            var ch = new Change(document);

            var obj = document.Map.Root.FindByID(_id);
            var data = obj?.Data.GetOne<EntityData>();
            if (data != null)
            {
                _oldName = data.Name;
                data.Name = _newName;
                ch.Update(obj);
            }

            return ch;
        }

        public async Task<Change> Reverse(MapDocument document)
        {
            var ch = new Change(document);

            var obj = document.Map.Root.FindByID(_id);
            var data = obj?.Data.GetOne<EntityData>();
            if (data != null && _oldName != null)
            {
                data.Name = _oldName;
                ch.Update(obj);
            }

            return ch;
        }
    }
}
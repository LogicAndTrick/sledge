using System.Collections.Generic;
using System.Threading.Tasks;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common.Transport;

namespace Sledge.BspEditor.Modification.Operations.Data
{
    public class EditEntityDataProperties : IOperation
    {
        private readonly long _id;
        private readonly Dictionary<string, string> _valuesToSet;
        private SerialisedObject _beforeState;
        public bool Trivial => false;

        public EditEntityDataProperties(long id, Dictionary<string, string> valuesToSet)
        {
            _id = id;
            _valuesToSet = valuesToSet;
        }

        public async Task<Change> Perform(MapDocument document)
        {
            var ch = new Change(document);

            var obj = document.Map.Root.FindByID(_id);
            var data = obj?.Data.GetOne<EntityData>();
            if (data != null)
            {
                _beforeState = data.ToSerialisedObject();
                foreach (var kv in _valuesToSet)
                {
                    if (kv.Value == null) data.Properties.Remove(kv.Key);
                    else data.Properties[kv.Key] = kv.Value;
                }
                ch.Update(obj);
            }

            return ch;
        }

        public async Task<Change> Reverse(MapDocument document)
        {
            var ch = new Change(document);

            var obj = document.Map.Root.FindByID(_id);
            if (obj != null && _beforeState != null)
            {
                var ed = new EntityData(_beforeState);
                obj.Data.Replace(ed);
                ch.Update(obj);
            }

            return ch;
        }
    }
}

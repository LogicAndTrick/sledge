using System.Collections.Generic;
using System.Linq;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Documents;

namespace Sledge.Editor.Actions.MapObjects.Visgroups
{
    public class RemoveVisgroup : IAction
    {
        private List<MapObject> _objects;
        private readonly int _visgroupId;

        public RemoveVisgroup(int visgroupId, IEnumerable<MapObject> objects)
        {
            _visgroupId = visgroupId;
            _objects = objects.Where(x => x.Visgroups.Contains(visgroupId)).ToList();
        }

        public void Dispose()
        {
            _objects = null;
        }

        public void Reverse(Document document)
        {
            _objects.ForEach(x => x.Visgroups.Add(_visgroupId));
        }

        public void Perform(Document document)
        {
            _objects.ForEach(x => x.Visgroups.Remove(_visgroupId));
        }
    }
}
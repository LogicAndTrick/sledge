using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common.Shell.Hooks;
using Sledge.Common.Transport;

namespace Sledge.BspEditor.Primitives
{
    [Export]
    public class MapElementFactory
    {
        [ImportMany] private IEnumerable<Lazy<IMapElementFormatter>> _imports;
        
        public IMapElement Deserialise(SerialisedObject obj)
        {
            var elem = _imports.Select(x => x.Value).FirstOrDefault(x => x.IsSupported(obj))?.Deserialise(obj);

            var mo = elem as IMapObject;
            if (mo != null)
            {
                foreach (var so in obj.Children)
                {
                    var data = Deserialise(so);
                    if (data is IMapObject) ((IMapObject) data).Hierarchy.Parent = mo;
                    else if (data is IMapObjectData) mo.Data.Add((IMapObjectData) data);
                }
            }

            return elem;
        }

        public SerialisedObject Serialise(IMapElement element)
        {
            return _imports.Select(x => x.Value).FirstOrDefault(x => x.IsSupported(element))?.Serialise(element);
        }
    }
}

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
    [Export(typeof(IInitialiseHook))]
    [Export]
    public class MapElementFactory : IInitialiseHook
    {
        [ImportMany] private IEnumerable<Lazy<IMapElementFormatter>> _imports;

        public Task OnInitialise()
        {
            _formatters.AddRange(_imports.Select(x => x.Value));
            return Task.FromResult(0);
        }

        private readonly List<IMapElementFormatter> _formatters;

        public MapElementFactory()
        {
            _formatters = new List<IMapElementFormatter>();
        }

        public IMapElement Deserialise(SerialisedObject obj)
        {
            var elem = _formatters.FirstOrDefault(x => x.IsSupported(obj))?.Deserialise(obj);

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
            return _formatters.FirstOrDefault(x => x.IsSupported(element))?.Serialise(element);
        }
    }
}

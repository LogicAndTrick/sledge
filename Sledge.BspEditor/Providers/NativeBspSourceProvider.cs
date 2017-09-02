using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Sledge.BspEditor.Primitives;
using Sledge.BspEditor.Primitives.MapData;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common.Shell.Documents;
using Sledge.Common.Transport;

namespace Sledge.BspEditor.Providers
{
    [Export(typeof(IBspSourceProvider))]
    public class NativeBspSourceProvider : IBspSourceProvider
    {
        [Import] private SerialisedObjectFormatter _formatter;
        [Import] private MapElementFactory _factory;

        private static readonly IEnumerable<Type> SupportedTypes = new List<Type>
        {
            // Just everything
            typeof(IMapObject),
            typeof(IMapObjectData),
            typeof(IMapData),
        };

        public IEnumerable<Type> SupportedDataTypes => SupportedTypes;

        public IEnumerable<FileExtensionInfo> SupportedFileExtensions { get; } = new[]
        {
            new FileExtensionInfo("Sledge map format", ".smf"), 
        };

        public async Task<Map> Load(Stream stream)
        {
            return await Task.Factory.StartNew(() =>
            {
                var map = new Map();
                var so = _formatter.Deserialize(stream);
                foreach (var o in so)
                {
                    if (o.Name == nameof(Root))
                    {
                        map.Root.Unclone((Root) _factory.Deserialise(o));
                    }
                    else
                    {
                        map.Data.Add((IMapData) _factory.Deserialise(o));
                    }
                }
                map.Root.DescendantsChanged();
                return map;
            });
        }
        
        public Task Save(Stream stream, Map map)
        {
            return Task.Factory.StartNew(() =>
            {
                var list = new List<SerialisedObject>
                {
                    _factory.Serialise(map.Root)
                };
                list.AddRange(map.Data.Select(_factory.Serialise).Where(x => x != null));
                _formatter.Serialize(stream, list);
            });
        }
    }
}
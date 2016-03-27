using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Sledge.DataStructures.MapObjects;

namespace Sledge.Providers.Map
{
    public class JmfProvider : MapProvider
    {
        protected override IEnumerable<MapFeature> GetFormatFeatures()
        {
            return new[]
            {
                MapFeature.Worldspawn,
                MapFeature.Solids,
                MapFeature.Entities,
                MapFeature.Groups,

                MapFeature.Colours,
                MapFeature.SingleVisgroups,
                MapFeature.Cameras
            };
            // todo: what features does JMF support?
        }

        protected override bool IsValidForFileName(string filename)
        {
            return filename.EndsWith(".jmf", true, CultureInfo.InvariantCulture)
                || filename.EndsWith(".jmx", true, CultureInfo.InvariantCulture);
        }

        protected override DataStructures.MapObjects.Map GetFromStream(Stream stream)
        {
            throw new NotImplementedException();
        }

        protected override void SaveToStream(Stream stream, DataStructures.MapObjects.Map map)
        {
            throw new NotImplementedException();
        }
    }
}

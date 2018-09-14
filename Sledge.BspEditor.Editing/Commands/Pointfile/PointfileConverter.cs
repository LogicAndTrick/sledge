using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Numerics;
using System.Threading.Tasks;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.BspEditor.Rendering.Converters;
using Sledge.BspEditor.Rendering.Resources;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Pipelines;
using Sledge.Rendering.Primitives;
using Sledge.Rendering.Resources;

namespace Sledge.BspEditor.Editing.Commands.Pointfile
{
    [Export(typeof(IMapObjectSceneConverter))]
    public class PointfileConverter : IMapObjectSceneConverter
    {
        public MapObjectSceneConverterPriority Priority => MapObjectSceneConverterPriority.OverrideLow;
        
        private Pointfile GetPointfile(MapDocument doc)
        {
            return doc.Map.Data.GetOne<Pointfile>();
        }

        public bool ShouldStopProcessing(MapDocument document, IMapObject obj)
        {
            return false;
        }

        public bool Supports(IMapObject obj)
        {
            return obj is Root;
        }

        public Task Convert(BufferBuilder builder, MapDocument document, IMapObject obj,
            ResourceCollector resourceCollector)
        {
            var pointfile = GetPointfile(document);
            if (pointfile == null) return Task.FromResult(0);

            var r = 1f;
            var g = 0.5f;
            var b = 0.5f;
            var change = 0.5f / pointfile.Lines.Count;

            var verts = new List<VertexStandard>();
            var index = new List<uint>();
            
            for (var i = 0; i < pointfile.Lines.Count; i++)
            {
                var line = pointfile.Lines[i];

                index.Add((uint) index.Count + 0);
                index.Add((uint) index.Count + 1);

                verts.Add(new VertexStandard
                {
                    Position = line.Start,
                    Colour = new Vector4(r, g, b, 1),
                    Tint = Vector4.One
                });
                verts.Add(new VertexStandard
                {
                    Position = line.End,
                    Colour = new Vector4(r, g, b, 1),
                    Tint = Vector4.One
                });
            
                r = 1f - (change * i);
                b = 0.5f + (change * i);
            }

            builder.Append(verts, index, new []
            {
                new BufferGroup(PipelineType.Wireframe, CameraType.Both, 0, (uint) index.Count)
            });

            return Task.FromResult(0);
        }
    }
}
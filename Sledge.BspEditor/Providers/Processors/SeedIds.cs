using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Primitives.MapObjects;

namespace Sledge.BspEditor.Providers.Processors
{
    /// <summary>
    /// Ensure that the number generator is seeded with the maximum face and object ids in the map
    /// </summary>
    [Export(typeof(IBspSourceProcessor))]
    public class SeedIds : IBspSourceProcessor
    {
        public string OrderHint => "A";

        public Task AfterLoad(MapDocument document)
        {
            long maxMapObject = 0;
            long maxFace = 0;
            foreach (var o in document.Map.Root.FindAll())
            {
                if (o.ID > maxMapObject) maxMapObject = o.ID;
                foreach (var face in o.Data.OfType<Face>())
                {
                    if (face.ID > maxFace) maxFace = face.ID;
                }
            }

            document.Map.NumberGenerator.Seed("MapObject", maxMapObject);
            document.Map.NumberGenerator.Seed("Face", maxFace);

            return Task.FromResult(0);
        }

        public Task BeforeSave(MapDocument document)
        {
            return Task.FromResult(0);
        }
    }
}
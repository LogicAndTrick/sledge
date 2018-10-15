using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Modification.Operations.Data;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common.Translations;

namespace Sledge.BspEditor.Editing.Problems
{
    [Export(typeof(IProblemCheck))]
    [AutoTranslate]
    public class TextureNotFound : IProblemCheck
    {
        public string Name { get; set; }
        public string Details { get; set; }
        public Uri Url => null;
        public bool CanFix => true;

        public async Task<List<Problem>> Check(MapDocument document, Predicate<IMapObject> filter)
        {
            var tc = await document.Environment.GetTextureCollection();

            // Get a list of all faces and textures
            var faces = document.Map.Root.FindAll()
                .OfType<Solid>()
                .Where(x => filter(x))
                .SelectMany(x => x.Faces.Select(f => new {Object = x, Face = f}))
                .ToList();

            // Get the list of textures in the map and in the texture collection
            var textureNames = faces.Select(x => x.Face.Texture.Name).ToHashSet(StringComparer.InvariantCultureIgnoreCase);
            var knownTextureNames = tc.GetAllTextures().ToHashSet(StringComparer.InvariantCultureIgnoreCase);

            // The set only contains textures that aren't known
            textureNames.ExceptWith(knownTextureNames);

            return faces
                .Where(x => textureNames.Contains(x.Face.Texture.Name))
                .Select(x => new Problem {Text = x.Face.Texture.Name}.Add(x.Object).Add(x.Face))
                .ToList();
        }

        public async Task Fix(MapDocument document, Problem problem)
        {
            var tc = await document.Environment.GetTextureCollection();

            // Get the default texture to apply
            var first = tc.GetBrowsableTextures()
                .OrderBy(t => t, StringComparer.CurrentCultureIgnoreCase)
                .Where(item => item.Length > 0)
                .Select(item => new { item, c = Char.ToLower(item[0]) })
                .Where(t => t.c >= 'a' && t.c <= 'z')
                .Select(t => t.item)
                .FirstOrDefault();

            var transaction = new Transaction();

            foreach (var obj in problem.Objects)
            {
                foreach (var face in obj.Data.Intersect(problem.ObjectData).OfType<Face>())
                {
                    var clone = (Face)face.Clone();
                    clone.Texture.Name = first;

                    transaction.Add(new RemoveMapObjectData(obj.ID, face));
                    transaction.Add(new AddMapObjectData(obj.ID, clone));
                }
            }

            await MapDocumentOperation.Perform(document, transaction);
        }
    }
}
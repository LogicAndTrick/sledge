using System.Collections.Generic;
using System.Linq;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Actions;
using Sledge.Editor.Actions.MapObjects.Operations;
using Sledge.Providers.Texture;

namespace Sledge.Editor.Problems
{
    public class TextureNotFound : IProblemCheck
    {
        public IEnumerable<Problem> Check(Map map)
        {
            var faces = map.WorldSpawn
                .Find(x => x is Solid).OfType<Solid>()
                .SelectMany(x => x.Faces)
                .Where(x => x.Texture.Texture == null)
                .ToList();
            foreach (var name in faces.Select(x => x.Texture.Name).Distinct())
            {
                yield return new Problem(GetType(), map, faces.Where(x => x.Texture.Name == name).ToList(), Fix, "Texture not found: " + name, "This texture was not found in the currently loaded texture packages. Ensure that the correct texture packages are loaded. Fixing the problems will reset the face textures to the default texture.");
            }
        }

        public IAction Fix(Problem problem)
        {/*
            var ignored = "{#!~+-0123456789".ToCharArray();
            var def = TexturePackage.GetLoadedItems()
                .OrderBy(x => new string(x.Name.Where(c => !ignored.Contains(c)).ToArray()) + "Z")
                .FirstOrDefault();
            return new EditFace(problem.Faces, (d, x) =>
                                                   {
                                                       if (def != null)
                                                       {
                                                           x.Texture.Name = def.Name;
                                                           x.Texture.Texture = def.GetTexture();
                                                           x.CalculateTextureCoordinates();
                                                       }
                                                   }, true);*/
            return null;
        }
    }
}
namespace Sledge.BspEditor.Editing.Problems
{
    /* todo fix this texture check
    public class TextureNotFound : IProblemCheck
    {
        public IEnumerable<Problem> Check(Map map, bool visibleOnly)
        {
            var faces = map.WorldSpawn
                .Find(x => x is Solid && (!visibleOnly || (!x.IsVisgroupHidden && !x.IsCodeHidden)))
                .OfType<Solid>()
                .SelectMany(x => x.Faces)
                .Where(x => x.Texture.Size.IsEmpty)
                .ToList();
            foreach (var name in faces.Select(x => x.Texture.Name).Distinct())
            {
                yield return new Problem(GetType(), map, faces.Where(x => x.Texture.Name == name).ToList(), Fix, "Texture not found: " + name, "This texture was not found in the currently loaded texture packages. Ensure that the correct texture packages are loaded. Fixing the problems will reset the face textures to the default texture.");
            }
        }

        public IOperation Fix(Problem problem)
        {
            return new EditFace(problem.Faces, (d, x) =>
                                                   {
                                                       var ignored = "{#!~+-0123456789".ToCharArray();
                                                       var def = d.TextureCollection.GetAllBrowsableItems()
                                                           .OrderBy(i => new string(i.Name.Where(c => !ignored.Contains(c)).ToArray()) + "Z")
                                                           .FirstOrDefault();
                                                       if (def != null)
                                                       {
                                                           x.Texture.Name = def.Name;
                                                           x.Texture.Size = new Size(def.Width, def.Height);
                                                           x.CalculateTextureCoordinates(true);
                                                       }
                                                   }, true);
        }
    }
    */
}
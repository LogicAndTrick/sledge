using System.Linq;
using System.Numerics;
using Sledge.BspEditor.Primitives;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.DataStructures.Geometric;
using Path = System.IO.Path;

namespace Sledge.BspEditor.Documents
{
    /// <summary>
    /// Common extensions for map documents
    /// </summary>
    public static class MapDocumentExtensions
    {
        /// <summary>
        /// Clone a document, retaining only objects contained within a cordon area, and create a box around the cordon bounds to seal the level.
        /// </summary>
        /// <param name="doc">This document</param>
        /// <param name="cordonBounds">The cordon bounds</param>
        /// <param name="cordonTextureName">The name of the texture to use for the sealing box</param>
        /// <returns>A cloned document</returns>
        public static MapDocument CloneWithCordon(this MapDocument doc, Box cordonBounds, string cordonTextureName)
        {
            // If we're exporting cordon then we need to ensure that only objects in the bounds are exported.
            // Additionally a surrounding box needs to be added to enclose the map.
            var cloneMap = new Map();

            // Copy the map data
            cloneMap.Data.Clear();
            cloneMap.Data.AddRange(doc.Map.Data.Copy(cloneMap.NumberGenerator));

            // Copy the root data
            cloneMap.Root.Data.Clear();
            cloneMap.Root.Data.AddRange(doc.Map.Root.Data.Copy(cloneMap.NumberGenerator));

            // Add copies of all the matching child objects (and their children, etc)
            cloneMap.Root.Hierarchy.Clear();
            foreach (var obj in doc.Map.Root.Hierarchy.Where(x => x.BoundingBox.IntersectsWith(cordonBounds)))
            {
                var copy = (IMapObject)obj.Copy(cloneMap.NumberGenerator);
                copy.Hierarchy.Parent = cloneMap.Root;
            }

            // Add a hollow box around the cordon bounds
            var outside = new Box(cloneMap.Root.Hierarchy.Select(x => x.BoundingBox).Union(new[] { cordonBounds }));
            outside = new Box(outside.Start - Vector3.One * 10, outside.End + Vector3.One * 10);
            var inside = cordonBounds;

            var outsideBox = new Solid(cloneMap.NumberGenerator.Next("MapObject"));
            foreach (var arr in outside.GetBoxFaces())
            {
                var face = new Face(cloneMap.NumberGenerator.Next("Face"))
                {
                    Plane = new DataStructures.Geometric.Plane(arr[0], arr[1], arr[2]),
                    Texture = { Name = cordonTextureName }
                };
                face.Vertices.AddRange(arr.Select(x => x.Round(0)));
                outsideBox.Data.Add(face);
            }

            outsideBox.DescendantsChanged();

            var insideBox = new Solid(cloneMap.NumberGenerator.Next("MapObject"));
            foreach (var arr in inside.GetBoxFaces())
            {
                var face = new Face(cloneMap.NumberGenerator.Next("Face"))
                {
                    Plane = new DataStructures.Geometric.Plane(arr[0], arr[1], arr[2]),
                    Texture = { Name = cordonTextureName }
                };
                face.Vertices.AddRange(arr.Select(x => x.Round(0)));
                insideBox.Data.Add(face);
            }

            insideBox.DescendantsChanged();

            // Carve the inside box into the outside box and add the front solids to the map
            foreach (var face in insideBox.Faces)
            {
                // Carve the box
                if (!outsideBox.Split(cloneMap.NumberGenerator, face.Plane, out var back, out var front)) continue;

                // Align texture to face
                foreach (var f in front.Faces)
                {
                    f.Texture.XScale = f.Texture.YScale = 1;
                    f.Texture.AlignToNormal(f.Plane.Normal);
                }

                // Add to map
                front.Hierarchy.Parent = cloneMap.Root;

                // Continue carving
                outsideBox = back;
            }

            // Now we're ready to export this map
            return new MapDocument(cloneMap, doc.Environment)
            {
                FileName = Path.GetFileName(doc.FileName),
                Name = doc.Name
            };
        }
    }
}

using System;
using System.Collections.Generic;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Modification;
using Sledge.Common;
using Sledge.BspEditor.Primitives.MapObjects;
using System.Linq;

namespace Sledge.BspEditor.Editing.Problems
{
    public class TextureAxisPerpendicularToFace : IProblemCheck
    {
        public IEnumerable<Problem> Check(MapDocument document, bool visibleOnly)
        {
            var faces = document.Map.Root
                .Find(x => x is Solid) // todo && (!visibleOnly || (!x.IsVisgroupHidden && !x.IsCodeHidden)))
                .OfType<Solid>()
                .SelectMany(x => x.Faces)
                .ToList();
            foreach (var face in faces)
            {
                var normal = face.Texture.GetNormal();
                if (DMath.Abs(face.Plane.Normal.Dot(normal)) <= 0.0001m) yield return new Problem(GetType(), document, new [] { face }, Fix, "Texture axis perpendicular to face", "The texture axis of this face is perpendicular to the face plane. This occurs when manipulating objects with texture lock off, as well as various other operations. Re-align the texture to the face to repair. Fixing the problem will reset the textures to the face plane.");
            }
        }

        public IOperation Fix(Problem problem)
        {
            // todo
            throw new NotImplementedException();
            // return new EditFace(problem.Faces, (d,x) => x.AlignTextureToFace(), false);
        }
    }
}
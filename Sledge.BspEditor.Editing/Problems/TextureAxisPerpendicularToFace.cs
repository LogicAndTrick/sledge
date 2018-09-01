using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Primitives.MapObjects;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Sledge.BspEditor.Modification.Operations.Data;
using Sledge.BspEditor.Primitives;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.Common.Translations;

namespace Sledge.BspEditor.Editing.Problems
{
    [Export(typeof(IProblemCheck))]
    [AutoTranslate]
    public class TextureAxisPerpendicularToFace : IProblemCheck
    {
        public string Name { get; set; }
        public string Details { get; set; }

        public Uri Url => new Uri("https://twhl.info/wiki/page/Error%3A_Texture_axis_perpendicular_to_face");
        public bool CanFix => true;

        public Task<List<Problem>> Check(MapDocument document, Predicate<IMapObject> filter)
        {
            var list = new List<Problem>();

            var solids = document.Map.Root.Find(x => x is Solid).OfType<Solid>().Where(x => filter(x));
            foreach (var solid in solids)
            {
                var perps = (
                    from face in solid.Faces
                    let normal = face.Texture.GetNormal()
                    where Math.Abs(Vector3.Dot(face.Plane.Normal, normal)) <= 0.0001
                    select face
                ).ToList();
                if (perps.Any()) list.Add(new Problem().Add(solid).Add(perps));
            }

            return Task.FromResult(list);
        }

        public Task Fix(MapDocument document, Problem problem)
        {
            var edit = new Transaction();

            var obj = problem.Objects[0];
            foreach (var face in problem.ObjectData.OfType<Face>())
            {
                var clone = (Face) face.Clone();
                clone.Texture.AlignToNormal(face.Plane.Normal);

                edit.Add(new RemoveMapObjectData(obj.ID, face));
                edit.Add(new AddMapObjectData(obj.ID, clone));
            }

            return MapDocumentOperation.Perform(document, edit);
        }
    }
}
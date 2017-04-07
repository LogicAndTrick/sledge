using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.DataStructures.Geometric;

namespace Sledge.BspEditor.Primitives.MapObjects
{
    /// <summary>
    /// A collection of faces
    /// </summary>
    public class Solid : BaseMapObject, IPrimitive
    {
        public IEnumerable<Face> Faces => Data.Get<Face>();
        public ObjectColor Color => Data.GetOne<ObjectColor>();

        public Solid(long id) : base(id)
        {
        }

        protected override Box GetBoundingBox()
        {
            var faces = Faces.ToList();
            return faces.Any(x => x.Vertices.Count > 0) ? new Box(faces.SelectMany(x => x.Vertices)) : Box.Empty;
        }

        public override IMapObject Clone()
        {
            var solid = new Solid(ID);
            CloneBase(solid);
            return solid;
        }

        public override void Unclone(IMapObject obj)
        {
            if (!(obj is Solid)) throw new ArgumentException("Cannot unclone into a different type.", nameof(obj));
            UncloneBase((BaseMapObject)obj);
        }

        public override IEnumerable<IPrimitive> ToPrimitives()
        {
            yield return this;
        }
    }
}
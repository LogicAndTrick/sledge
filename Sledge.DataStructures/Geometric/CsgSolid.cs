using System.Collections.Generic;
using System.Linq;

namespace Sledge.DataStructures.Geometric
{
    // Ported from: https://github.com/evanw/csg.js/
    // Copyright (c) 2011 Evan Wallace (http://madebyevan.com/)
    // MIT license
    public class CsgSolid
    {
        public List<Polygon> Polygons { get; private set; }

        public CsgSolid(IEnumerable<Polygon> polygons)
        {
            Polygons = polygons.ToList();
        }

        public CsgSolid()
        {
            Polygons = new List<Polygon>();
        }

        public CsgSolid Union(CsgSolid solid)
        {
            var a = new CsgNode(this);
            var b = new CsgNode(solid);
            a.ClipTo(b);
            b.ClipTo(a);
            b.Invert();
            b.ClipTo(a);
            b.Invert();
            a.Build(b.AllPolygons());
            return new CsgSolid(a.AllPolygons());
        }

        public CsgSolid Subtract(CsgSolid solid)
        {
            var a = new CsgNode(this);
            var b = new CsgNode(solid);
            a.Invert();
            a.ClipTo(b);
            b.ClipTo(a);
            b.Invert();
            b.ClipTo(a);
            b.Invert();
            a.Build(b.AllPolygons());
            a.Invert();
            return new CsgSolid(a.AllPolygons());
        }

        public CsgSolid Intersect(CsgSolid solid)
        {
            var a = new CsgNode(this);
            var b = new CsgNode(solid);
            a.Invert();
            b.ClipTo(a);
            b.Invert();
            a.ClipTo(b);
            b.ClipTo(a);
            a.Build(b.AllPolygons());
            a.Invert();
            return new CsgSolid(a.AllPolygons());
        }
    }
}

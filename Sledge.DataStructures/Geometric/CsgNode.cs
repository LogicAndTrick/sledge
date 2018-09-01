using System.Collections.Generic;
using System.Linq;

namespace Sledge.DataStructures.Geometric
{
    // Ported from: https://github.com/evanw/csg.js/
    // Copyright (c) 2011 Evan Wallace (http://madebyevan.com/)
    // MIT license
    public class CsgNode
    {
        private List<Polygon> Polygons { get; set; }

        private Plane Plane { get; set; }
        private CsgNode Front { get; set; }
        private CsgNode Back { get; set; }

        public CsgNode(CsgSolid solid) : this()
        {
            Build(solid.Polygons.ToList());
        }

        private CsgNode()
        {
            Polygons = new List<Polygon>();
            Front = null;
            Back = null;
        }

        private List<Polygon> ClipPolygons(IEnumerable<Polygon> polygons)
        {
            if (Plane == null) return polygons.ToList();
            var pp = Plane.ToPrecisionPlane();
            var front = new List<Polygon>();
            var back = new List<Polygon>();
            foreach (var polygon in polygons.Select(x => x.ToPrecisionPolygon()))
            {
                polygon.Split(pp, out var b, out var f, out var cb, out var cf);
                if (f != null) front.Add(f.ToStandardPolygon());
                if (b != null) back.Add(b.ToStandardPolygon());
                if (cf != null) front.Add(cf.ToStandardPolygon());
                if (cb != null) back.Add(cb.ToStandardPolygon());
            }
            if (Front != null) front = Front.ClipPolygons(front);
            back = Back != null ? Back.ClipPolygons(back) : new List<Polygon>();
            return front.Concat(back).ToList();
        }

        public void ClipTo(CsgNode bsp)
        {
            Polygons = bsp.ClipPolygons(Polygons);
            if (Front != null) Front.ClipTo(bsp);
            if (Back != null) Back.ClipTo(bsp);
        }

        public void Invert()
        {
            Polygons = Polygons.Select(x => x.Flip()).ToList();
            Plane = new Plane(-Plane.Normal, Plane.PointOnPlane);
            if (Front != null) Front.Invert();
            if (Back != null) Back.Invert();
            var temp = Front;
            Front = Back;
            Back = temp;
        }

        public List<Polygon> AllPolygons()
        {
            var polygons = Polygons.ToList();
            if (Front != null) polygons.AddRange(Front.AllPolygons());
            if (Back != null) polygons.AddRange(Back.AllPolygons());
            return polygons;
        }

        public void Build(List<Polygon> polygons)
        {
            if (polygons.Count == 0) return;
            if (Plane == null) Plane = polygons[0].Plane.Clone();
            var pp = Plane.ToPrecisionPlane();
            var front = new List<Polygon>();
            var back = new List<Polygon>();
            foreach (var polygon in polygons.Select(x => x.ToPrecisionPolygon()))
            {
                polygon.Split(pp, out var b, out var f, out var cb, out var cf);
                if (f != null) front.Add(f.ToStandardPolygon());
                if (b != null) back.Add(b.ToStandardPolygon());
                if (cf != null) front.Add(cf.ToStandardPolygon());
                if (cb != null) back.Add(cb.ToStandardPolygon());
            }
            if (front.Count > 0)
            {
                if (Front == null) Front = new CsgNode();
                Front.Build(front);
            }
            if (back.Count > 0)
            {
                if (Back == null) Back = new CsgNode();
                Back.Build(back);
            }
        }
    }
}
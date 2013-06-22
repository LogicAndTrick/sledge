using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Brushes;

namespace Sledge.Tests
{
    [TestClass]
    public class PlaneIntersectionTest
    {
        [TestMethod]
        public void TestPlaneIntersection()
        {
            var planes = new[]
                             {
                                 new Plane(new Coordinate(-64, 64, 64), new Coordinate(64, 64, 64), new Coordinate(64, -64, 64)),
                                 new Plane(new Coordinate(-64, -64, -64), new Coordinate(64, -64, -64), new Coordinate(64, 64, -64)),
                                 new Plane(new Coordinate(-64, 64, 64), new Coordinate(-64, -64, 64), new Coordinate(-64, -64, -64)),
                                 new Plane(new Coordinate(64, 64, -64), new Coordinate(64, -64, -64), new Coordinate(64, -64, 64)),
                                 new Plane(new Coordinate(64, 64, 64), new Coordinate(-64, 64, 64), new Coordinate(-64, 64, -64)),
                                 new Plane(new Coordinate(64, -64, -64), new Coordinate(-64, -64, -64), new Coordinate(-64, -64, 64))
                             };
            var solid = Solid.CreateFromIntersectingPlanes(planes, new IDGenerator());

            Assert.AreEqual(6, solid.Faces.Count);
        }

        [TestMethod]
        public void TestPolygonSplitting()
        {
            var planes = new[]
                             {
                                 new Plane(new Coordinate(-64, 64, 64), new Coordinate(64, 64, 64), new Coordinate(64, -64, 64)),
                                 new Plane(new Coordinate(-64, -64, -64), new Coordinate(64, -64, -64), new Coordinate(64, 64, -64)),
                                 new Plane(new Coordinate(-64, 64, 64), new Coordinate(-64, -64, 64), new Coordinate(-64, -64, -64)),
                                 new Plane(new Coordinate(64, 64, -64), new Coordinate(64, -64, -64), new Coordinate(64, -64, 64)),
                                 new Plane(new Coordinate(64, 64, 64), new Coordinate(-64, 64, 64), new Coordinate(-64, 64, -64)),
                                 new Plane(new Coordinate(64, -64, -64), new Coordinate(-64, -64, -64), new Coordinate(-64, -64, 64))
                             }.ToList();
            var polys = new List<Polygon>();
            for (var i = 0; i < planes.Count; i++)
            {
                var poly = new Polygon(planes[i]);
                for (var j = 0; j < planes.Count; j++)
                {
                    if (i != j) poly.Split(planes[j]);
                }
                polys.Add(poly);
            }
            Assert.AreEqual(6, polys.Count);
        }

        [TestMethod]
        public void BenchmarkSolidConstruction()
        {
            var idg = new IDGenerator();
            var box = new Box(Coordinate.One * -100, Coordinate.One * 100);
            var planes = new CylinderBrush().Create(idg, box, null).OfType<Solid>().SelectMany(x => x.Faces).Select(x => x.Plane).ToList();
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            for (var b = 0; b < 1000; b++)
            {
                Solid.CreateFromIntersectingPlanes(planes, idg);
            }
            stopwatch.Stop();
            Debug.WriteLine(stopwatch.Elapsed);
            stopwatch.Restart();
            for (var b = 0; b < 1000; b++)
            {
                var polys = new List<Polygon>();
                for (var i = 0; i < planes.Count; i++)
                {
                    var poly = new Polygon(planes[i]);
                    for (var j = 0; j < planes.Count; j++)
                    {
                        if (i != j) poly.Split(planes[j]);
                    }
                    polys.Add(poly);
                }
                var solid = new Solid(idg.GetNextObjectID());
                foreach (var polygon in polys)
                {
                    var face = new Face(idg.GetNextFaceID()) {Plane = polygon.Plane};
                    face.Vertices.AddRange(polygon.Vertices.Select(x => new Vertex(x, face)));
                    face.UpdateBoundingBox();
                    face.AlignTextureToWorld();
                    solid.Faces.Add(face);
                }
                solid.UpdateBoundingBox();
            }
            stopwatch.Stop();
            Debug.WriteLine(stopwatch.Elapsed);
        }
    }
}

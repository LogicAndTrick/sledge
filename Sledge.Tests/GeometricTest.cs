using System.Linq;
using Sledge.DataStructures.Geometric;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Sledge.DataStructures.MapObjects;

namespace Sledge.Tests
{
    [TestClass()]
    public class GeometricTest
    {
        [TestMethod()]
        public void PlaneLineIntersectionTest()
        {
            var plane = new Plane(new Coordinate(0, 0, 1), 100);

            var passLine = new Line(new Coordinate(0, 0, 0), new Coordinate(0, 0, 200));
            var reversePassLine = passLine.Reverse();
            var failSegment = new Line(new Coordinate(0, 0, 0), new Coordinate(0, 0, 50));
            var failLine = new Line(new Coordinate(0, 0, 0), new Coordinate(1, 0, 0));

            var pass1 = plane.GetIntersectionPoint(passLine);
            var pass2 = plane.GetIntersectionPoint(reversePassLine, true);
            var pass3 = plane.GetIntersectionPoint(failSegment, false, true);

            var fail1 = plane.GetIntersectionPoint(reversePassLine);
            var fail2 = plane.GetIntersectionPoint(failSegment);
            var fail3 = plane.GetIntersectionPoint(failLine);

            Assert.IsNotNull(pass1);
            Assert.IsNotNull(pass2);
            Assert.IsNotNull(pass3);
            Assert.IsNull(fail1);
            Assert.IsNull(fail2);
            Assert.IsNull(fail3);
        }

        [TestMethod()]
        public void PlaneConstructionTest()
        {
            var p1 = new Coordinate(-100, -100, 100);
            var p2 = new Coordinate(100, -100, 100);
            var p3 = new Coordinate(100, 100, 100);
            var p4 = new Coordinate(0, 0, 0);
            var refPlane = new Plane(new Coordinate(0, 0, 1), 100);
            var plane = new Plane(p1, p2, p3);

            var o1 = refPlane.OnPlane(p1);
            var o2 = plane.OnPlane(p1);
            var o3 = refPlane.OnPlane(p4);
            var o4 = plane.OnPlane(p4);

            Assert.IsTrue(o1 == 0);
            Assert.IsTrue(o2 == 0);
            Assert.IsFalse(o3 == 0);
            Assert.IsFalse(o4 == 0);

            Assert.AreEqual(refPlane.A, plane.A);
            Assert.AreEqual(refPlane.B, plane.B);
            Assert.AreEqual(refPlane.C, plane.C);
            Assert.AreEqual(refPlane.D, plane.D);

            var plane2 = new Plane(new Coordinate(-192, 704, 192),
                                   new Coordinate(-192, 320, 192),
                                   new Coordinate(-192, 320, -192));
        }

        [TestMethod()]
        public void FaceLineIntersectionTest()
        {
            var plane = new Plane(new Coordinate(0, 0, 1), 100);
            var face = new Face(1) {Plane = plane};
            var coords = new[]
                             {
                                 new Coordinate(-100, -100, 100),
                                 new Coordinate(100, -100, 100),
                                 new Coordinate(100, 100, 100),
                                 new Coordinate(-100, 100, 100)
                             };
            face.Vertices.AddRange(coords.Select(x => new Vertex(x, face)));
            face.CalculateTextureCoordinates();

            var passLine = new Line(new Coordinate(0, 0, 0), new Coordinate(0, 0, 200));
            var reversePassLine = passLine.Reverse();
            var failSegment = new Line(new Coordinate(0, 0, 0), new Coordinate(0, 0, 50));
            var failLine = new Line(new Coordinate(0, 0, 0), new Coordinate(1, 0, 0));
            var outsideFaceLine = new Line(new Coordinate(200, 0, 0), new Coordinate(200, 0, 200));

            var pass1 = face.GetIntersectionPoint(passLine);

            var fail1 = face.GetIntersectionPoint(reversePassLine);
            var fail2 = face.GetIntersectionPoint(failSegment);
            var fail3 = face.GetIntersectionPoint(failLine);
            var fail4 = face.GetIntersectionPoint(outsideFaceLine);

            Assert.IsNotNull(pass1);
            Assert.IsNull(fail1);
            Assert.IsNull(fail2);
            Assert.IsNull(fail3);
            Assert.IsNull(fail4);
        }
    }
}

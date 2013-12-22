using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sledge.DataStructures.Geometric;
using Sledge.Extensions;

namespace Sledge.Tests
{
    [TestClass]
    public class AnglesTest
    {


        [TestMethod]
        public void TestAngles1()
        {
            var c = Coordinate.UnitX;
            var euler = Coordinate.Zero;
            var tform = Matrix.RotationZ(DMath.DegreesToRadians(90));

            var start = Quaternion.EulerAngles(euler).Rotate(c);
            var end = start * tform;
            Assert.AreEqual(Coordinate.UnitY, end);

            var finish = end.ToEulerAngles() * (180 / DMath.PI);
            finish = finish.Round(0);
            Assert.AreEqual(new Coordinate(0, 0, 90), finish);
        }

        [TestMethod]
        public void TestAngles2()
        {
            var c = Coordinate.UnitX;
            var euler = Coordinate.Zero;
            var tform = Matrix.RotationY(DMath.DegreesToRadians(-90));

            var start = Quaternion.EulerAngles(euler).Rotate(c);
            var end = start * tform;
            Assert.AreEqual(Coordinate.UnitZ, end);

            var finish = end.ToEulerAngles() * (180 / DMath.PI);
            finish = finish.Round(0);
            Assert.AreEqual(new Coordinate(0, -90, 0), finish);
        }

        [TestMethod]
        public void TestAngles3()
        {
            var c = Coordinate.UnitX;
            var euler = Coordinate.Zero;
            var tform = Matrix.RotationX(DMath.DegreesToRadians(90));

            var start = Quaternion.EulerAngles(euler).Rotate(c);
            var end = start * tform;
            Assert.AreEqual(Coordinate.UnitX, end);

            var finish = end.ToEulerAngles() * (180 / DMath.PI);
            finish = finish.Round(0);
            Assert.AreEqual(new Coordinate(0, -90, 0), finish);
        }
    }
}

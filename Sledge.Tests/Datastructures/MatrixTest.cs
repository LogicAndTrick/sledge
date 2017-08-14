using System;
using System.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sledge.DataStructures.Geometric;

namespace Sledge.Tests.Datastructures
{
    [TestClass]
    public class MatrixTest
    {
        [TestMethod]
        public void TestConvert()
        {
            var mat = Matrix.Rotation(Coordinate.UnitX, 0.15m).Translate(new Coordinate(100, 200, 300));
            var matOtk = Matrix.FromOpenTKMatrix4(mat.ToOpenTKMatrix4());
            
            CollectionAssert.AreEqual(matOtk.Values, mat.Values, new FuzzyDecimalComparer());
        }

        [TestMethod]
        public void TestInverse()
        {
            var mat = Matrix.Rotation(Coordinate.UnitX, 0.15m).Translate(new Coordinate(100, 200, 300));
            var invertOtk = Matrix.FromOpenTKMatrix4(mat.ToOpenTKMatrix4().Inverted());
            var inv = mat.Inverse();
            
            CollectionAssert.AreEqual(invertOtk.Values, inv.Values, new FuzzyDecimalComparer());
        }

        private class FuzzyDecimalComparer : IComparer
        {
            private readonly decimal _delta;

            public FuzzyDecimalComparer(decimal delta = 0.0001m)
            {
                _delta = delta;
            }

            public int Compare(object x, object y)
            {
                var d1 = (decimal) x;
                var d2 = (decimal) y;
                return Math.Abs(d1 - d2) < _delta ? 0 : d1.CompareTo(d2);
            }
        }
    }
}

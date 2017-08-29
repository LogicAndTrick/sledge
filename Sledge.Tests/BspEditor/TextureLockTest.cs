using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenTK;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.DataStructures.Geometric;

namespace Sledge.Tests.BspEditor
{
    /// <summary>
    /// This is a series of tests to ensure that texture lock behaves the same as other editors.
    /// </summary>
    [TestClass]
    public class TextureLockTest
    {
        private Face MakeScale1Face()
        {
            var f = new Face(1)
            {
                Texture =
                {
                    Name = "Test",
                    Rotation = 0,
                    UAxis = new Coordinate(1, 0, 0),
                    XShift = 0,
                    XScale = 1,
                    VAxis = new Coordinate(0, 0, -1),
                    YShift = 0,
                    YScale = 1
                },
                Vertices =
                {
                    new Coordinate(0, 0, 512),
                    new Coordinate(512, 0, 512),
                    new Coordinate(512, 0, 0),
                    new Coordinate(0, 0, 0),
                }
            };
            return f;
        }

        private Face MakeScale4Face()
        {
            var f = new Face(1)
            {
                Texture =
                {
                    Name = "Test",
                    Rotation = 0,
                    UAxis = new Coordinate(1, 0, 0),
                    XShift = 0,
                    XScale = 4,
                    VAxis = new Coordinate(0, 0, -1),
                    YShift = 0,
                    YScale = 4
                },
                Vertices =
                {
                    new Coordinate(0, 0, 512),
                    new Coordinate(512, 0, 512),
                    new Coordinate(512, 0, 0),
                    new Coordinate(0, 0, 0),
                }
            };
            return f;
        }

        private Face MakeScaleOddFace()
        {
            var f = new Face(1)
            {
                Texture =
                {
                    Name = "Test",
                    Rotation = 0,
                    UAxis = new Coordinate(.933581m, 0, 0.358368m),
                    XShift = 251.997m,
                    XScale = 0.25m,
                    VAxis = new Coordinate(0.358368m, 0, -0.933581m),
                    YShift = 22.9356m,
                    YScale = 1
                },
                Vertices =
                {
                    new Coordinate(0, 0, 512),
                    new Coordinate(512, 0, 512),
                    new Coordinate(512, 0, 0),
                    new Coordinate(0, 0, 0),
                }
            };
            return f;
        }

        private void TransformWithLock(Face face, Matrix matrix, bool textureLock, bool textureScaleLock)
        {
            if (textureLock)
            {
                face.Texture.TransformUniform(matrix);
            }
            if (textureScaleLock)
            {
                face.Texture.TransformScale(matrix);
            }
            face.Transform(matrix);
        }

        [TestMethod]
        public void TestShift1_Off()
        {
            var transform = Matrix.Translation(new Coordinate(64, 0, 0));
            var face = MakeScale1Face();

            TransformWithLock(face, transform, false, false);

            Assert.AreEqual(new Coordinate(1, 0, 0), face.Texture.UAxis);
            Assert.AreEqual(0, face.Texture.XShift);
            Assert.AreEqual(1, face.Texture.XScale);
            Assert.AreEqual(new Coordinate(0, 0, -1), face.Texture.VAxis);
            Assert.AreEqual(0, face.Texture.YShift);
            Assert.AreEqual(1, face.Texture.YScale);
        }

        [TestMethod]
        public void TestShift4_Off()
        {
            var transform = Matrix.Translation(new Coordinate(64, 0, 0));
            var face = MakeScale4Face();

            TransformWithLock(face, transform, false, false);

            Assert.AreEqual(new Coordinate(1, 0, 0), face.Texture.UAxis);
            Assert.AreEqual(0, face.Texture.XShift);
            Assert.AreEqual(4, face.Texture.XScale);
            Assert.AreEqual(new Coordinate(0, 0, -1), face.Texture.VAxis);
            Assert.AreEqual(0, face.Texture.YShift);
            Assert.AreEqual(4, face.Texture.YScale);
        }

        [TestMethod]
        public void TestShiftOdd_Off()
        {
            var transform = Matrix.Translation(new Coordinate(64, 0, 0));
            var face = MakeScaleOddFace();

            TransformWithLock(face, transform, false, false);

            Assert.AreEqual(new Coordinate(0.933581m, 0, 0.358368m), face.Texture.UAxis);
            Assert.AreEqual(251.997m, face.Texture.XShift);
            Assert.AreEqual(0.25m, face.Texture.XScale);
            Assert.AreEqual(new Coordinate(0.358368m, 0m, -0.933581m), face.Texture.VAxis);
            Assert.AreEqual(22.9356m, face.Texture.YShift);
            Assert.AreEqual(1, face.Texture.YScale);
        }

        [TestMethod]
        public void TestShift1_On()
        {
            var transform = Matrix.Translation(new Coordinate(64, 0, 0));
            var face = MakeScale1Face();

            TransformWithLock(face, transform, true, false);

            Assert.AreEqual(new Coordinate(1, 0, 0), face.Texture.UAxis);
            Assert.AreEqual(-64, face.Texture.XShift);
            Assert.AreEqual(1, face.Texture.XScale);
            Assert.AreEqual(new Coordinate(0, 0, -1), face.Texture.VAxis);
            Assert.AreEqual(0, face.Texture.YShift);
            Assert.AreEqual(1, face.Texture.YScale);
        }

        [TestMethod]
        public void TestShift4_On()
        {
            var transform = Matrix.Translation(new Coordinate(64, 0, 0));
            var face = MakeScale4Face();

            TransformWithLock(face, transform, true, false);

            Assert.AreEqual(new Coordinate(1, 0, 0), face.Texture.UAxis);
            Assert.AreEqual(-16, face.Texture.XShift);
            Assert.AreEqual(4, face.Texture.XScale);
            Assert.AreEqual(new Coordinate(0, 0, -1), face.Texture.VAxis);
            Assert.AreEqual(0, face.Texture.YShift);
            Assert.AreEqual(4, face.Texture.YScale);
        }

        [TestMethod]
        public void TestShiftOdd_On()
        {
            var transform = Matrix.Translation(new Coordinate(64, 0, 0));
            var face = MakeScaleOddFace();

            TransformWithLock(face, transform, true, false);

            Assert.AreEqual(new Coordinate(0.933581m, 0, 0.358368m), face.Texture.UAxis);
            Assert.That.DecimalEquals(13, face.Texture.XShift);
            Assert.That.DecimalEquals(0.25m, face.Texture.XScale);
            Assert.AreEqual(new Coordinate(0.358368m, 0m, -0.933581m), face.Texture.VAxis);
            Assert.That.DecimalEquals(0, face.Texture.YShift);
            Assert.That.DecimalEquals(1, face.Texture.YScale);
        }

        [TestMethod]
        public void TestScale1_On()
        {
            var transform = Matrix.Scale(new Coordinate((512 + 64) / 512m, 1, 1));
            var face = MakeScale1Face();

            TransformWithLock(face, transform, false, true);

            Assert.AreEqual(new Coordinate(1, 0, 0), face.Texture.UAxis);
            Assert.AreEqual(0, face.Texture.XShift);
            Assert.AreEqual(1.125m, face.Texture.XScale);
            Assert.AreEqual(new Coordinate(0, 0, -1), face.Texture.VAxis);
            Assert.AreEqual(0, face.Texture.YShift);
            Assert.AreEqual(1, face.Texture.YScale);
        }

        [TestMethod]
        public void TestScale1_Shifted_On()
        {
            var transform = Matrix.Scale(new Coordinate((512 + 64) / 512m, 1, 1));
            var face = MakeScale1Face();

            var shift = Matrix.Translation(new Coordinate(64, 0, 0));
            face.Transform(shift);

            Assert.AreEqual(new Coordinate(1, 0, 0), face.Texture.UAxis);
            Assert.AreEqual(0, face.Texture.XShift);
            Assert.AreEqual(1, face.Texture.XScale);
            Assert.AreEqual(new Coordinate(0, 0, -1), face.Texture.VAxis);
            Assert.AreEqual(0, face.Texture.YShift);
            Assert.AreEqual(1, face.Texture.YScale);

            // 

            TransformWithLock(face, transform, false, true);

            Assert.AreEqual(new Coordinate(1, 0, 0), face.Texture.UAxis);
            Assert.That.DecimalEquals(7.11111m, face.Texture.XShift);
            Assert.AreEqual(1.125m, face.Texture.XScale);
            Assert.AreEqual(new Coordinate(0, 0, -1), face.Texture.VAxis);
            Assert.AreEqual(0, face.Texture.YShift);
            Assert.AreEqual(1, face.Texture.YScale);
        }

        [TestMethod]
        public void TestScale4_On()
        {
            var transform = Matrix.Scale(new Coordinate((512 + 64) / 512m, 1, 1));
            var face = MakeScale4Face();

            TransformWithLock(face, transform, false, true);

            Assert.AreEqual(new Coordinate(1, 0, 0), face.Texture.UAxis);
            Assert.AreEqual(0, face.Texture.XShift);
            Assert.That.DecimalEquals(4.5m, face.Texture.XScale);
            Assert.AreEqual(new Coordinate(0, 0, -1), face.Texture.VAxis);
            Assert.AreEqual(0, face.Texture.YShift);
            Assert.AreEqual(4, face.Texture.YScale);
        }

        [TestMethod]
        public void TestScale4_Shifted_On()
        {
            var transform = Matrix.Scale(new Coordinate((512 + 64) / 512m, 1, 1));
            var face = MakeScale4Face();

            var shift = Matrix.Translation(new Coordinate(64, 0, 0));
            face.Transform(shift);

            Assert.AreEqual(new Coordinate(1, 0, 0), face.Texture.UAxis);
            Assert.AreEqual(0, face.Texture.XShift);
            Assert.AreEqual(4, face.Texture.XScale);
            Assert.AreEqual(new Coordinate(0, 0, -1), face.Texture.VAxis);
            Assert.AreEqual(0, face.Texture.YShift);
            Assert.AreEqual(1, face.Texture.YScale);

            // 

            TransformWithLock(face, transform, false, true);

            Assert.AreEqual(new Coordinate(1, 0, 0), face.Texture.UAxis);
            Assert.That.DecimalEquals(1.77778m, face.Texture.XShift);
            Assert.AreEqual(14.5m, face.Texture.XScale);
            Assert.AreEqual(new Coordinate(0, 0, -1), face.Texture.VAxis);
            Assert.AreEqual(0, face.Texture.YShift);
            Assert.AreEqual(1, face.Texture.YScale);
        }

        [TestMethod]
        public void TestScaleOdd_On()
        {
            var transform = Matrix.Scale(new Coordinate((512 + 64) / 512m, 1, 1));
            var face = MakeScaleOddFace();

            TransformWithLock(face, transform, false, true);

            Assert.AreEqual(new Coordinate(0.933581m, 0, 0.358368m), face.Texture.UAxis);
            Assert.That.DecimalEquals(251.997m, face.Texture.XShift);
            Assert.That.DecimalEquals(0.277434m, face.Texture.XScale);
            Assert.AreEqual(new Coordinate(0.358368m, 0m, -0.933581m), face.Texture.VAxis);
            Assert.That.DecimalEquals(22.9356m, face.Texture.YShift);
            Assert.That.DecimalEquals(1.01691m, face.Texture.YScale);
        }

        [TestMethod]
        public void TestScaleOdd_Shifted_On()
        {
            var transform = Matrix.Scale(new Coordinate((512 + 64) / 512m, 1, 1));
            var face = MakeScaleOddFace();

            var shift = Matrix.Translation(new Coordinate(64, 0, 0));
            face.Transform(shift);

            Assert.AreEqual(new Coordinate(0.933581m, 0, 0.358368m), face.Texture.UAxis);
            Assert.That.DecimalEquals(251.997m, face.Texture.XShift);
            Assert.That.DecimalEquals(0.25m, face.Texture.XScale);
            Assert.AreEqual(new Coordinate(0.358368m, 0m, -0.933581m), face.Texture.VAxis);
            Assert.That.DecimalEquals(22.9356m, face.Texture.YShift);
            Assert.That.DecimalEquals(1, face.Texture.YScale);

            // 

            TransformWithLock(face, transform, false, true);

            Assert.AreEqual(new Coordinate(0.933581m, 0, 0.358368m), face.Texture.UAxis);
            Assert.That.DecimalEquals(278.917m, face.Texture.XShift);
            Assert.That.DecimalEquals(0.277434m, face.Texture.XScale);
            Assert.AreEqual(new Coordinate(0.358368m, 0m, -0.933581m), face.Texture.VAxis);
            Assert.That.DecimalEquals(25.7549m, face.Texture.YShift);
            Assert.That.DecimalEquals(1.01691m, face.Texture.YScale);
        }

        [TestMethod]
        public void TestRotate1_15_Off()
        {
            var rotm = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(15));
            var mov = Matrix4.CreateTranslation(-256, -256, -256);
            var rot = Matrix4.Mult(mov, rotm);
            var mat = Matrix4.Mult(rot, Matrix4.Invert(mov));

            var transform = Matrix.FromOpenTKMatrix4(mat);
            var face = MakeScale1Face();

            TransformWithLock(face, transform, false, false);

            Assert.AreEqual(new Coordinate(1, 0, 0), face.Texture.UAxis);
            Assert.That.DecimalEquals(0, face.Texture.XShift);
            Assert.That.DecimalEquals(1, face.Texture.XScale);
            Assert.AreEqual(new Coordinate(0, 0, -1), face.Texture.VAxis);
            Assert.That.DecimalEquals(0, face.Texture.YShift);
            Assert.That.DecimalEquals(1, face.Texture.YScale);
        }

        [TestMethod]
        public void TestRotate1_15_On()
        {
            var rotm = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(15));
            var mov = Matrix4.CreateTranslation(-256, -256, -256);
            var rot = Matrix4.Mult(mov, rotm);
            var mat = Matrix4.Mult(rot, Matrix4.Invert(mov));

            var transform = Matrix.FromOpenTKMatrix4(mat);
            var face = MakeScale1Face();

            TransformWithLock(face, transform, true, false);

            Assert.AreEqual(new Coordinate(0.965926m, 0, -0.258819m), face.Texture.UAxis);
            Assert.That.DecimalEquals(74.9807m, face.Texture.XShift);
            Assert.That.DecimalEquals(1, face.Texture.XScale);
            Assert.AreEqual(new Coordinate(-0.258819m, 0, -0.965926m), face.Texture.VAxis);
            Assert.That.DecimalEquals(57.5347m, face.Texture.YShift);
            Assert.That.DecimalEquals(1, face.Texture.YScale);
        }

        [TestMethod]
        public void TestRotate1_60_On()
        {
            var rotm = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(60));
            var mov = Matrix4.CreateTranslation(-256, -256, -256);
            var rot = Matrix4.Mult(mov, rotm);
            var mat = Matrix4.Mult(rot, Matrix4.Invert(mov));

            var transform = Matrix.FromOpenTKMatrix4(mat);
            var face = MakeScale1Face();

            TransformWithLock(face, transform, true, false);

            Assert.AreEqual(new Coordinate(0.5m, 0, -0.866025m), face.Texture.UAxis);
            Assert.That.DecimalEquals(349.703m, face.Texture.XShift);
            Assert.That.DecimalEquals(1, face.Texture.XScale);
            Assert.AreEqual(new Coordinate(-0.866025m, 0, -0.5m), face.Texture.VAxis);
            Assert.That.DecimalEquals(93.7025m, face.Texture.YShift);
            Assert.That.DecimalEquals(1, face.Texture.YScale);
        }

        // Skew locking should technically be possible but it's not worth it right now
        /*
        [TestMethod]
        public void TestSkew1_On()
        {
            var shearMatrix = Matrix4.Identity;
            shearMatrix.M31 = 64;

            var transform = Matrix.FromOpenTKMatrix4(shearMatrix);
            var face = MakeScale1Face();

            TransformWithLock(face, transform, true, true);

            Assert.AreEqual(new Coordinate(0.992288m, 0, -0.12395m), face.Texture.UAxis);
            Assert.That.DecimalEquals(0, face.Texture.XShift);
            Assert.That.DecimalEquals(1, face.Texture.XScale);
            Assert.AreEqual(new Coordinate(0, 0, -1), face.Texture.VAxis);
            Assert.That.DecimalEquals(0, face.Texture.YShift);
            Assert.That.DecimalEquals(1, face.Texture.YScale);
        }
        */
    }
}

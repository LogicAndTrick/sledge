using System;
using System.Numerics;

namespace Sledge.Rendering
{
    public static class MathFunctions
    {
        // https://gist.github.com/871099/8d37734ba22737c69173c2e44eaa332f9c85bcde
        // http://www.opentk.com/node/1892
        // http://www.opentk.com/node/1276
        // http://www.opentk.com/node/887
        // http://mesa3d.org/

        /// <summary>
        /// Projects a coordinate from world space into screen space.
        /// </summary>
        /// <param name="coordinate">The coordinate to project</param>
        /// <param name="viewport">The viewport dimensions</param>
        /// <param name="projection">The projection matrix</param>
        /// <param name="modelview">The modelview matrix</param>
        /// <returns>The coordinate in screen space.</returns>
        public static Vector3 Project(Vector3 coordinate, int[] viewport, Matrix4x4 projection, Matrix4x4 modelview)
        {
            var source = new Vector4(coordinate.X, coordinate.Y, coordinate.Z, 1);
            var imed = Vector4.Transform(source, modelview);
            var vector = Vector4.Transform(imed, projection);
            if (vector.W < 0.00001) return new Vector3(1000000, 1000000, 1000000);
            var result = Vector3.Divide(new Vector3(vector.X, vector.Y, vector.Z), vector.W);
            result.X = viewport[0] + viewport[2] * (result.X + 1) / 2;
            result.Y = viewport[1] + viewport[3] * (result.Y + 1) / 2;
            result.Z = (result.Z + 1) / 2;
            return result;
        }

        /// <summary>
        /// Converts a screen space point into a corresponding point in world space.
        /// </summary>
        /// <param name="coordinate">The coordinate to project</param>
        /// <param name="viewport">The viewport dimensions</param>
        /// <param name="projection">The projection matrix</param>
        /// <param name="modelview">The modelview matrix</param>
        /// <returns>The coordinate in world space.</returns>
        public static Vector3 Unproject(Vector3 coordinate, int[] viewport, Matrix4x4 projection, Matrix4x4 modelview)
        {
            if (!Matrix4x4.Invert(Matrix4x4.Multiply(modelview, projection), out var matrix)) throw new Exception("Matrix cannot be inverted.");
            var source = new Vector4(
                (coordinate.X - viewport[0]) * 2 / viewport[2] - 1,
                (coordinate.Y - viewport[1]) * 2 / viewport[3] - 1,
                2 * coordinate.Z - 1,
                1);
            var vector = Vector4.Transform(source, matrix);
            if (vector.W < 0.00001) return new Vector3(1000000, 1000000, 1000000);
            var result = Vector3.Divide(new Vector3(vector.X, vector.Y, vector.Z), vector.W);
            return result;
        }
    }
}

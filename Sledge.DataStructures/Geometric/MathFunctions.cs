using System;
using OpenTK;

namespace Sledge.DataStructures.Geometric
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
        public static Coordinate Project(Coordinate coordinate, int[] viewport, Matrix4d projection, Matrix4d modelview)
        {
            var source = new Vector4d(coordinate.DX, coordinate.DY, coordinate.DZ, 1);
            var imed = Vector4d.Transform(source, modelview);
            var vector = Vector4d.Transform(imed, projection);
            if (Math.Abs(vector.W - 0) < 0.00001) return null;
            var result = Vector3d.Divide(vector.Xyz, vector.W);
            result.X = viewport[0] + viewport[2] * (result.X + 1) / 2;
            result.Y = viewport[1] + viewport[3] * (result.Y + 1) / 2;
            result.Z = (result.Z + 1) / 2;
            return new Coordinate((decimal) result.X, (decimal) result.Y, (decimal) result.Z);
        }

        /// <summary>
        /// Converts a screen space point into a corresponding point in world space.
        /// </summary>
        /// <param name="coordinate">The coordinate to project</param>
        /// <param name="viewport">The viewport dimensions</param>
        /// <param name="projection">The projection matrix</param>
        /// <param name="modelview">The modelview matrix</param>
        /// <returns>The coordinate in world space.</returns>
        public static Coordinate Unproject(Coordinate coordinate, int[] viewport, Matrix4d projection, Matrix4d modelview)
        {
            var matrix = Matrix4d.Invert(Matrix4d.Mult(modelview, projection));
            var source = new Vector4d(
                (coordinate.DX - viewport[0]) * 2 / viewport[2] - 1,
                (coordinate.DY - viewport[1]) * 2 / viewport[3] - 1,
                2 * coordinate.DZ - 1,
                1);
            var vector = Vector4d.Transform(source, matrix);
            if (Math.Abs(vector.W - 0) < 0.00001) return null;
            var result = Vector3d.Divide(vector.Xyz, vector.W);
            return new Coordinate((decimal) result.X, (decimal) result.Y, (decimal) result.Z);
        }
    }
}

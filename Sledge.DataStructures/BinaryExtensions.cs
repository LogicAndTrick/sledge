using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using OpenTK;
using Sledge.Common.Extensions;
using Sledge.DataStructures.Geometric;

namespace Sledge.DataStructures
{
    public static class BinaryExtensions
    {
        public static Coordinate[] ReadCoordinateArray(this BinaryReader br, int num)
        {
            var arr = new Coordinate[num];
            for (var i = 0; i < num; i++) arr[i] = br.ReadCoordinate();
            return arr;
        }

        public static CoordinateF[] ReadCoordinateFArray(this BinaryReader br, int num)
        {
            var arr = new CoordinateF[num];
            for (var i = 0; i < num; i++) arr[i] = br.ReadCoordinateF();
            return arr;
        }

        public static Vector3[] ReadVector3Array(this BinaryReader br, int num)
        {
            var arr = new Vector3[num];
            for (var i = 0; i < num; i++) arr[i] = br.ReadVector3();
            return arr;
        }

        public static Coordinate ReadCoordinate(this BinaryReader br)
        {
            return new Coordinate(
                br.ReadSingleAsDecimal(),
                br.ReadSingleAsDecimal(),
                br.ReadSingleAsDecimal()
                );
        }

        public static CoordinateF ReadCoordinateF(this BinaryReader br)
        {
            return new CoordinateF(
                br.ReadSingle(),
                br.ReadSingle(),
                br.ReadSingle()
                );
        }

        public static Vector3 ReadVector3(this BinaryReader br)
        {
            return new Vector3(
                br.ReadSingle(),
                br.ReadSingle(),
                br.ReadSingle()
                );
        }

        public static void WriteCoordinate(this BinaryWriter bw, Coordinate c)
        {
            bw.WriteDecimalAsSingle(c.X);
            bw.WriteDecimalAsSingle(c.Y);
            bw.WriteDecimalAsSingle(c.Z);
        }

        public static void WriteCoordinateF(this BinaryWriter bw, CoordinateF c)
        {
            bw.Write(c.X);
            bw.Write(c.Y);
            bw.Write(c.Z);
        }

        public static Plane ReadPlane(this BinaryReader br)
        {
            return new Plane(
                ReadCoordinate(br),
                ReadCoordinate(br),
                ReadCoordinate(br)
                );
        }

        public static void WritePlane(this BinaryWriter bw, Coordinate[] coords)
        {
            WriteCoordinate(bw, coords[0]);
            WriteCoordinate(bw, coords[1]);
            WriteCoordinate(bw, coords[2]);
        }
    }
}
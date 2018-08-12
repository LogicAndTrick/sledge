using System.IO;
using System.Numerics;
using Sledge.Common.Extensions;
using Sledge.DataStructures.Geometric;
using Plane = Sledge.DataStructures.Geometric.Plane;

namespace Sledge.DataStructures
{
    public static class BinaryExtensions
    {
        public static DVector3[] ReadDVector3Array(this BinaryReader br, int num)
        {
            var arr = new DVector3[num];
            for (var i = 0; i < num; i++) arr[i] = br.ReadDVector3();
            return arr;
        }

        public static Vector3[] ReadVector3Array(this BinaryReader br, int num)
        {
            var arr = new Vector3[num];
            for (var i = 0; i < num; i++) arr[i] = br.ReadVector3();
            return arr;
        }

        public static DVector3 ReadDVector3(this BinaryReader br)
        {
            return new DVector3(
                br.ReadSingleAsDecimal(),
                br.ReadSingleAsDecimal(),
                br.ReadSingleAsDecimal()
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

        public static void WriteDVector3(this BinaryWriter bw, DVector3 c)
        {
            bw.WriteDecimalAsSingle(c.X);
            bw.WriteDecimalAsSingle(c.Y);
            bw.WriteDecimalAsSingle(c.Z);
        }

        public static void WriteVector3(this BinaryWriter bw, Vector3 c)
        {
            bw.Write(c.X);
            bw.Write(c.Y);
            bw.Write(c.Z);
        }

        public static Plane ReadPlane(this BinaryReader br)
        {
            return new Plane(
                ReadVector3(br),
                ReadVector3(br),
                ReadVector3(br)
                );
        }

        public static void WritePlane(this BinaryWriter bw, Vector3[] coords)
        {
            WriteVector3(bw, coords[0]);
            WriteVector3(bw, coords[1]);
            WriteVector3(bw, coords[2]);
        }
    }
}
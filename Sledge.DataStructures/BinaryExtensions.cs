using System.IO;
using System.Numerics;
using Plane = Sledge.DataStructures.Geometric.Plane;

namespace Sledge.DataStructures
{
    public static class BinaryExtensions
    {
        public static Vector3[] ReadVector3Array(this BinaryReader br, int num)
        {
            var arr = new Vector3[num];
            for (var i = 0; i < num; i++) arr[i] = br.ReadVector3();
            return arr;
        }

        public static Vector3 ReadVector3(this BinaryReader br)
        {
            return new Vector3(
                br.ReadSingle(),
                br.ReadSingle(),
                br.ReadSingle()
            );
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
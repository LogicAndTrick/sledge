using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using Plane = Sledge.DataStructures.Geometric.Plane;

namespace Sledge.Providers
{
    public static class BinaryExtensions
    {
        public static string ReadFixedLengthString(this BinaryReader br, Encoding encoding, int length)
        {
            var bstr = br.ReadBytes(length).TakeWhile(b => b != 0).ToArray();
            return encoding.GetString(bstr);
        }

        public static void WriteFixedLengthString(this BinaryWriter bw, Encoding encoding, int length, string str)
        {
            var arr = new byte[length];
            encoding.GetBytes(str, 0, str.Length, arr, 0);
            bw.Write(arr, 0, length);
        }

        public static string ReadNullTerminatedString(this BinaryReader br)
        {
            var str = "";
            char c;
            while ((c = br.ReadChar()) != 0)
            {
                str += c;
            }
            return str;
        }

        public static void WriteNullTerminatedString(this BinaryWriter bw, string str)
        {
            bw.Write(str.ToCharArray());
            bw.Write((char) 0);
        }

        public static byte[] ReadByteArray(this BinaryReader br, int num)
        {
            var arr = new byte[num];
            for (var i = 0; i < num; i++) arr[i] = br.ReadByte();
            return arr;
        }

        public static short[] ReadShortArray(this BinaryReader br, int num)
        {
            var arr = new short[num];
            for (var i = 0; i < num; i++) arr[i] = br.ReadInt16();
            return arr;
        }

        public static int[] ReadIntArray(this BinaryReader br, int num)
        {
            var arr = new int[num];
            for (var i = 0; i < num; i++) arr[i] = br.ReadInt32();
            return arr;
        }

        public static decimal[] ReadSingleArrayAsDecimal(this BinaryReader br, int num)
        {
            var arr = new decimal[num];
            for (var i = 0; i < num; i++) arr[i] = br.ReadSingleAsDecimal();
            return arr;
        }

        public static float[] ReadSingleArray(this BinaryReader br, int num)
        {
            var arr = new float[num];
            for (var i = 0; i < num; i++) arr[i] = br.ReadSingle();
            return arr;
        }

        public static Vector3[] ReadVector3Array(this BinaryReader br, int num)
        {
            var arr = new Vector3[num];
            for (var i = 0; i < num; i++) arr[i] = br.ReadVector3();
            return arr;
        }

        public static string ReadCString(this BinaryReader br)
        {
            // GH#87: RMF strings aren't prefixed in the same way .NET's BinaryReader expects
            // Read the byte length and then read that number of characters.
            var len = br.ReadByte();
            var chars = br.ReadChars(len);
            return new string(chars).Trim('\0');
        }

        const int MaxVariableStringLength = 127;

        public static void WriteCString(this BinaryWriter bw, string str)
        {
            // GH#87: RMF strings aren't prefixed in the same way .NET's BinaryReader expects
            // Write the byte length (+1) and then write that number of characters plus the null terminator.
            // Hammer doesn't like RMF strings longer than 128 bytes...
            if (str == null) str = "";
            if (str.Length > MaxVariableStringLength) str = str.Substring(0, MaxVariableStringLength);
            bw.Write((byte) (str.Length + 1));
            bw.Write(str.ToCharArray());
            bw.Write('\0');
        }


        public static decimal ReadSingleAsDecimal(this BinaryReader br)
        {
            return (decimal) br.ReadSingle();
        }

        public static void WriteDecimalAsSingle(this BinaryWriter bw, decimal dec)
        {
            bw.Write((float) dec);
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

        public static Color ReadRGBColour(this BinaryReader br)
        {
            return Color.FromArgb(255, br.ReadByte(), br.ReadByte(), br.ReadByte());
        }

        public static void WriteRGBColour(this BinaryWriter bw, Color c)
        {
            bw.Write(c.R);
            bw.Write(c.G);
            bw.Write(c.B);
        }

        public static Color ReadRGBAColour(this BinaryReader br)
        {
            var r = br.ReadByte();
            var g = br.ReadByte();
            var b = br.ReadByte();
            var a = br.ReadByte();
            return Color.FromArgb(a, r, g, b);
        }

        public static void WriteRGBAColour(this BinaryWriter bw, Color c)
        {
            bw.Write(c.R);
            bw.Write(c.G);
            bw.Write(c.B);
            bw.Write(c.A);
        }
    }
}
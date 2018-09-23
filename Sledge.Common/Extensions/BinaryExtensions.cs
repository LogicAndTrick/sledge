using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace Sledge.Common.Extensions
{
    /// <summary>
    /// Common binary reader/write extension methods
    /// </summary>
    public static class BinaryExtensions
    {
        // Strings

        /// <summary>
        /// Read a fixed number of bytes from the reader and parse out an optionally null-terminated string
        /// </summary>
        /// <param name="br">Binary reader</param>
        /// <param name="encoding">The text encoding to use</param>
        /// <param name="length">The number of bytes to read</param>
        /// <returns>The string that was read</returns>
        public static string ReadFixedLengthString(this BinaryReader br, Encoding encoding, int length)
        {
            var bstr = br.ReadBytes(length).TakeWhile(b => b != 0).ToArray();
            return encoding.GetString(bstr);
        }

        /// <summary>
        /// Write a string to the writer and pad the width with nulls to reach a fixed number of bytes.
        /// </summary>
        /// <param name="bw">Binary writer</param>
        /// <param name="encoding">The text encoding to use</param>
        /// <param name="length">The number of bytes to write</param>
        /// <param name="str">The string to write</param>
        public static void WriteFixedLengthString(this BinaryWriter bw, Encoding encoding, int length, string str)
        {
            var arr = new byte[length];
            encoding.GetBytes(str, 0, str.Length, arr, 0);
            bw.Write(arr, 0, length);
        }

        /// <summary>
        /// Read a variable number of bytes into a string until a null terminator is reached.
        /// </summary>
        /// <param name="br">Binary reader</param>
        /// <returns>The string that was read</returns>
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

        /// <summary>
        /// Write a string followed by a null terminator.
        /// </summary>
        /// <param name="bw">Binary writer</param>
        /// <param name="str">The string to write</param>
        public static void WriteNullTerminatedString(this BinaryWriter bw, string str)
        {
            bw.Write(str.ToCharArray());
            bw.Write((char) 0);
        }

        /// <summary>
        /// Read a length-prefixed string from the reader.
        /// </summary>
        /// <param name="br">Binary reader</param>
        /// <returns>String that was read</returns>
        public static string ReadCString(this BinaryReader br)
        {
            // GH#87: RMF strings aren't prefixed in the same way .NET's BinaryReader expects
            // Read the byte length and then read that number of characters.
            var len = br.ReadByte();
            var chars = br.ReadChars(len);
            return new string(chars).Trim('\0');
        }


        /// <summary>
        /// Write a length-prefixed string to the writer.
        /// </summary>
        /// <param name="bw">Binary writer</param>
        /// <param name="str">The string to write</param>
        /// <param name="maximumLength">The maximum length of the string</param>
        public static void WriteCString(this BinaryWriter bw, string str, int maximumLength)
        {
            // GH#87: RMF strings aren't prefixed in the same way .NET's BinaryReader expects
            // Write the byte length (+1) and then write that number of characters plus the null terminator.
            // Hammer doesn't like RMF strings longer than 128 bytes...
            if (str == null) str = "";
            if (str.Length > maximumLength) str = str.Substring(0, maximumLength);
            bw.Write((byte)(str.Length + 1));
            bw.Write(str.ToCharArray());
            bw.Write('\0');
        }

        // Arrays

        /// <summary>
        /// Read an array of short unsigned integers
        /// </summary>
        /// <param name="br">Binary reader</param>
        /// <param name="num">The number of values to read</param>
        /// <returns>The resulting array</returns>
        public static ushort[] ReadUshortArray(this BinaryReader br, int num)
        {
            var arr = new ushort[num];
            for (var i = 0; i < num; i++) arr[i] = br.ReadUInt16();
            return arr;
        }

        /// <summary>
        /// Read an array of short integers
        /// </summary>
        /// <param name="br">Binary reader</param>
        /// <param name="num">The number of values to read</param>
        /// <returns>The resulting array</returns>
        public static short[] ReadShortArray(this BinaryReader br, int num)
        {
            var arr = new short[num];
            for (var i = 0; i < num; i++) arr[i] = br.ReadInt16();
            return arr;
        }

        /// <summary>
        /// Read an array of integers
        /// </summary>
        /// <param name="br">Binary reader</param>
        /// <param name="num">The number of values to read</param>
        /// <returns>The resulting array</returns>
        public static int[] ReadIntArray(this BinaryReader br, int num)
        {
            var arr = new int[num];
            for (var i = 0; i < num; i++) arr[i] = br.ReadInt32();
            return arr;
        }

        /// <summary>
        /// Read an array of floats and cast them to decimals
        /// </summary>
        /// <param name="br">Binary reader</param>
        /// <param name="num">The number of values to read</param>
        /// <returns>The resulting array</returns>
        public static decimal[] ReadSingleArrayAsDecimal(this BinaryReader br, int num)
        {
            var arr = new decimal[num];
            for (var i = 0; i < num; i++) arr[i] = br.ReadSingleAsDecimal();
            return arr;
        }

        /// <summary>
        /// Read an array of floats
        /// </summary>
        /// <param name="br">Binary reader</param>
        /// <param name="num">The number of values to read</param>
        /// <returns>The resulting array</returns>
        public static float[] ReadSingleArray(this BinaryReader br, int num)
        {
            var arr = new float[num];
            for (var i = 0; i < num; i++) arr[i] = br.ReadSingle();
            return arr;
        }

        // Decimal <-> Single
        
        /// <summary>
        /// Read a float and cast it to decimal
        /// </summary>
        /// <param name="br">Binary reader</param>
        /// <returns>Value that was read</returns>
        public static decimal ReadSingleAsDecimal(this BinaryReader br)
        {
            return (decimal) br.ReadSingle();
        }

        /// <summary>
        /// Write a decimal as a float
        /// </summary>
        /// <param name="bw">Binary writer</param>
        /// <param name="dec">Value to write</param>
        public static void WriteDecimalAsSingle(this BinaryWriter bw, decimal dec)
        {
            bw.Write((float) dec);
        }

        // Colours

        /// <summary>
        /// Read an RGB colour as 3 bytes
        /// </summary>
        /// <param name="br">Binary reader</param>
        /// <returns>The colour which was read</returns>
        public static Color ReadRGBColour(this BinaryReader br)
        {
            return Color.FromArgb(255, br.ReadByte(), br.ReadByte(), br.ReadByte());
        }

        /// <summary>
        /// Write an RGB colour as 3 bytes
        /// </summary>
        /// <param name="bw">Binary writer</param>
        /// <param name="c">The colour to write</param>
        public static void WriteRGBColour(this BinaryWriter bw, Color c)
        {
            bw.Write(c.R);
            bw.Write(c.G);
            bw.Write(c.B);
        }

        /// <summary>
        /// Read an RGBA colour as 4 bytes
        /// </summary>
        /// <param name="br">Binary reader</param>
        /// <returns>The colour which was read</returns>
        public static Color ReadRGBAColour(this BinaryReader br)
        {
            var r = br.ReadByte();
            var g = br.ReadByte();
            var b = br.ReadByte();
            var a = br.ReadByte();
            return Color.FromArgb(a, r, g, b);
        }

        /// <summary>
        /// Write an RGBA colour as 4 bytes
        /// </summary>
        /// <param name="bw">Binary writer</param>
        /// <param name="c">The colour to write</param>
        public static void WriteRGBAColour(this BinaryWriter bw, Color c)
        {
            bw.Write(c.R);
            bw.Write(c.G);
            bw.Write(c.B);
            bw.Write(c.A);
        }
    }
}
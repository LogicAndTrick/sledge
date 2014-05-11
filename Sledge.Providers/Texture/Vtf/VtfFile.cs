using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Sledge.DataStructures.GameData;
using Sledge.FileSystem;

namespace Sledge.Providers.Texture.Vtf
{
    struct Color
    {
        public byte A;
        public byte R;
        public byte G;
        public byte B;

        public Color(byte a, byte r, byte g, byte b)
        {
            A = a;
            R = r;
            G = g;
            B = b;
        }

        public Color(int r, int g, int b)
        {
            A = 255;
            R = (byte) r;
            G = (byte) g;
            B = (byte) b;
        }
    }

    public enum VtfImageFormat : int
    {
        None = -1,
        Rgba8888 = 0,
        Abgr8888,
        Rgb888,
        Bgr888,
        Rgb565,
        I8,
        Ia88,
        P8,
        A8,
        Rgb888Bluescreen,
        Bgr888Bluescreen,
        Argb8888,
        Bgra8888,
        Dxt1,
        Dxt3,
        Dxt5,
        Bgrx8888,
        Bgr565,
        Bgrx5551,
        Bgra4444,
        Dxt1Onebitalpha,
        Bgra5551,
        Uv88,
        Uvwq8888,
        Rgba16161616F,
        Rgba16161616,
        Uvlx8888,
        R32F,
        Rgb323232F,
        Rgba32323232F,
        NvDst16,
        NvDst24,	
        NvIntz,
        NvRawz,
        AtiDst16,
        AtiDst24,
        NvNull,
        Ati2N,	
        Ati1N,
    }

    [Flags]
    public enum VtfImageFlag : uint
    {
        Pointsample = 0x00000001,
        Trilinear = 0x00000002,
        Clamps = 0x00000004,
        Clampt = 0x00000008,
        Anisotropic = 0x00000010,
        HintDxt5 = 0x00000020,
        Srgb = 0x00000040,
        DeprecatedNocompress = 0x00000040,
        Normal = 0x00000080,
        Nomip = 0x00000100,
        Nolod = 0x00000200,
        Minmip = 0x00000400,
        Procedural = 0x00000800,
        Onebitalpha = 0x00001000,
        Eightbitalpha = 0x00002000,
        Envmap = 0x00004000,
        Rendertarget = 0x00008000,
        Depthrendertarget = 0x00010000,
        Nodebugoverride = 0x00020000,
        Singlecopy = 0x00040000,
        Unused0 = 0x00080000,
        DeprecatedOneovermiplevelinalpha = 0x00080000,
        Unused1 = 0x00100000,
        DeprecatedPremultcolorbyoneovermiplevel = 0x00100000,
        Unused2 = 0x00200000,
        DeprecatedNormaltodudv = 0x00200000,
        Unused3 = 0x00400000,
        DeprecatedAlphatestmipgeneration = 0x00400000,
        Nodepthbuffer = 0x00800000,
        Unused4 = 0x01000000,
        DeprecatedNicefiltered = 0x01000000,
        Clampu = 0x02000000,
        Vertextexture = 0x04000000,
        Ssbump = 0x08000000,
        Unused5 = 0x10000000,
        DeprecatedUnfilterableOk = 0x10000000,
        Border = 0x20000000,
        DeprecatedSpecvarRed = 0x40000000,
        DeprecatedSpecvarAlpha = 0x80000000,
        Last = 0x20000000
    }

    // Uses a lot of code from the excellent VtfLib
    // Re-created natively for flexibility and portability
    public class VtfProvider
    {
        private const string VtfHeader = "VTF";

        public static Size GetSize(IFile file)
        {
            using (var br = new BinaryReader(file.Open()))
            {
                var header = br.ReadFixedLengthString(Encoding.ASCII, 4);
                if (header != VtfHeader) throw new ProviderException("Invalid VTF header. Expected '" + VtfHeader + "', got '" + header + "'.");

                var v1 = br.ReadUInt32();
                var v2 = br.ReadUInt32();
                var version = v1 + (v2 / 10); // e.g. 7.3

                var headerSize = br.ReadUInt32();
                var width = br.ReadUInt16();
                var height = br.ReadUInt16();

                return new Size(width, height);
            }
        }

        public static Bitmap GetImage(IFile file)
        {
            using (var br = new BinaryReader(file.Open()))
            {
                var header = br.ReadFixedLengthString(Encoding.ASCII, 4);
                if (header != VtfHeader) throw new ProviderException("Invalid VTF header. Expected '" + VtfHeader + "', got '" + header + "'.");

                var v1 = br.ReadUInt32();
                var v2 = br.ReadUInt32();
                var version = v1 + (v2 / 10m); // e.g. 7.3

                var headerSize = br.ReadUInt32();
                var width = br.ReadUInt16();
                var height = br.ReadUInt16();

                var flags = (VtfImageFlag) br.ReadUInt32();

                var numFrames = br.ReadUInt16();
                var firstFrame = br.ReadUInt16();

                br.ReadBytes(4); // padding

                var reflectivity = br.ReadCoordinate();

                br.ReadBytes(4); // padding

                var bumpmapScale = br.ReadSingle();

                var highResImageFormat = (VtfImageFormat) br.ReadUInt32();
                var mipmapCount = br.ReadByte();
                var lowResImageFormat = (VtfImageFormat) br.ReadUInt32();
                var lowResWidth = br.ReadByte();
                var lowResHeight = br.ReadByte();

                //if (highResImageFormat != VtfImageFormat.Dxt1 && highResImageFormat != VtfImageFormat.Dxt1Onebitalpha && highResImageFormat != VtfImageFormat.Dxt5 && highResImageFormat != VtfImageFormat.Bgra8888)
                //    throw new Exception("This one! " + file.Name);

                ushort depth = 1;
                uint numResources = 0;

                if (version >= 7.2m)
                {
                    depth = br.ReadUInt16();
                }
                if (version >= 7.3m)
                {
                    br.ReadBytes(3);
                    numResources = br.ReadUInt32();
                }

                var faces = 1;
                if (flags.HasFlag(VtfImageFlag.Envmap))
                {
                    faces = version < 7.5m && firstFrame != 0xFFFF ? 7 : 6;
                }

                var imageSize = ComputeImageSize(width, height, depth, mipmapCount, highResImageFormat) * faces * numFrames;
                var thumbnailSize = lowResImageFormat == VtfImageFormat.None
                    ? 0
                    : ComputeImageSize(lowResWidth, lowResHeight, 1, lowResImageFormat);

                var thumbnailPos = headerSize;
                var dataPos = headerSize + thumbnailSize;

                for (var i = 0; i < numResources; i++)
                {
                    var type = br.ReadUInt32();
                    var data = br.ReadUInt32();
                    switch (type)
                    {
                        case 0x01:
                            // Low res image
                            thumbnailPos = data;
                            break;
                        case 0x30:
                            // Regular image
                            dataPos = data;
                            break;
                    }
                }

                if (lowResImageFormat != VtfImageFormat.None)
                {
                    br.BaseStream.Position = thumbnailPos;
                    var thumbnail = br.ReadBytes((int) thumbnailSize);
                    //var thumbImage = LoadImage(br, lowResWidth, lowResHeight, lowResImageFormat);
                    //return thumbImage;
                }

                var mipNum = GetMipToLoad(width, height, 512, mipmapCount);

                for (var frame = 0; frame < numFrames; frame++)
                {
                    for (var face = 0; face < faces; face++)
                    {
                        for (var slice = 0; slice < depth; slice++)
                        {
                            for (var mip = mipNum; mip < mipmapCount; mip++)
                            {
                                var wid = MipmapResize(width, mip);
                                var hei = MipmapResize(height, mip);
                                var offset = GetImageDataLocation(frame, face, slice, mip, highResImageFormat, width, height, numFrames, faces, depth, mipmapCount);
                                br.BaseStream.Position = offset + dataPos;

                                var img = LoadImage(br, (uint) wid, (uint) hei, highResImageFormat);
                                //img.Save(String.Format(@"D:\Github\sledge\_Resources\VTF\_test_fr{0}_fa{1}_sl{2}_m{3}.png", frame, face, slice, mip));
                                return img;
                            }
                        }
                    }
                }


                return null;
            }
        }

        private static int GetMipToLoad(uint width, uint height, int max, int mipCount)
        {
            var mip = 0;
            while (mip < mipCount - 1 && (width > max || height > max))
            {
                mip++;
                width >>= 1;
                height >>= 1;
            }
            return mip;
        }

        private static long GetImageDataLocation(int frame, int face, int slice, int mip, VtfImageFormat format, int width, int height, int numFrames, int numFaces, int depth, int numMips)
        {
            long offset = 0;

            // Transverse past all frames and faces of each mipmap (up to the requested one).
            for (var i = numMips - 1; i > mip; i--)
            {
                offset += ComputeMipmapSize(width, height, depth, i, format) * numFrames * numFaces;
            }

            var uiTemp1 = ComputeMipmapSize(width, height, depth, mip, format);
            var uiTemp2 = ComputeMipmapSize(width, height, 1, mip, format);

            // Transverse past requested frames and faces of requested mipmap.
            offset += uiTemp1 * frame * numFaces * depth;
            offset += uiTemp1 * face * depth;
            offset += uiTemp2 * slice;

            return offset;
        }

        private static int MipmapResize(int input, int level)
        {
            var res = input >> level;
            if (res < 1) res = 1;
            return res;
        }

        private static void TransformBytes(byte[] buffer, BinaryReader br, uint width, uint height, int bpp, int a, int r, int g, int b, bool bluescreen)
        {
            var bytes = br.ReadBytes((int) (width * height * bpp));
            for (var i = 0; i < bytes.Length; i += bpp)
            {
                buffer[i + 0] = (b >= 0) ? bytes[i + b] : (byte) 0; // b
                buffer[i + 1] = (g >= 0) ? bytes[i + g] : (byte) 0; // g
                buffer[i + 2] = (r >= 0) ? bytes[i + r] : (byte) 0; // r
                buffer[i + 3] = (a >= 0) ? bytes[i + a] : (byte) 255; // a
                if (bluescreen && buffer[i + 0] == 255 && buffer[i + 1] == 0 && buffer[i + 2] == 0) buffer[i + 3] = 0;
            }
        }

        private static Bitmap LoadImage(BinaryReader br, uint width, uint height, VtfImageFormat format)
        {
            var buffer = new byte[width * height * 4];
            switch (format)
            {
                case VtfImageFormat.Rgba8888:
                    TransformBytes(buffer, br, width, height, 4, 3, 0, 1, 2, false);
                    break;
                case VtfImageFormat.Abgr8888:
                    TransformBytes(buffer, br, width, height, 4, 0, 3, 2, 1, false);
                    break;
                case VtfImageFormat.Rgb888:
                    TransformBytes(buffer, br, width, height, 3, -1, 0, 1, 2, false);
                    break;
                case VtfImageFormat.Bgr888:
                    TransformBytes(buffer, br, width, height, 3, -1, 2, 1, 0, false);
                    break;
                case VtfImageFormat.Rgb565:
                    throw new NotImplementedException();
                    break;
                case VtfImageFormat.I8:
                    throw new NotImplementedException();
                    break;
                case VtfImageFormat.Ia88:
                    throw new NotImplementedException();
                    break;
                case VtfImageFormat.P8:
                    throw new NotImplementedException();
                    break;
                case VtfImageFormat.A8:
                    throw new NotImplementedException();
                    break;
                case VtfImageFormat.Rgb888Bluescreen:
                    TransformBytes(buffer, br, width, height, 3, -1, 0, 1, 2, true);
                    break;
                case VtfImageFormat.Bgr888Bluescreen:
                    TransformBytes(buffer, br, width, height, 3, -1, 2, 1, 0, true);
                    break;
                case VtfImageFormat.Argb8888:
                    TransformBytes(buffer, br, width, height, 4, 0, 1, 2, 3, false);
                    break;
                case VtfImageFormat.Bgra8888:
                    br.Read(buffer, 0, buffer.Length);
                    break;
                case VtfImageFormat.Dxt1:
                case VtfImageFormat.Dxt1Onebitalpha:
                    DecompressDxt1(buffer, br, width, height);
                    break;
                case VtfImageFormat.Dxt3:
                    throw new NotImplementedException();
                    break;
                case VtfImageFormat.Dxt5:
                    DecompressDxt5(buffer, br, width, height);
                    break;
                case VtfImageFormat.Bgrx8888:
                    TransformBytes(buffer, br, width, height, 4, -1, 2, 1, 0, false);
                    break;
                case VtfImageFormat.Bgr565:
                    throw new NotImplementedException();
                    break;
                case VtfImageFormat.Bgrx5551:
                    throw new NotImplementedException();
                    break;
                case VtfImageFormat.Bgra4444:
                    throw new NotImplementedException();
                    break;
                case VtfImageFormat.Bgra5551:
                    throw new NotImplementedException();
                    break;
                case VtfImageFormat.Uv88:
                    throw new NotImplementedException();
                    break;
                case VtfImageFormat.Uvwq8888:
                    throw new NotImplementedException();
                    break;
                case VtfImageFormat.Rgba16161616F:
                    throw new NotImplementedException();
                    break;
                case VtfImageFormat.Rgba16161616:
                    throw new NotImplementedException();
                    break;
                case VtfImageFormat.Uvlx8888:
                    throw new NotImplementedException();
                    break;
                case VtfImageFormat.R32F:
                    throw new NotImplementedException();
                    break;
                case VtfImageFormat.Rgb323232F:
                    throw new NotImplementedException();
                    break;
                case VtfImageFormat.Rgba32323232F:
                    throw new NotImplementedException();
                    break;
                case VtfImageFormat.NvDst16:
                    throw new NotImplementedException();
                    break;
                case VtfImageFormat.NvDst24:
                    throw new NotImplementedException();
                    break;
                case VtfImageFormat.NvIntz:
                    throw new NotImplementedException();
                    break;
                case VtfImageFormat.NvRawz:
                    throw new NotImplementedException();
                    break;
                case VtfImageFormat.AtiDst16:
                    throw new NotImplementedException();
                    break;
                case VtfImageFormat.AtiDst24:
                    throw new NotImplementedException();
                    break;
                case VtfImageFormat.NvNull:
                    throw new NotImplementedException();
                    break;
                case VtfImageFormat.Ati2N:
                    throw new NotImplementedException();
                    break;
                case VtfImageFormat.Ati1N:
                    throw new NotImplementedException();
                    break;
            }
            var bmp = new Bitmap((int)width, (int)height, PixelFormat.Format32bppArgb);
            var bits = bmp.LockBits(new Rectangle(0, 0, (int)width, (int)height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            Marshal.Copy(buffer, 0, bits.Scan0, buffer.Length);
            bmp.UnlockBits(bits);
            return bmp;
        }

        private static void DecompressDxt1(byte[] buffer, BinaryReader br, uint width, uint height)
        {
            var c = new byte[16];
            for (var y = 0; y < height; y += 4)
            {
                for (var x = 0; x < width; x += 4)
                {
                    var c0 = br.ReadUInt16();
                    var c1 = br.ReadUInt16();

                    c[0] = (byte)((c0 & 0xF800) >> 8);
                    c[1] = (byte)((c0 & 0x07E0) >> 3);
                    c[2] = (byte)((c0 & 0x001F) << 3);
                    c[3] = 255;

                    c[4] = (byte)((c1 & 0xF800) >> 8);
                    c[5] = (byte)((c1 & 0x07E0) >> 3);
                    c[6] = (byte)((c1 & 0x001F) << 3);
                    c[7] = 255;

                    if (c0 > c1)
                    {
                        // No alpha channel

                        c[8 ] = (byte)((2 * c[0] + c[4]) / 3);
                        c[9 ] = (byte)((2 * c[1] + c[5]) / 3);
                        c[10] = (byte)((2 * c[2] + c[6]) / 3);
                        c[11] = 255;

                        c[12] = (byte)((c[0] + 2 * c[4]) / 3);
                        c[13] = (byte)((c[1] + 2 * c[5]) / 3);
                        c[14] = (byte)((c[2] + 2 * c[6]) / 3);
                        c[15] = 255;
                    }
                    else
                    {
                        // 1-bit alpha channel

                        c[8] = (byte)((c[0] + c[4]) / 2);
                        c[9] = (byte)((c[1] + c[5]) / 2);
                        c[10] = (byte)((c[2] + c[6]) / 2);
                        c[11] = 255;
                        c[12] = 0;
                        c[13] = 0;
                        c[14] = 0;
                        c[15] = 0;
                    }

                    var bytes = br.ReadUInt32();
                    for (var yy = 0; yy < 4; yy++)
                    {
                        for (var xx = 0; xx < 4; xx++)
                        {
                            var xpos = x + xx;
                            var ypos = y + yy;
                            if (xpos < width && ypos < height)
                            {
                                var index = bytes & 0x0003;
                                index *= 4;
                                var pointer = ypos * width * 4 + xpos * 4;
                                buffer[pointer + 0] = c[index + 2]; // b
                                buffer[pointer + 1] = c[index + 1]; // g
                                buffer[pointer + 2] = c[index + 0]; // r
                                buffer[pointer + 3] = c[index + 3]; // a
                            }
                            bytes >>= 2;
                        }
                    }
                }
            }
        }

        private static void DecompressDxt5(byte[] buffer, BinaryReader br, uint width, uint height)
        {
            var c = new Color[4];
            var a = new int[8];
            for (var y = 0; y < height; y += 4)
            {
                for (var x = 0; x < width; x += 4)
                {
                    var a0 = br.ReadByte();
                    var a1 = br.ReadByte();
                    a[0] = a0;
                    a[1] = a1;

                    if (a0 > a1)
                    {
                        a[2] = (6 * a[0] + 1 * a[1] + 3) / 7;
                        a[3] = (5 * a[0] + 2 * a[1] + 3) / 7;
                        a[4] = (4 * a[0] + 3 * a[1] + 3) / 7;
                        a[5] = (3 * a[0] + 4 * a[1] + 3) / 7;
                        a[6] = (2 * a[0] + 5 * a[1] + 3) / 7;
                        a[7] = (1 * a[0] + 6 * a[1] + 3) / 7;
                    }
                    else
                    {
                        a[2] = (4 * a[0] + 1 * a[1] + 2) / 5;
                        a[3] = (3 * a[0] + 2 * a[1] + 2) / 5;
                        a[4] = (2 * a[0] + 3 * a[1] + 2) / 5;
                        a[5] = (1 * a[0] + 4 * a[1] + 2) / 5;
                        a[6] = 0x00;
                        a[7] = 0xFF;
                    }

                    var aindex = 0L;
                    for (var i = 0; i < 6; i++) aindex |= ((long) br.ReadByte()) << (8 * i);

                    var c0 = br.ReadUInt16();
                    var c1 = br.ReadUInt16();
                    c[0] = ColourFromRgb565(c0);
                    c[1] = ColourFromRgb565(c1);
                    c[2] = LerpColours(c[0], c[1], 1, 3);
                    c[3] = LerpColours(c[0], c[1], 2, 3);

                    var bytes = br.ReadUInt32();
                    for (var yy = 0; yy < 4; yy++)
                    {
                        for (var xx = 0; xx < 4; xx++)
                        {
                            var xpos = x + xx;
                            var ypos = y + yy;
                            if (xpos < width && ypos < height)
                            {
                                var index = bytes & 0x0003;
                                var colour = c[index];
                                var alpha = (byte) a[aindex & 0x07];
                                var pointer = ypos * width * 4 + xpos * 4;
                                buffer[pointer + 0] = colour.B; // b
                                buffer[pointer + 1] = colour.G; // g
                                buffer[pointer + 2] = colour.R; // r
                                buffer[pointer + 3] = alpha; // a
                            }
                            bytes >>= 2;
                            aindex >>= 3;
                        }
                    }
                }
            }
        }

        private static Color ColourFromRgb565(ushort c)
        {
            var r = (c & 0xF800) >> 8;
            var g = (c & 0x07E0) >> 3;
            var b = (c & 0x001F) << 3;
            return new Color(r, g, b);
        }

        private static Color LerpColours(Color min, Color max, int num, int total)
        {
            var d1 = 1 - num;
            var d2 = num;
            var r = (min.R * d1 + max.R * d2) / total;
            var g = (min.G * d1 + max.G * d2) / total;
            var b = (min.B * d1 + max.B * d2) / total;
            return new Color(r, g, b);
        }

        private static VtfImageFormatInfo GetImageFormatInfo(VtfImageFormat imageFormat)
        {
            return ImageFormats[imageFormat];
        }

        private static uint ComputeMipmapSize(int width, int height, int depth, int mipLevel, VtfImageFormat format)
        {
            var w = MipmapResize(width, mipLevel);
            var h = MipmapResize(height, mipLevel);
            var d = MipmapResize(depth, mipLevel);
            return ComputeImageSize((uint) w, (uint) h, (uint) d, format);
        }

        private static uint ComputeImageSize(uint width, uint height, uint depth, VtfImageFormat imageFormat)
        {
            switch (imageFormat)
            {
                case VtfImageFormat.Dxt1:
                case VtfImageFormat.Dxt1Onebitalpha:
                    if (width < 4 && width > 0) width = 4;
                    if (height < 4 && height > 0) height = 4;
                    return ((width + 3) / 4) * ((height + 3) / 4) * 8 * depth;
                case VtfImageFormat.Dxt3:
                case VtfImageFormat.Dxt5:
                    if (width < 4 && width > 0) width = 4;
                    if (height < 4 && height > 0) height = 4;
                    return ((width + 3) / 4) * ((height + 3) / 4) * 16 * depth;
                default:
                    return width * height * depth * GetImageFormatInfo(imageFormat).BytesPerPixel;
            }
        }

        private static uint ComputeImageSize(uint width, uint height, uint depth, uint uiMipmaps, VtfImageFormat imageFormat)
        {
            uint uiImageSize = 0;

            for (var i = 0; i < uiMipmaps; i++)
            {
                uiImageSize += ComputeImageSize(width, height, depth, imageFormat);

                width >>= 1;
                height >>= 1;
                depth >>= 1;

                if (width < 1) width = 1;
                if (height < 1) height = 1;
                if (depth < 1) depth = 1;
            }

            return uiImageSize;
        }

        private class VtfImageFormatInfo
        {
            public VtfImageFormat Format { get; set; }
            public uint BitsPerPixel { get; set; }
            public uint BytesPerPixel { get; set; }
            public uint RedBitsPerPixel { get; set; }
            public uint GreenBitsPerPixel { get; set; }
            public uint BlueBitsPerPixel { get; set; }
            public uint AlphaBitsPerPixel { get; set; }
            public bool IsCompressed { get; set; }
            public bool IsSupported { get; set; }

            public VtfImageFormatInfo(VtfImageFormat format, uint bitsPerPixel, uint bytesPerPixel, uint redBitsPerPixel, uint greenBitsPerPixel, uint blueBitsPerPixel, uint alphaBitsPerPixel, bool isCompressed, bool isSupported)
            {
                Format = format;
                BitsPerPixel = bitsPerPixel;
                BytesPerPixel = bytesPerPixel;
                RedBitsPerPixel = redBitsPerPixel;
                GreenBitsPerPixel = greenBitsPerPixel;
                BlueBitsPerPixel = blueBitsPerPixel;
                AlphaBitsPerPixel = alphaBitsPerPixel;
                IsCompressed = isCompressed;
                IsSupported = isSupported;
            }
        }

        private static readonly Dictionary<VtfImageFormat, VtfImageFormatInfo> ImageFormats = new Dictionary<VtfImageFormat, VtfImageFormatInfo>
        {
            {VtfImageFormat.Rgba8888, new VtfImageFormatInfo(VtfImageFormat.Rgba8888, 32, 4, 8, 8, 8, 8, false, true)},
            {VtfImageFormat.Abgr8888, new VtfImageFormatInfo(VtfImageFormat.Abgr8888, 32, 4, 8, 8, 8, 8, false, true)},
            {VtfImageFormat.Rgb888, new VtfImageFormatInfo(VtfImageFormat.Rgb888, 24, 3, 8, 8, 8, 0, false, true)},
            {VtfImageFormat.Bgr888, new VtfImageFormatInfo(VtfImageFormat.Bgr888, 24, 3, 8, 8, 8, 0, false, true)},
            {VtfImageFormat.Rgb565, new VtfImageFormatInfo(VtfImageFormat.Rgb565, 16, 2, 5, 6, 5, 0, false, true)},
            {VtfImageFormat.I8, new VtfImageFormatInfo(VtfImageFormat.I8, 8, 1, 0, 0, 0, 0, false, true)},
            {VtfImageFormat.Ia88, new VtfImageFormatInfo(VtfImageFormat.Ia88, 16, 2, 0, 0, 0, 8, false, true)},
            {VtfImageFormat.P8, new VtfImageFormatInfo(VtfImageFormat.P8, 8, 1, 0, 0, 0, 0, false, false)},
            {VtfImageFormat.A8, new VtfImageFormatInfo(VtfImageFormat.A8, 8, 1, 0, 0, 0, 8, false, true)},
            {VtfImageFormat.Rgb888Bluescreen, new VtfImageFormatInfo(VtfImageFormat.Rgb888Bluescreen, 24, 3, 8, 8, 8, 0, false, true)},
            {VtfImageFormat.Bgr888Bluescreen, new VtfImageFormatInfo(VtfImageFormat.Bgr888Bluescreen, 24, 3, 8, 8, 8, 0, false, true)},
            {VtfImageFormat.Argb8888, new VtfImageFormatInfo(VtfImageFormat.Argb8888, 32, 4, 8, 8, 8, 8, false, true)},
            {VtfImageFormat.Bgra8888, new VtfImageFormatInfo(VtfImageFormat.Bgra8888, 32, 4, 8, 8, 8, 8, false, true)},
            {VtfImageFormat.Dxt1, new VtfImageFormatInfo(VtfImageFormat.Dxt1, 4, 0, 0, 0, 0, 0, true, true)},
            {VtfImageFormat.Dxt3, new VtfImageFormatInfo(VtfImageFormat.Dxt3, 8, 0, 0, 0, 0, 8, true, true)},
            {VtfImageFormat.Dxt5, new VtfImageFormatInfo(VtfImageFormat.Dxt5, 8, 0, 0, 0, 0, 8, true, true)},
            {VtfImageFormat.Bgrx8888, new VtfImageFormatInfo(VtfImageFormat.Bgrx8888, 32, 4, 8, 8, 8, 0, false, true)},
            {VtfImageFormat.Bgr565, new VtfImageFormatInfo(VtfImageFormat.Bgr565, 16, 2, 5, 6, 5, 0, false, true)},
            {VtfImageFormat.Bgrx5551, new VtfImageFormatInfo(VtfImageFormat.Bgrx5551, 16, 2, 5, 5, 5, 0, false, true)},
            {VtfImageFormat.Bgra4444, new VtfImageFormatInfo(VtfImageFormat.Bgra4444, 16, 2, 4, 4, 4, 4, false, true)},
            {VtfImageFormat.Dxt1Onebitalpha, new VtfImageFormatInfo(VtfImageFormat.Dxt1Onebitalpha, 4, 0, 0, 0, 0, 1, true, true)},
            {VtfImageFormat.Bgra5551, new VtfImageFormatInfo(VtfImageFormat.Bgra5551, 16, 2, 5, 5, 5, 1, false, true)},
            {VtfImageFormat.Uv88, new VtfImageFormatInfo(VtfImageFormat.Uv88, 16, 2, 8, 8, 0, 0, false, true)},
            {VtfImageFormat.Uvwq8888, new VtfImageFormatInfo(VtfImageFormat.Uvwq8888, 32, 4, 8, 8, 8, 8, false, true)},
            {VtfImageFormat.Rgba16161616F, new VtfImageFormatInfo(VtfImageFormat.Rgba16161616F, 64, 8, 16, 16, 16, 16, false, true)},
            {VtfImageFormat.Rgba16161616, new VtfImageFormatInfo(VtfImageFormat.Rgba16161616, 64, 8, 16, 16, 16, 16, false, true)},
            {VtfImageFormat.Uvlx8888, new VtfImageFormatInfo(VtfImageFormat.Uvlx8888, 32, 4, 8, 8, 8, 8, false, true)},
            {VtfImageFormat.R32F, new VtfImageFormatInfo(VtfImageFormat.R32F, 32, 4, 32, 0, 0, 0, false, true)},
            {VtfImageFormat.Rgb323232F, new VtfImageFormatInfo(VtfImageFormat.Rgb323232F, 96, 12, 32, 32, 32, 0, false, true)},
            {VtfImageFormat.Rgba32323232F, new VtfImageFormatInfo(VtfImageFormat.Rgba32323232F, 128, 16, 32, 32, 32, 32, false, true)},
            {VtfImageFormat.NvDst16, new VtfImageFormatInfo(VtfImageFormat.NvDst16, 16, 2, 0, 0, 0, 0, false, true)},
            {VtfImageFormat.NvDst24, new VtfImageFormatInfo(VtfImageFormat.NvDst24, 24, 3, 0, 0, 0, 0, false, true)},
            {VtfImageFormat.NvIntz, new VtfImageFormatInfo(VtfImageFormat.NvIntz, 32, 4, 0, 0, 0, 0, false, true)},
            {VtfImageFormat.NvRawz, new VtfImageFormatInfo(VtfImageFormat.NvRawz, 32, 4, 0, 0, 0, 0, false, true)},
            {VtfImageFormat.AtiDst16, new VtfImageFormatInfo(VtfImageFormat.AtiDst16, 16, 2, 0, 0, 0, 0, false, true)},
            {VtfImageFormat.AtiDst24, new VtfImageFormatInfo(VtfImageFormat.AtiDst24, 24, 3, 0, 0, 0, 0, false, true)},
            {VtfImageFormat.NvNull, new VtfImageFormatInfo(VtfImageFormat.NvNull, 32, 4, 0, 0, 0, 0, false, true)},
            {VtfImageFormat.Ati1N, new VtfImageFormatInfo(VtfImageFormat.Ati1N, 4, 0, 0, 0, 0, 0, true, true)},
            {VtfImageFormat.Ati2N, new VtfImageFormatInfo(VtfImageFormat.Ati2N, 8, 0, 0, 0, 0, 0, true, true)},
        };
    }
}

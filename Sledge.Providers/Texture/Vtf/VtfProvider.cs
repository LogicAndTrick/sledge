using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using OpenTK;
using Sledge.FileSystem;

namespace Sledge.Providers.Texture.Vtf
{
    // Uses a lot of code from the excellent VtfLib
    // Re-created natively for flexibility and portability
    public static class VtfProvider
    {
        private const string VtfHeader = "VTF";

        public static Size GetSize(IFile file)
        {
            return GetSize(file.Open());
        }

        public static Size GetSize(Stream stream)
        {
            using (var br = new BinaryReader(stream))
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

        public static Bitmap GetImage(IFile file, int maxWidth = 0, int maxHeight = 0)
        {
            return GetImage(file.Open(), maxWidth, maxHeight);
        }

        public static Bitmap GetImage(Stream stream, int maxWidth = 0, int maxHeight = 0)
        {
            using (var br = new BinaryReader(stream))
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

                var mipNum = 0;
                if (maxWidth > 0 || maxHeight > 0) mipNum = GetMipToLoad(width, height, maxWidth > 0 ? maxWidth : width, maxHeight > 0 ? maxHeight : height, mipmapCount);

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

        private static int GetMipToLoad(uint width, uint height, int maxWidth, int maxHeight, int mipCount)
        {
            var mip = 0;
            while (mip < mipCount - 1 && (width > maxWidth || height > maxHeight))
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
            var bytes = br.ReadBytes((int)(width * height * bpp));
            for (int i = 0, j = 0; i < bytes.Length; i += bpp, j += 4)
            {
                buffer[j + 0] = (b >= 0) ? bytes[i + b] : (byte)0; // b
                buffer[j + 1] = (g >= 0) ? bytes[i + g] : (byte)0; // g
                buffer[j + 2] = (r >= 0) ? bytes[i + r] : (byte)0; // r
                buffer[j + 3] = (a >= 0) ? bytes[i + a] : (byte)255; // a
                if (bluescreen && buffer[j + 0] == 255 && buffer[j + 1] == 0 && buffer[j + 2] == 0) buffer[j + 3] = 0;
            }
        }

        private static void TransformShorts(byte[] buffer, BinaryReader br, uint width, uint height, int bpp, int a, int r, int g, int b)
        {
            a *= 2;
            r *= 2;
            g *= 2;
            b *= 2;
            var bytes = br.ReadBytes((int)(width * height * bpp));
            for (int i = 0, j = 0; i < bytes.Length; i += bpp, j += 4)
            {
                buffer[j + 0] = (b >= 0) ? (byte)((bytes[i + b] | bytes[i + b + 1] << 8) / 256) : (byte)0; // b
                buffer[j + 1] = (g >= 0) ? (byte)((bytes[i + g] | bytes[i + g + 1] << 8) / 256) : (byte)0; // g
                buffer[j + 2] = (r >= 0) ? (byte)((bytes[i + r] | bytes[i + r + 1] << 8) / 256) : (byte)0; // r
                buffer[j + 3] = (a >= 0) ? (byte)((bytes[i + a] | bytes[i + a + 1] << 8) / 256) : (byte)255; // a
            }
        }

        private static void TransformRgba16161616F(byte[] buffer, BinaryReader br, uint width, uint height)
        {
            // I have no idea how this works. It's just converted straight from VTFLib.
            // I think the half format is slightly different to what it should be, which causes the result to be different to VTFLib.
            // Fortunately Sledge does not need to care about cubemaps, which is what this format seems to be used for...
            const int a = 6;
            const int r = 0;
            const int g = 2;
            const int b = 4;
            var bytes = br.ReadBytes((int)(width * height * 8));

            var log = 0d;
            for (int i = 0, j = 0; i < bytes.Length; i += 8, j += 4)
            {
                var hb = Half.FromBytes(bytes, i + b).ToSingle();
                var hg = Half.FromBytes(bytes, i + g).ToSingle();
                var hr = Half.FromBytes(bytes, i + r).ToSingle();
                var lum = hr * 0.299f + hg * 0.587f + hb * 0.114f;
                log += Math.Log(0.0000000001d + lum);
            }
            log = Math.Exp(log / (width * height));

            for (int i = 0, j = 0; i < bytes.Length; i += 8, j += 4)
            {
                var hb = Half.FromBytes(bytes, i + b).ToSingle();
                var hg = Half.FromBytes(bytes, i + g).ToSingle();
                var hr = Half.FromBytes(bytes, i + r).ToSingle();
                var ha = Half.FromBytes(bytes, i + a).ToSingle();

                var y = hr * 0.299f + hg * 0.587f + hb * 0.114f;
                var u = (hb - y) * 0.565f;
                var v = (hr - y) * 0.713f;

                var mul = 4 * y / log;
                mul = mul / (1 + mul);
                mul /= y;

                hr = (float) Math.Pow((y + 1.403f * v) * mul, 2.25f);
                hg = (float) Math.Pow((y - 0.344f * u - 0.714f * v) * mul, 2.25f);
                hb = (float)Math.Pow((y + 1.770f * u) * mul, 2.25f);

                if (hr < 0) hr = 0;
                if (hr > 1) hr = 1;
                if (hg < 0) hg = 0;
                if (hg > 1) hg = 1;
                if (hb < 0) hb = 0;
                if (hb > 1) hb = 1;

                buffer[j + 0] = (byte)(hb * 255); // b
                buffer[j + 1] = (byte)(hg * 255); // g
                buffer[j + 2] = (byte)(hr * 255); // r
                buffer[j + 3] = (byte)(ha * 255); // a
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
                    TransformBytes(buffer, br, width, height, 2, -1, 0, 1, -1, false);
                    break;
                case VtfImageFormat.Uvwq8888:
                    throw new NotImplementedException();
                    break;
                case VtfImageFormat.Rgba16161616F:
                    TransformRgba16161616F(buffer, br, width, height);
                    break;
                case VtfImageFormat.Rgba16161616:
                    TransformShorts(buffer, br, width, height, 8, 3, 0, 1, 2);
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
            var num = ((width + 3) / 4) * ((height + 3) / 4) * 8;
            var all = br.ReadBytes((int) num);
            var pos = 0;
            var c = new byte[16];
            for (var y = 0; y < height; y += 4)
            {
                for (var x = 0; x < width; x += 4)
                {
                    int c0 = all[pos++];
                    c0 |= all[pos++] << 8;

                    int c1 = all[pos++];
                    c1 |= all[pos++] << 8;

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

                    int bytes = all[pos++];
                    bytes |= all[pos++] << 8;
                    bytes |= all[pos++] << 16;
                    bytes |= all[pos++] << 24;

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
            var num = ((width + 3) / 4) * ((height + 3) / 4) * 16;
            var all = br.ReadBytes((int) num);
            var pos = 0;
            var c = new byte[16];
            var a = new int[8];
            for (var y = 0; y < height; y += 4)
            {
                for (var x = 0; x < width; x += 4)
                {
                    var a0 = all[pos++];
                    var a1 = all[pos++];

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
                    for (var i = 0; i < 6; i++) aindex |= ((long)all[pos++]) << (8 * i);

                    int c0 = all[pos++];
                    c0 |= all[pos++] << 8;

                    int c1 = all[pos++];
                    c1 |= all[pos++] << 8;

                    c[0] = (byte)((c0 & 0xF800) >> 8);
                    c[1] = (byte)((c0 & 0x07E0) >> 3);
                    c[2] = (byte)((c0 & 0x001F) << 3);
                    c[3] = 255;

                    c[4] = (byte)((c1 & 0xF800) >> 8);
                    c[5] = (byte)((c1 & 0x07E0) >> 3);
                    c[6] = (byte)((c1 & 0x001F) << 3);
                    c[7] = 255;

                    c[8] = (byte)((2 * c[0] + c[4]) / 3);
                    c[9] = (byte)((2 * c[1] + c[5]) / 3);
                    c[10] = (byte)((2 * c[2] + c[6]) / 3);
                    c[11] = 255;

                    c[12] = (byte)((c[0] + 2 * c[4]) / 3);
                    c[13] = (byte)((c[1] + 2 * c[5]) / 3);
                    c[14] = (byte)((c[2] + 2 * c[6]) / 3);
                    c[15] = 255;

                    int bytes = all[pos++];
                    bytes |= all[pos++] << 8;
                    bytes |= all[pos++] << 16;
                    bytes |= all[pos++] << 24;

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
                                var alpha = (byte) a[aindex & 0x07];
                                var pointer = ypos * width * 4 + xpos * 4;
                                buffer[pointer + 0] = c[index + 2]; // b
                                buffer[pointer + 1] = c[index + 1]; // g
                                buffer[pointer + 2] = c[index + 0]; // r
                                buffer[pointer + 3] = alpha; // a
                            }
                            bytes >>= 2;
                            aindex >>= 3;
                        }
                    }
                }
            }
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
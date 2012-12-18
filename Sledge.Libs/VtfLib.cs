using System;
using System.Runtime.InteropServices;

namespace Sledge.Libs
{
    public static class VtfLib
    {
        #region Constants
        public const int VersionNumber = 130;
        public const string VersionString = "1.3.0";

        public const uint MajorVersion = 7;
        public const uint MinorVersion = 4;
        #endregion

        #region Enumerations
        public enum Option
        {
            OptionDXTQuality = 0,

            OptionLuminanceWeightR,
            OptionLuminanceWeightG,
            OptionLuminanceWeightB,

            OptionBlueScreenMaskR,
            OptionBlueScreenMaskG,
            OptionBlueScreenMaskB,

            OptionBlueScreenClearR,
            OptionBlueScreenClearG,
            OptionBlueScreenClearB,

            OptionFP16HDRKey,
            OptionFP16HDRShift,
            OptionFP16HDRGamma,

            OptionUnsharpenRadius,
            OptionUnsharpenAmount,
            OptionUnsharpenThreshold,

            OptionXSharpenStrength,
            OptionXSharpenThreshold,

            OptionVMTParseMode
        }

        public enum ImageFormat
        {
            ImageFormatRGBA8888 = 0,
            ImageFormatABGR8888,
            ImageFormatRGB888,
            ImageFormatBGR888,
            ImageFormatRGB565,
            ImageFormatI8,
            ImageFormatIA88,
            ImageFormatP8,
            ImageFormatA8,
            ImageFormatRGB888BlueScreen,
            ImageFormatBGR888BlueScreen,
            ImageFormatARGB8888,
            ImageFormatBGRA8888,
            ImageFormatDXT1,
            ImageFormatDXT3,
            ImageFormatDXT5,
            ImageFormatBGRX8888,
            ImageFormatBGR565,
            ImageFormatBGRX5551,
            ImageFormatBGRA4444,
            ImageFormatDXT1OneBitAlpha,
            ImageFormatBGRA5551,
            ImageFormatUV88,
            ImageFormatUVWQ8888,
            ImageFormatRGBA16161616F,
            ImageFormatRGBA16161616,
            ImageFormatUVLX8888,
            ImageFormatI32F,
            ImageFormatRGB323232F,
            ImageFormatRGBA32323232F,
            ImageFormatCount,
            ImageFormatNone = -1
        }

        public enum ImageFlag : uint
        {
            ImageFlagNone = 0x00000000,
            ImageFlagPointSample = 0x00000001,
            ImageFlagTrilinear = 0x00000002,
            ImageFlagClampS = 0x00000004,
            ImageFlagClampT = 0x00000008,
            ImageFlagAnisotropic = 0x00000010,
            ImageFlagHintDXT5 = 0x00000020,
            ImageFlagSRGB = 0x00000040,
            ImageFlagNormal = 0x00000080,
            ImageFlagNoMIP = 0x00000100,
            ImageFlagNoLOD = 0x00000200,
            ImageFlagMinMIP = 0x00000400,
            ImageFlagProcedural = 0x00000800,
            ImageFlagOneBitAlpha = 0x00001000,
            ImageFlagEightBitAlpha = 0x00002000,
            ImageFlagEnviromentMap = 0x00004000,
            ImageFlagRenderTarget = 0x00008000,
            ImageFlagDepthRenderTarget = 0x00010000,
            ImageFlagNoDebugOverride = 0x00020000,
            ImageFlagSingleCopy = 0x00040000,
            ImageFlagUnused0 = 0x00080000,
            ImageFlagUnused1 = 0x00100000,
            ImageFlagUnused2 = 0x00200000,
            ImageFlagUnused3 = 0x00400000,
            ImageFlagNoDepthBuffer = 0x00800000,
            ImageFlagUnused4 = 0x01000000,
            ImageFlagClampU = 0x02000000,
            ImageFlagVertexTexture = 0x04000000,
            ImageFlagSSBump = 0x08000000,
            ImageFlagUnused5 = 0x10000000,
            ImageFlagBorder = 0x20000000,
            ImageFlagCount = 30
        }

        public enum CubemapFace
        {
            CubemapFaceRight = 0,
            CubemapFaceLeft,
            CubemapFaceBack,
            CubemapFaceFront,
            CubemapFaceUp,
            CubemapFaceDown,
            CubemapFaceSphereMap,
            CubemapFaceCount
        }

        public enum MipmapFilter
        {
            MipmapFilterPoint = 0,
            MipmapFilterBox,
            MipmapFilterTriangle,
            MipmapFilterQuadratic,
            MipmapFilterCubic,
            MipmapFilterCatrom,
            MipmapFilterMitchell,
            MipmapFilterGaussian,
            MipmapFilterSinC,
            MipmapFilterBessel,
            MipmapFilterHanning,
            MipmapFilterHamming,
            MipmapFilterBlackman,
            MipmapFilterKaiser,
            MipmapFilterCount
        }

        public enum SharpenFilter
        {
            SharpenFilterNone = 0,
            SharpenFilterNegative,
            SharpenFilterLighter,
            SharpenFilterDarker,
            SharpenFilterContrastMore,
            SharpenFilterContrastLess,
            SharpenFilterSmoothen,
            SharpenFilterSharpenSoft,
            SharpenFilterSharpenMeium,
            SharpenFilterSharpenStrong,
            SharpenFilterFindEdges,
            SharpenFilterContour,
            SharpenFilterEdgeDetect,
            SharpenFilterEdgeDetectSoft,
            SharpenFilterEmboss,
            SharpenFilterMeanRemoval,
            SharpenFilterUnsharp,
            SharpenFilterXSharpen,
            SharpenFilterWarpSharp,
            SharpenFilterCount
        }

        public enum DXTQuality
        {
            DXTQualityLow = 0,
            DXTQualityMedium,
            DXTQualityHigh,
            DXTQualityHighest,
            DXTQualityCount
        }

// ReSharper disable InconsistentNaming
        public enum KernelFilter
        {
            KernelFilter4x = 0,
            KernelFilter3x3,
            KernelFilter5x5,
            KernelFilter7x7,
            KernelFilter9x9,
            KernelFilterDuDv,
            KernelFilterCount
        }
// ReSharper restore InconsistentNaming

        public enum HeightConversionMethod
        {
            HeightConversionMethodAlpha = 0,
            HeightConversionMethodAverageRGB,
            HeightConversionMethodBiasedRGB,
            HeightConversionMethodRed,
            HeightConversionMethodGreed,
            HeightConversionMethodBlue,
            HeightConversionMethodMaxRGB,
            HeightConversionMethodColorSspace,
            //HeightConversionMethodNormalize,
            HeightConversionMethodCount
        }

        public enum NormalAlphaResult
        {
            NormalAlphaResultNoChange = 0,
            NormalAlphaResultHeight,
            NormalAlphaResultBlack,
            NormalAlphaResultWhite,
            NormalAlphaResultCount
        }

        public enum ResizeMethod
        {
            ResizeMethodNearestPowerTwo = 0,
            ResizeMethodBiggestPowerTwo,
            ResizeMethodSmallestPowerTwo,
            ResizeMethodSet,
            ResizeMethodCount
        }

        public enum ResourceFlag : uint
        {
            ResourceFlagNoDataChunk = 0x02,
            ResourceFlagCount = 1
        }

        public enum ResourceType : uint
        {
            ResourceTypeLowResolutionImage = 0x01,
            ResourceTypeImage = 0x30,
            ResourceTypeSheet = 0x10,
            ResourceTypeCRC = 'C' | ('R' << 8) | ('C' << 24) | (ResourceFlag.ResourceFlagNoDataChunk << 32),
            ResourceTypeLODControl = 'L' | ('O' << 8) | ('D' << 24) | (ResourceFlag.ResourceFlagNoDataChunk << 32),
            ResourceTypeTextureSettingsEx = 'T' | ('S' << 8) | ('O' << 24) | (ResourceFlag.ResourceFlagNoDataChunk << 32),
            ResourceTypeKeyValueData = 'K' | ('V' << 8) | ('D' << 24)
        }

        public enum Proc
        {
            ProcReadClose = 0,
            ProcReadOpen,
            ProcReadRead,
            ProcReadSeek,
            ProcReadTell,
            ProcReadSize,
            ProcWriteClose,
            ProcWriteOpen,
            ProcWriteWrite,
            ProcWriteSeek,
            ProcWriteSize,
            ProcWriteTell
        }

        public enum SeekMode
        {
            Begin = 0,
            Current,
            End
        }
        #endregion

        #region Structures
        public const uint MaximumResources = 32;

        [StructLayout(LayoutKind.Sequential, Pack=1)]
        public struct ImageFormatInfo
        {
            //[MarshalAs(UnmanagedType.LPStr)]
            public IntPtr sName;
            public uint uiBitsPerPixel;
            public uint uiBytesPerPixel;
            public uint uiRedBitsPerPixel;
            public uint uiGreenBitsPerPixel;
            public uint uiBlueBitsPerPixel;
            public uint uiAlphaBitsPerPixel;
            [MarshalAs(UnmanagedType.U1)]
            public bool bIsCompressed;
            [MarshalAs(UnmanagedType.U1)]
            public bool bIsSupported;

            public string GetName()
            {
                return sName == IntPtr.Zero ? null : Marshal.PtrToStringAnsi(sName);
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct CreateOptions
        {
            public uint uiVersionMajor;
            public uint uiVersionMinor;
            public ImageFormat eImageFormat;

            public uint uiFlags;
            public uint uiStartFrame;
            public float fBumpScale;
            public float fRefectivityX;
            public float fRefectivityY;
            public float fRefectivityZ;

            [MarshalAs(UnmanagedType.U1)]
            public bool bMipmaps;
            public MipmapFilter eMipmapFilter;
            public SharpenFilter eSharpenFilter;

            [MarshalAs(UnmanagedType.U1)]
            public bool bThumbnail;
            [MarshalAs(UnmanagedType.U1)]
            public bool bReflectivity;

            [MarshalAs(UnmanagedType.U1)]
            public bool bResize;
            public ResizeMethod eResizeMethod;
            public MipmapFilter eResizeFilter;
            public SharpenFilter eResizeSharpenFilter;
            public uint uiResizeWidth;
            public uint uiResizeHeight;

            [MarshalAs(UnmanagedType.U1)]
            public bool bResizeClamp;
            public uint uiResizeClampWidth;
            public uint uiResizeClampHeight;

            [MarshalAs(UnmanagedType.U1)]
            public bool bGammaCorrection;
            public float fGammaCorrection;

            [MarshalAs(UnmanagedType.U1)]
            public bool bNormalMap;
            public KernelFilter eKernelFilter;
            public HeightConversionMethod eHeightConversionMethod;
            public NormalAlphaResult eNormalAlphaResult;
            public byte uiNormalMinimumZ;
            public float fNormalScale;
            [MarshalAs(UnmanagedType.U1)]
            public bool bNormalWrap;
            [MarshalAs(UnmanagedType.U1)]
            public bool bNormalInvertX;
            [MarshalAs(UnmanagedType.U1)]
            public bool bNormalInvertY;
            [MarshalAs(UnmanagedType.U1)]
            public bool bNormalInvertZ;

            [MarshalAs(UnmanagedType.U1)]
            public bool bSphereMap;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct LODControlResource
        {
            public byte uiResolutionClampU;
            public byte uiResolutionClampV;
            public byte uiPadding0;
            public byte uiPadding1;
        }
        #endregion

        #region Callback Functions
        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
        [return:MarshalAs(UnmanagedType.U1)]
        public delegate bool ReadCloseProc(IntPtr pUserData);
        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
        [return:MarshalAs(UnmanagedType.U1)]
        public delegate bool ReadOpenProc(IntPtr pUserData);
        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
        public delegate uint ReadReadProc(IntPtr lpData, uint uiBytes, IntPtr pUserData);
        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
        public delegate uint ReadSeekProc(int iOffset, SeekMode eSeekMode, IntPtr pUserData);
        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
        public delegate uint ReadSizeProc(IntPtr pUserData);
        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
        public delegate uint ReadTellProc(IntPtr pUserData);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
        [return:MarshalAs(UnmanagedType.U1)]
        public delegate bool WriteCloseProc(IntPtr pUserData);
        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
        [return:MarshalAs(UnmanagedType.U1)]
        public delegate bool WriteOpenProc(IntPtr pUserData);
        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
        public delegate uint WriteWriteProc(IntPtr lpData, uint uiBytes, IntPtr pUserData);
        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
        public delegate uint WriteSeekProc(int iOffset, SeekMode eSeekMode, IntPtr pUserData);
        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
        public delegate uint WriteSizeProc(IntPtr pUserData);
        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
        public delegate uint WriteTellProc(IntPtr pUserData);
        #endregion

        #region Functions

        //
        // VTFLib
        //

        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern uint vlGetVersion();
        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern string vlGetVersionString();

        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern string vlGetLastError();

        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U1)]
        public unsafe static extern bool vlInitialize();
        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void vlShutdown();

        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U1)]
        public unsafe static extern bool vlGetBoolean(Option eOption);
        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void vlSetBoolean(Option eOption, [MarshalAs(UnmanagedType.U1)]bool bValue);

        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern int vlGetInteger(Option eOption);
        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void vlSetInteger(Option eOption, int iValue);

        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern float vlGetFloat(Option eOption);
        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void vlSetFloat(Option eOption, float fValue);

        //
        // Proc
        //

        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void vlSetProc(Proc eProc, IntPtr pProc);
        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern IntPtr vlGetProc(Proc eProc);

        //
        // Memory managment routines.
        //

        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U1)]
        public unsafe static extern bool vlImageIsBound();
        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U1)]
        public unsafe static extern bool vlBindImage(uint uiImage);

        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U1)]
        public unsafe static extern bool vlCreateImage(uint* uiImage);
        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void vlDeleteImage(uint uiImage);

        //
        // Library routines.  (Basically class wrappers.)
        //

        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void vlImageCreateDefaultCreateStructure(out CreateOptions createOptions);

        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U1)]
        public unsafe static extern bool vlImageCreate(uint uiWidth, uint uiHeight, uint uiFrames, uint uiFaces, uint uiSlices, ImageFormat imageFormat, [MarshalAs(UnmanagedType.U1)]bool bThumbnail, [MarshalAs(UnmanagedType.U1)]bool bMipmaps, [MarshalAs(UnmanagedType.U1)]bool bNullImageData);
        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U1)]
        public unsafe static extern bool vlImageCreateSingle(uint uiWidth, uint uiHeight, byte* lpImageDataRGBA8888, ref CreateOptions createOptions);
        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U1)]
        public unsafe static extern bool vlImageCreateMultiple(uint uiWidth, uint uiHeight, uint uiFrames, uint uiFaces, uint uiSlices, byte** lpImageDataRGBA8888, ref CreateOptions createOptions);
        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void vlImageDestroy();

        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U1)]
        public unsafe static extern bool vlImageIsLoaded();

        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U1)]
        public unsafe static extern bool vlImageLoad(string sFileName, [MarshalAs(UnmanagedType.U1)]bool bHeaderOnly);
        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U1)]
        public unsafe static extern bool vlImageLoadLump(void* lpData, uint uiBufferSize, [MarshalAs(UnmanagedType.U1)]bool bHeaderOnly);
        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U1)]
        public unsafe static extern bool vlImageLoadProc(IntPtr pUserData, [MarshalAs(UnmanagedType.U1)]bool bHeaderOnly);

        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U1)]
        public unsafe static extern bool vlImageSave(string sFileName);
        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U1)]
        public unsafe static extern bool vlImageSaveLump(void* lpData, uint uiBufferSize, uint* uiSize);
        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U1)]
        public unsafe static extern bool vlImageSaveProc(IntPtr pUserData);

        //
        // Image routines.
        //

        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern uint vlImageGetHasImage();

        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern uint vlImageGetMajorVersion();
        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern uint vlImageGetMinorVersion();

        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern uint vlImageGetSize();
        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern uint vlImageGetWidth();
        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern uint vlImageGetHeight();
        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern uint vlImageGetDepth();

        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern uint vlImageGetFrameCount();
        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern uint vlImageGetFaceCount();
        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern uint vlImageGetMipmapCount();

        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern uint vlImageGetStartFrame();
        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void vlImageSetStartFrame(uint uiStartFrame);

        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern uint vlImageGetFlags();
        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void vlImageSetFlags(uint uiFlags);

        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U1)]
        public unsafe static extern bool vlImageGetFlag(ImageFlag imageFlag);
        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void vlImageSetFlag(ImageFlag imageFlag, [MarshalAs(UnmanagedType.U1)]bool bState);

        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern float vlImageGetBumpmapScale();
        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void vlImageSetBumpmapScale(float fBumpmapScale);

        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void vlImageGetReflectivity(float* fRed, float* fGreen, float* fBlue);
        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void vlImageSetReflectivity(float fRed, float fGreen, float fBlue);

        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern ImageFormat vlImageGetFormat();

        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern byte* vlImageGetData(uint uiFrame, uint uiFace, uint uiSlice, uint uiMipmapLevel);
        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void vlImageSetData(uint uiFrame, uint uiFace, uint uiSlice, uint uiMipmapLevel, byte* lpData);

        //
        // Thumbnail routines.
        //

        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U1)]
        public unsafe static extern bool vlImageGetHasThumbnail();

        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern uint vlImageGetThumbnailWidth();
        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern uint vlImageGetThumbnailHeight();

        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern ImageFormat vlImageGetThumbnailFormat();

        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern byte* vlImageGetThumbnailData();
        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void vlImageSetThumbnailData(byte* lpData);

        //
        // Resource routines.
        //

        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U1)]
        public unsafe static extern bool vlImageGetSupportsResources();

        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern uint vlImageGetResourceCount();
        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern uint vlImageGetResourceType(uint uiIndex);
        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U1)]
        public unsafe static extern bool vlImageGetHasResource(ResourceType resourceType);

        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void* vlImageGetResourceData(ResourceType resourceType, uint* uiSize);
        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void* vlImageSetResourceData(ResourceType resourceType, uint uiSize, void* lpData);

        //
        // Helper routines.
        //

        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U1)]
        public unsafe static extern bool vlImageGenerateMipmaps(uint uiFace, uint uiFrame, MipmapFilter mipmapFilter, SharpenFilter sharpenFilter);
        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U1)]
        public unsafe static extern bool vlImageGenerateAllMipmaps(MipmapFilter mipmapFilter, SharpenFilter sharpenFilter);

        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U1)]
        public unsafe static extern bool vlImageGenerateThumbnail();

        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U1)]
        public unsafe static extern bool vlImageGenerateNormalMap(uint uiFrame, KernelFilter kernelFilter, HeightConversionMethod heightConversionMethod, NormalAlphaResult normalAlphaResult);
        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U1)]
        public unsafe static extern bool vlImageGenerateAllNormalMaps(KernelFilter kernelFilter, HeightConversionMethod heightConversionMethod, NormalAlphaResult normalAlphaResult);

        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U1)]
        public unsafe static extern bool vlImageGenerateSphereMap();

        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U1)]
        public unsafe static extern bool vlImageComputeReflectivity();

        //
        // Conversion routines.
        //

        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U1)]
        public unsafe static extern bool vlImageGetImageFormatInfoEx(ImageFormat imageFormat, out ImageFormatInfo imageFormatInfo);

        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern uint vlImageComputeImageSize(uint uiWidth, uint uiHeight, uint uiDepth, uint uiMipmaps, ImageFormat imageFormat);

        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern uint vlImageComputeMipmapCount(uint uiWidth, uint uiHeight, uint uiDepth);
        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void vlImageComputeMipmapDimensions(uint uiWidth, uint uiHeight, uint uiDepth, uint uiMipmapLevel, uint* uiMipmapWidth, uint* uiMipmapHeight, uint* uiMipmapDepth);
        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern uint vlImageComputeMipmapSize(uint uiWidth, uint uiHeight, uint uiDepth, uint uiMipmapLevel, ImageFormat imageFormat);

        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U1)]
        public unsafe static extern bool vlImageConvert(byte* lpSource, byte* lpDest, uint uiWidth, uint uiHeight, ImageFormat sourceFormat, ImageFormat destFormat);

        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U1)]
        public unsafe static extern bool vlImageConvertToNormalMap(byte* lpSourceRGBA8888, byte* lpDestRGBA8888, uint uiWidth, uint uiHeight, KernelFilter kernelFilter, HeightConversionMethod heightConversionMethod, NormalAlphaResult normalAlphaResult, byte bMinimumZ, float fScale, [MarshalAs(UnmanagedType.U1)]bool bWrap, [MarshalAs(UnmanagedType.U1)]bool bInvertX, [MarshalAs(UnmanagedType.U1)]bool bInvertY);

        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.U1)]
        public unsafe static extern bool vlImageResize(byte* lpSourceRGBA8888, byte* lpDestRGBA8888, uint uiSourceWidth, uint uiSourceHeight, uint uiDestWidth, uint uiDestHeight, MipmapFilter resizeFilter, SharpenFilter sharpenFilter);

        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void vlImageCorrectImageGamma(byte* lpImageDataRGBA8888, uint uiWidth, uint uiHeight, float fGammaCorrection);
        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void vlImageComputeImageReflectivity(byte* lpImageDataRGBA8888, uint uiWidth, uint uiHeight, float* sX, float* sY, float* sZ);

        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void vlImageFlipImage(byte* lpImageDataRGBA8888, uint uiWidth, uint uiHeight);
        [DllImport("VTFLib.x86.dll", CallingConvention = CallingConvention.Cdecl)]
        public unsafe static extern void vlImageMirrorImage(byte* lpImageDataRGBA8888, uint uiWidth, uint uiHeight);

        #endregion
    }
}

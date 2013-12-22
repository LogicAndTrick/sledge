using System;
using System.Collections.Generic;
using System.IO;
using SysIO = System.IO;
using System.Runtime.InteropServices;

namespace Sledge.Libs.HLLib
{
    public static class HLLib
    {
        #region Constants
        public const int VersionNumber = ((2 << 24) | (3 << 16) | (0 << 8) | 0);
        public const string VersionString = "2.3.0";

        public const uint IDInvalid = 0xffffffff;

        public const uint DefaultPackageTestBufferSize = 8;
        public const uint DefaultViewSize = 131072;
        public const uint DefaultCopyBufferSize = 131072;
        #endregion

        #region Enumerations
        public enum Option
        {
            Version = 0,
            Error,
            ErrorSystem,
            ErrorShortFormated,
            ErrorLongFormated,
            ProcOpen,
            ProcClose,
            ProcRead,
            ProcWrite,
            ProcSeek,
            ProcTell,
            ProcSize,
            ProcExtractItemStart,
            ProcExtractItemEnd,
            ProcExtractFileProgress,
            ProcValidateFileProgress,
            OverwriteFiles,
            PackageBound,
            PackageID,
            PackageSize,
            PackageTotalAllocations,
            PackageTotalMemoryAllocated,
            PackageTotalMemoryUsed,
            ReadEncrypted,
            ForceDefragment,
            ProcDefragmentProgress,
            ProcDefragmentProgressEx
        }

        public enum FileMode
        {
            Invalid = 0x00,
            Read = 0x01,
            Write = 0x02,
            Create = 0x04,
            Volatile = 0x08,
            NoFilemapping = 0x10,
            QuickFilemapping = 0x20,
            ReadVolatile = Read | Volatile
        }

        public enum SeekMode
        {
            Beginning = 0,
            Current,
            End
        }

        public enum DirectoryItemType
        {
            None = 0,
            Folder,
            File
        }

        public enum SortOrder
        {
            Ascending = 0,
            Descending
        }

        public enum SortField
        {
            Name = 0,
            Size
        }

        public enum FindType
        {
            Files = 0x01,
            Folders = 0x02,
            NoRecurse = 0x04,
            CaseSensitive = 0x08,
            ModeString = 0x10,
            ModeSubstring = 0x20,
            ModeWildcard = 0x00,
            All = Files | Folders
        }

        public enum StreamType
        {
            None = 0,
            File,
            GCF,
            Mapping,
            Memory,
            Proc,
            Null
        }

        public enum MappingType
        {
            None = 0,
            File,
            Memory,
            Stream
        }

        public enum PackageType
        {
            None = 0,
            BSP,
            GCF,
            PAK,
            VBSP,
            WAD,
            XZP,
            ZIP,
            NCF,
            VPK
        }

        public enum AttributeType
        {
            Invalid = 0,
            Boolean,
            Integer,
            UnsignedInteger,
            Float,
            String
        }

        public enum PackageAttribute
        {
            BSPPackageVersion = 0,
            BSPPackageCount,
            BSPItemWidth = 0,
            BSPItemHeight,
            BSPItemPaletteEntries,
            BSPItemCount,

            GCFPackageVersion = 0,
            GCFPackageID,
            GCFPackageAllocatedBlocks,
            GCFPackageUsedBlocks,
            GCFPackageBlockLength,
            GCFPackageLastVersionPlayed,
            GCFPackageCount,
            GCFItemEncrypted = 0,
            GCFItemCopyLocal,
            GCFItemOverwriteLocal,
            GCFItemBackupLocal,
            GCFItemFlags,
            GCFItemFragmentation,
            GCFItemCount,

            NCFPackageVersion = 0,
            NCFPackageID,
            NCFPackageLastVersionPlayed,
            NCFPackageCount,
            NCFItemEncrypted = 0,
            NCFItemCopyLocal,
            NCFItemOverwriteLocal,
            NCFItemBackupLocal,
            NCFItemFlags,
            NCFItemCount,

            PAKPackageCount = 0,
            PAKItemCount = 0,

            VBSPPackageVersion = 0,
            VBSPPackageMapRevision,
            VBSPPackageCount,
            VBSPItemVersion = 0,
            VBSPItemFourCc,
            VBSPZIPPackageDisk,
            VBSPZIPPackageComment,
            VBSPZIPItemCreateVersion,
            VBSPZIPItemExtractVersion,
            VBSPZIPItemFlags,
            VBSPZIPItemCompressionMethod,
            VBSPZIPItemCRC,
            VBSPZIPItemDisk,
            VBSPZIPItemComment,
            VBSPItemCount,

            VPKPackageCount = 0,
            VPKItemPreloadBytes = 0,
            VPKItemArchive,
            VPKItemCRC,
            VPKItemCount,

            WADPackageVersion = 0,
            WADPackageCount,
            WADItemWidth = 0,
            WADItemHeight,
            WADItemPaletteEntries,
            WADItemMipmaps,
            WADItemCompressed,
            WADItemType,
            WADItemCount,

            XZPPackageVersion = 0,
            XZPPackagePreloadBytes,
            XZPPackageCount,
            XZPItemCreated = 0,
            XZPItemPreloadBytes,
            XZPItemCount,

            ZIPPackageDisk = 0,
            ZIPPackageComment,
            ZIPPackageCount,
            ZIPItemCreateVersion = 0,
            ZIPItemExtractVersion,
            ZIPItemFlags,
            ZIPItemCompressionMethod,
            ZIPItemCRC,
            ZIPItemDisk,
            ZIPItemComment,
            ZIPItemCount
        }

        public enum Validation
        {
            Ok = 0,
            AssumedOk,
            Incomplete,
            Corrupt,
            Canceled,
            Error
        }
        #endregion

        #region Structures
        [StructLayout(LayoutKind.Explicit)]
        internal struct AttributeStruct
        {
            [FieldOffset(0)]
            public AttributeType Type;

            [FieldOffset(4)]
            [MarshalAs(UnmanagedType.ByValArray, SizeConst=252)]
            public char[] Name;

            [FieldOffset(256)]
            [MarshalAs(UnmanagedType.ByValArray, SizeConst=256)]
            public byte[] Value;
        }
        #endregion

        #region Callback Functions
        //
        // Important: Callback functions cannot use IntPtr.  Instead, I use [MarshalAs(UnmanagedType.I4)]int.
        // Convert IntPtr objects using IntPtr.ToInt32().  Convert int objects to IntPtr using new IntPtr(int).
        // This obviously only works on 32 bit builds of HLLib.
        //

        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
        [return:MarshalAs(UnmanagedType.U1)]
        public delegate bool HLOpenProc(uint uiMode, [MarshalAs(UnmanagedType.I4)]int pUserData);
        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
        [return:MarshalAs(UnmanagedType.U1)]
        public delegate bool HLCloseProc([MarshalAs(UnmanagedType.I4)]int pUserData);
        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
        public delegate uint HLReadProc([MarshalAs(UnmanagedType.I4)]int lpData, uint uiBytes, [MarshalAs(UnmanagedType.I4)]int pUserData);
        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
        public delegate uint HLWriteProc([MarshalAs(UnmanagedType.I4)]int lpData, uint uiBytes, [MarshalAs(UnmanagedType.I4)]int pUserData);
        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
        public delegate uint HLSeekProc(Int64 iOffset, SeekMode eSeekMode, [MarshalAs(UnmanagedType.I4)]int pUserData);
        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
        public delegate uint HLTellProc([MarshalAs(UnmanagedType.I4)]int pUserData);
        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
        public delegate uint HLSizeProc([MarshalAs(UnmanagedType.I4)]int pUserData);

        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
        public delegate void HLExtractItemStartProc([MarshalAs(UnmanagedType.I4)]int pItem);
        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
        public delegate void HLExtractItemEndProc([MarshalAs(UnmanagedType.I4)]int pItem, [MarshalAs(UnmanagedType.U1)]bool bSuccess);
        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
        public delegate void HLExtractFileProgressProc([MarshalAs(UnmanagedType.I4)]int pFile, uint uiBytesExtracted, uint uiBytesTotal, [MarshalAs(UnmanagedType.U1)]ref bool bCancel);
        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
        public delegate void HLValidateFileProgressProc(int pFile, uint uiBytesValidated, uint uiBytesTotal, [MarshalAs(UnmanagedType.U1)]ref bool pCancel);
        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
        public delegate void HLDefragmentFileProgressProc([MarshalAs(UnmanagedType.I4)]int pFile, uint uiFilesDefragmented, uint uiFilesTotal, uint uiBytesDefragmented, uint uiBytesTotal, [MarshalAs(UnmanagedType.U1)]ref bool pCancel);
        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
        public delegate void HLDefragmentFileProgressExProc([MarshalAs(UnmanagedType.I4)]int pFile, uint uiFilesDefragmented, uint uiFilesTotal, UInt64 uiBytesDefragmented, UInt64 uiBytesTotal, [MarshalAs(UnmanagedType.U1)]ref bool pCancel);
        #endregion

        #region Functions
        //
        // HLLib
        //

        [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlInitialize")]
        public static extern void Initialize();
        [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlShutdown")]
        public static extern void Shutdown();

        //
        // Get/Set
        //

        [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
        [return:MarshalAs(UnmanagedType.U1)]
        public static extern bool hlGetBoolean(Option eOption);
        [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
        [return:MarshalAs(UnmanagedType.U1)]
        public static extern bool hlGetBooleanValidate(Option eOption, [MarshalAs(UnmanagedType.U1)]out bool pValue);
        [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void hlSetBoolean(Option eOption, [MarshalAs(UnmanagedType.U1)]bool bValue);

        [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int hlGetInteger(Option eOption);
        [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
        [return:MarshalAs(UnmanagedType.U1)]
        public static extern bool hlGetIntegerValidate(Option eOption, out int pValue);
        [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void hlSetInteger(Option eOption, int iValue);

        [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint hlGetUnsignedInteger(Option eOption);
        [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
        [return:MarshalAs(UnmanagedType.U1)]
        public static extern bool hlGetUnsignedIntegerValidate(Option eOption, out uint pValue);
        [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void hlSetUnsignedInteger(Option eOption, uint uiValue);

        [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern float hlGetFloat(Option eOption);
        [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
        [return:MarshalAs(UnmanagedType.U1)]
        public static extern bool hlGetFloatValidate(Option eOption, out float pValue);
        [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void hlSetFloat(Option eOption, float pValue);

        [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
        //[return: MarshalAs(UnmanagedType.AnsiBStr)]
        public static extern string hlGetString(Option eOption);
        [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
        [return:MarshalAs(UnmanagedType.U1)]
        public static extern bool hlGetStringValidate(Option eOption, out string pValue);
        [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void hlSetString(Option eOption, string lpValue);

        [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr hlGetVoid(Option eOption);
        [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
        [return:MarshalAs(UnmanagedType.U1)]
        public static extern bool hlGetVoidValidate(Option eOption, out IntPtr pValue);
        [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void hlSetVoid(Option eOption, IntPtr lpValue);

        public class Attribute
        {
            private AttributeStruct _attr;

            internal Attribute(AttributeStruct attr)
            {
                _attr = attr;
            }

            public string GetName()
            {
                var iLength = 0;
                while (_attr.Name[iLength] != 0)
                {
                    iLength++;
                }
                try
                {
                    return new string(_attr.Name, 0, iLength);
                }
                catch
                {
                    return string.Empty;
                }
            }

            public object GetData()
            {
                switch (_attr.Type)
                {
                    case AttributeType.Boolean:
                        return GetBoolean(ref _attr);
                    case AttributeType.Integer:
                        return GetInteger(ref _attr);
                    case AttributeType.UnsignedInteger:
                        return GetUnsignedInteger(ref _attr);
                    case AttributeType.Float:
                        return GetFloat(ref _attr);
                    case AttributeType.String:
                        return GetString(ref _attr);
                    default:
                        return null;
                }
            }

            public bool GetBool()
            {
                return (bool)GetData();
            }

            public int GetInt()
            {
                return (int)GetData();
            }

            public uint GetUInt()
            {
                return (uint)GetData();
            }

            public float GetFloat()
            {
                return (float)GetData();
            }

            public string GetString()
            {
                return (string)GetData();
            }

            public override string ToString()
            {
                switch (_attr.Type)
                {
                    case AttributeType.Boolean:
                        return GetBoolean(ref _attr).ToString();
                    case AttributeType.Integer:
                        return GetInteger(ref _attr).ToString("#,##0");
                    case AttributeType.UnsignedInteger:
                        if (_attr.Value[4] == 0)
                        {
                            return GetUnsignedInteger(ref _attr).ToString("#,##0");
                        }
                        else // Display as hexadecimal.
                        {
                            return "0x" + GetUnsignedInteger(ref _attr).ToString("x8");
                        }
                    case AttributeType.Float:
                        return GetFloat(ref _attr).ToString("#,##0.00000000");
                    case AttributeType.String:
                        return GetString(ref _attr);
                    default:
                        return string.Empty;
                }
            }

            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlAttributeGetBoolean")]
            [return: MarshalAs(UnmanagedType.U1)]
            private static extern bool GetBoolean(ref AttributeStruct pAttributeStruct);

            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlAttributeSetBoolean")]
            private static extern void SetBoolean(ref AttributeStruct pAttributeStruct, string lpName, [MarshalAs(UnmanagedType.U1)]bool bValue);

            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlAttributeGetInteger")]
            private static extern int GetInteger(ref AttributeStruct pAttributeStruct);

            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlAttributeSetInteger")]
            private static extern void SetInteger(ref AttributeStruct pAttributeStruct, string lpName, int iValue);

            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlAttributeGetUnsignedInteger")]
            private static extern uint GetUnsignedInteger(ref AttributeStruct pAttributeStruct);

            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SetUnsignedInteger")]
            private static extern void SetUnsignedInteger(ref AttributeStruct pAttributeStruct, string lpName, uint uiValue, [MarshalAs(UnmanagedType.U1)]bool bHexadecimal);

            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlAttributeGetFloat")]
            private static extern float GetFloat(ref AttributeStruct pAttributeStruct);

            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlAttributeSetFloat")]
            private static extern void SetFloat(ref AttributeStruct pAttributeStruct, string lpName, float fValue);

            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlAttributeGetString")]
            private static extern string GetString(ref AttributeStruct pAttributeStruct);

            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlAttributeSetString")]
            private static extern void SetString(ref AttributeStruct pAttributeStruct, string lpName, string lpValue);
        }

        public class Item
        {
            protected internal IntPtr ItemPtr { get; protected set; }

            internal Item(IntPtr item)
            {
                ItemPtr = item;
            }

            public bool Exists { get { return ItemPtr != IntPtr.Zero; } }
            public DirectoryItemType Type { get { return GetType(ItemPtr); } }
            public string Name { get { return GetName(ItemPtr); } }
            public uint ID { get { return GetID(ItemPtr); } }
            public IntPtr Data { get { return GetData(ItemPtr); } }
            public uint PackageID { get { return GetPackage(ItemPtr); } }
            public IntPtr Parent { get { return GetParent(ItemPtr); } }
            public uint Size { get { uint size; return (GetSize(ItemPtr, out size) ? size : 0); } }
            public ulong SizeEx { get { ulong size; return (GetSizeEx(ItemPtr, out size) ? size : 0); } }
            public uint SizeOnDisk { get { uint size; return (GetSizeOnDisk(ItemPtr, out size) ? size : 0); } }
            public ulong SizeOnDiskEx { get { ulong size; return (GetSizeOnDiskEx(ItemPtr, out size) ? size : 0); } }

            public bool GetPath(IntPtr path, uint pathSize)
            {
                return GetPath(ItemPtr, path, pathSize);
            }

            public bool Extract(string path)
            {
                return Extract(ItemPtr, path);
            }

            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlItemGetType")]
            protected static extern DirectoryItemType GetType(IntPtr pItem);

            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlItemGetName")]
            //[return: MarshalAs(UnmanagedType.AnsiBStr)]
            // There's as reason we're not using AnsiBStr anymore, see below.
            protected static extern unsafe byte* GetNameBytes(IntPtr pItem);

            protected static string GetName(IntPtr item)
            {
                // VS2013 hates marshalling as an AnsiBStr for some reason.
                // When in debug mode, it just craps out when it tries. No error, nothing. When running without debug, it works fine.
                // That's annoying, so this is the work around - get the pointer, loop until we find the null terminator, create a string from it.
                // Unsafe, but uncrash-y...
                unsafe
                {
                    var gn2 = GetNameBytes(item);
                    var bytes = new List<byte>();
                    for (var i = 0; gn2[i] > 0; i++)
                    {
                        bytes.Add(gn2[i]);
                    }
                    return new string(System.Text.Encoding.ASCII.GetChars(bytes.ToArray()));
                }
            }

            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlItemGetID")]
            protected static extern uint GetID(IntPtr pItem);

            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlItemGetData")]
            protected static extern IntPtr GetData(IntPtr pItem);

            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlItemGetPackage")]
            protected static extern uint GetPackage(IntPtr pItem);

            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlItemGetParent")]
            protected static extern IntPtr GetParent(IntPtr pItem);

            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlItemGetSize")]
            [return:MarshalAs(UnmanagedType.U1)]
            protected static extern bool GetSize(IntPtr pItem, out uint pSize);

            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlItemGetSizeEx")]
            [return:MarshalAs(UnmanagedType.U1)]
            protected static extern bool GetSizeEx(IntPtr pItem, out UInt64 pSize);

            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlItemGetSizeOnDisk")]
            [return:MarshalAs(UnmanagedType.U1)]
            protected static extern bool GetSizeOnDisk(IntPtr pItem, out uint pSize);

            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlItemGetSizeOnDiskEx")]
            [return:MarshalAs(UnmanagedType.U1)]
            protected static extern bool GetSizeOnDiskEx(IntPtr pItem, out UInt64 pSize);

            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlItemGetPath")]
            [return:MarshalAs(UnmanagedType.U1)]
            protected static extern bool GetPath(IntPtr pItem, IntPtr lpPath, uint uiPathSize);

            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlItemExtract")]
            [return:MarshalAs(UnmanagedType.U1)]
            protected static extern bool Extract(IntPtr pItem, string lpPath);

            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlPackageGetItemAttributeCount")]
            protected static extern uint GetItemAttributeCount();

            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlPackageGetItemAttributeName")]
            protected static extern string GetPackageItemAttributeName(PackageAttribute eAttribute);

            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlPackageGetItemAttribute")]
            [return: MarshalAs(UnmanagedType.U1)]
            internal static extern bool GetItemAttribute(IntPtr pItem, PackageAttribute eAttribute, out AttributeStruct pAttributeStruct);
        }

        public class Folder : Item
        {
            public Folder(Item item) : base(item.ItemPtr)
            {
                
            }

            internal Folder(IntPtr folder) : base(folder)
            {
            }

            public IEnumerable<Item> GetItems()
            {
                for (uint i = 0; i < ItemCount; i++)
                {
                    yield return GetItem(i);
                }
            }

            public uint ItemCount { get { return GetCount(ItemPtr); } }

            public Item GetItem(uint index)
            {
                return new Item(GetItem(ItemPtr, index));
            }

            public Item GetItemByName(string name, FindType find)
            {
                return new Item(GetItemByName(ItemPtr, name, find));
            }

            public Item GetItemByPath(string path, FindType find)
            {
                return new Item(GetItemByPath(ItemPtr, path, find));
            }

            public void Sort(SortField field, SortOrder order, bool recurse)
            {
                Sort(ItemPtr, field, order, recurse);
            }

            public Item FindFirst(string search, FindType find)
            {
                return new Item(FindFirst(ItemPtr, search, find));
            }

            public Item FindNext(Item item, string search, FindType find)
            {
                return new Item(FindNext(ItemPtr, item.ItemPtr, search, find));
            }

            public uint GetSize(bool recurse)
            {
                return GetSize(ItemPtr, recurse);
            }

            public ulong GetSizeEx(bool recurse)
            {
                return GetSizeEx(ItemPtr, recurse);
            }

            public uint GetSizeOnDisk(bool recurse)
            {
                return GetSizeOnDisk(ItemPtr, recurse);
            }

            public ulong GetSizeOnDiskEx(bool recurse)
            {
                return GetSizeOnDiskEx(ItemPtr, recurse);
            }

            public uint GetFileCount(bool recurse)
            {
                return GetFileCount(ItemPtr, recurse);
            }

            public uint GetFolderCount(bool recurse)
            {
                return GetFolderCount(ItemPtr, recurse);
            }

            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlFolderGetCount")]
            protected static extern uint GetCount(IntPtr pItem);

            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlFolderGetItem")]
            protected static extern IntPtr GetItem(IntPtr pItem, uint uiIndex);

            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlFolderGetItemByName")]
            protected static extern IntPtr GetItemByName(IntPtr pItem, string lpName, FindType eFind);

            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlFolderGetItemByPath")]
            protected static extern IntPtr GetItemByPath(IntPtr pItem, string lpPath, FindType eFind);

            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlFolderSort")]
            protected static extern void Sort(IntPtr pItem, SortField eField, SortOrder eOrder, [MarshalAs(UnmanagedType.U1)]bool bRecurse);

            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlFolderFindFirst")]
            protected static extern IntPtr FindFirst(IntPtr pFolder, string lpSearch, FindType eFind);

            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlFolderFindNext")]
            protected static extern IntPtr FindNext(IntPtr pFolder, IntPtr pItem, string lpSearch, FindType eFind);

            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlFolderGetSize")]
            protected static extern uint GetSize(IntPtr pItem, [MarshalAs(UnmanagedType.U1)]bool bRecurse);

            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlFolderGetSizeEx")]
            protected static extern UInt64 GetSizeEx(IntPtr pItem, [MarshalAs(UnmanagedType.U1)]bool bRecurse);

            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlFolderGetSizeOnDisk")]
            protected static extern uint GetSizeOnDisk(IntPtr pItem, [MarshalAs(UnmanagedType.U1)]bool bRecurse);

            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlFolderGetSizeOnDiskEx")]
            protected static extern UInt64 GetSizeOnDiskEx(IntPtr pItem, [MarshalAs(UnmanagedType.U1)]bool bRecurse);

            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlFolderGetFolderCount")]
            protected static extern uint GetFolderCount(IntPtr pItem, [MarshalAs(UnmanagedType.U1)]bool bRecurse);

            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlFolderGetFileCount")]
            protected static extern uint GetFileCount(IntPtr pItem, [MarshalAs(UnmanagedType.U1)] bool bRecurse);
        }

        public class File : Item
        {
            public File(Item item) : base (item.ItemPtr)
            {
                
            }

            internal File(IntPtr file) : base(file)
            {
            }

            public uint Extractable { get { return GetExtractable(ItemPtr); } }
            public new uint Size { get { return GetSize(ItemPtr); } }
            public new uint SizeOnDisk { get { return GetSizeOnDisk(ItemPtr); } }

            public Validation GetValidation()
            {
                return GetValidation(ItemPtr);
            }

            /// <summary>
            /// Open this file as a HLLib package. This will open a stream
            /// which will be closed when the package is disposed.
            /// </summary>
            /// <returns>The opened package.</returns>
            public Package OpenPackage()
            {
                return new Package(CreateStream());
            }

            public HLLibFileStream CreateStream()
            {
                return new HLLibFileStream(ItemPtr);
            }

            public class HLLibFileStream : Stream
            {
                internal IntPtr FilePtr { get; private set; }

                internal HLLibFileStream(IntPtr file)
                {
                    FilePtr = file;
                    IntPtr stream;
                    if (!CreateStream(FilePtr, out stream))
                    {
                        throw new Exception("Unable to create stream.");
                    }
                    StreamPtr = stream;
                    Open((uint) FileMode.ReadVolatile);
                }

                protected override void Dispose(bool disposing)
                {
                    Close();
                    ReleaseStream(FilePtr, StreamPtr);
                    base.Dispose(disposing);
                }
            }

            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlFileGetExtractable")]
            protected static extern uint GetExtractable(IntPtr pItem);

            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlFileGetValidation")]
            protected static extern Validation GetValidation(IntPtr pItem);

            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlFileGetSize")]
            protected static extern uint GetSize(IntPtr pItem);

            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlFileGetSizeOnDisk")]
            protected static extern uint GetSizeOnDisk(IntPtr pItem);

            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlFileCreateStream")]
            [return: MarshalAs(UnmanagedType.U1)]
            protected static extern bool CreateStream(IntPtr pItem, out IntPtr pStream);

            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlFileReleaseStream")]
            protected static extern void ReleaseStream(IntPtr pItem, IntPtr pStream);
        }

        public class Stream : System.IO.Stream
        {
            protected internal IntPtr StreamPtr { get; protected set; }

            internal Stream(IntPtr streamPtr)
            {
                StreamPtr = streamPtr;
            }

            protected Stream()
            {
                // Used for file stream above
            }

            public StreamType Type { get { return GetType(StreamPtr); } }
            public bool IsOpened { get { return GetOpened(StreamPtr); } }
            public uint Mode { get { return GetMode(StreamPtr); } }
            public uint Size { get { return GetStreamSize(StreamPtr); } }
            //public uint Pointer { get { return GetStreamPointer(StreamPtr); } }
            public uint Pointer { get { return (uint) StreamPtr; } }

            public bool Open(uint mode)
            {
                return IsOpened || Open(StreamPtr, mode);
            }

            public uint Seek(long offset, SeekMode mode)
            {
                return Seek(StreamPtr, offset, mode);
            }

            public char ReadChar()
            {
                char c;
                if (!ReadChar(StreamPtr, out c))
                {
                    throw new Exception("Unable to read character.");
                }
                return c;
            }

            public byte[] ReadAll()
            {
                return Read(Size);
            }

            public byte[] Read(uint size)
            {
                var buffer = new byte[size];
                var allocation = Marshal.AllocHGlobal((int)size);
                try
                {
                    Read(StreamPtr, allocation, size);
                    Marshal.Copy(allocation, buffer, 0, (int)size);
                }
                finally
                {
                    Marshal.FreeHGlobal(allocation);
                }
                return buffer;
            }

            private bool _disposed = false;
            protected override void Dispose(bool disposing)
            {
                if (_disposed || !IsOpened) return;
                _disposed = true;
                Close(StreamPtr);
                base.Dispose(disposing);
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                var mode = SeekMode.Beginning;
                if (origin == SeekOrigin.Current) mode = SeekMode.Current;
                if (origin == SeekOrigin.End) mode = SeekMode.End;
                return Seek(offset, mode);
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                Seek(offset, SeekMode.Current);
                var read = Read((uint) count);
                Array.Copy(read, buffer, read.Length);
                return read.Length;
            }

            public override bool CanRead
            {
                get { return true; }
            }

            public override bool CanSeek
            {
                get { return true; }
            }

            public override bool CanWrite
            {
                get { return false; }
            }

            public override long Length
            {
                get { return Size; }
            }

            public override long Position
            {
                get { return GetStreamPointer(StreamPtr); }
                set { Seek(value, SeekMode.Beginning); }
            }

            public override void Flush()
            {
                throw new NotImplementedException();
            }

            public override void SetLength(long value)
            {
                throw new NotImplementedException();
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                throw new NotImplementedException();
            }

            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlStreamGetType")]
            private static extern StreamType GetType(IntPtr pStream);

            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlStreamGetOpened")]
            [return: MarshalAs(UnmanagedType.U1)]
            private static extern bool GetOpened(IntPtr pStream);

            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlStreamGetMode")]
            private static extern uint GetMode(IntPtr pStream);

            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlStreamOpen")]
            [return: MarshalAs(UnmanagedType.U1)]
            private static extern bool Open(IntPtr pStream, uint uiMode);

            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlStreamClose")]
            private static extern void Close(IntPtr pStream);

            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlStreamGetStreamSize")]
            private static extern uint GetStreamSize(IntPtr pStream);

            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlStreamGetStreamPointer")]
            private static extern uint GetStreamPointer(IntPtr pStream);

            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlStreamSeek")]
            private static extern uint Seek(IntPtr pStream, Int64 iOffset, SeekMode eSeekMode);

            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlStreamReadChar")]
            [return: MarshalAs(UnmanagedType.U1)]
            private static extern bool ReadChar(IntPtr pStream, out char pChar);

            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlStreamRead")]
            private static extern uint Read(IntPtr pStream, IntPtr lpData, uint uiBytes);

            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlStreamWriteChar")]
            [return: MarshalAs(UnmanagedType.U1)]
            private static extern bool WriteChar(IntPtr pStream, char iChar);

            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlStreamWrite")]
            private static extern uint Write(IntPtr pStream, IntPtr lpData, uint uiBytes);
        }

        public class Package : IDisposable
        {
            public PackageType Type { get; private set; }

            internal uint PackageID { get; private set; }
            internal IntPtr PackageRootPtr { get; private set; }
            internal string Path { get; private set; }
            internal Stream Stream { get; private set; }

            /// <summary>
            /// Open a package from the Windows filesystem.
            /// </summary>
            /// <param name="path">The package to open</param>
            public Package(string path)
            {
                Path = path;
                Type = GetTypeFromPath(path);

                if (Type == PackageType.None)
                {
                    throw new Exception("Unsupported package type.");
                }

                uint packid;
                Create(Type, out packid);
                PackageID = packid;
                Open();
            }

            /// <summary>
            /// Open a package from a Stream. The stream will be disposed if the package is.
            /// </summary>
            /// <param name="stream">The Stream to open</param>
            public Package(Stream stream)
            {
                Stream = stream;
                Type = GetTypeFromStream(stream);

                if (Type == PackageType.None)
                {
                    throw new Exception("Unsupported package type.");
                }

                uint packid;
                Create(Type, out packid);
                PackageID = packid;
                Open();
            }

            public string Extension { get { Bind(PackageID); return GetExtension(); } }
            public string Description { get { Bind(PackageID); return GetDescription(); } }
            public bool IsOpen { get { Bind(PackageID); return IsOpened(); } }
            public uint AttributeCount { get { Bind(PackageID); return GetAttributeCount(); } }
            public uint ItemAttributeCount { get { Bind(PackageID); return GetItemAttributeCount(); } }

            public void Dispose()
            {
                if (Stream != null) Stream.Dispose();
                Close();
                Delete(PackageID);
            }

            public void Open()
            {
                Bind(PackageID);
                if (IsOpened()) return;
                if (Stream == null)
                {
                    if (OpenFile(Path, (uint) FileMode.ReadVolatile))
                    {
                        PackageRootPtr = GetRoot();
                        return;
                    }
                }
                else
                {
                    if (OpenStream(new IntPtr(Stream.Pointer), (uint) FileMode.ReadVolatile))
                    {
                        PackageRootPtr = GetRoot();
                        return;
                    }
                }
                Delete(PackageID);
                throw new Exception("Unable to open file.");
            }

            public void Close()
            {
                Bind(PackageID);
                if (!IsOpened()) return;
                ClosePackage();
            }

            public Folder GetRootFolder()
            {
                return new Folder(PackageRootPtr);
            }

            public string GetAttributeName(PackageAttribute attribute)
            {
                Bind(PackageID);
                return GetPackageAttributeName(attribute);
            }

            public Attribute GetAttribute(PackageAttribute attribute)
            {
                Bind(PackageID);
                AttributeStruct attr;
                if (!GetAttribute(attribute, out attr))
                {
                    throw new Exception("Unable to get attribute.");
                }
                return new Attribute(attr);
            }

            public string GetItemAttributeName(PackageAttribute attribute)
            {
                Bind(PackageID);
                return GetPackageItemAttributeName(attribute);
            }

            public Attribute GetItemAttribute(Item item, PackageAttribute attribute)
            {
                Bind(PackageID);
                AttributeStruct attr;
                if (!GetItemAttribute(item.ItemPtr, attribute, out attr))
                {
                    throw new Exception("Unable to get item attribute.");
                }
                return new Attribute(attr);
            }

            public uint GetFileSize(File file)
            {
                Bind(PackageID);
                uint size;
                if (!GetFileSize(file.ItemPtr, out size))
                {
                    throw new Exception("Unable to get file size.");
                }
                return size;
            }

            public uint GetFileSizeOnDisk(File file)
            {
                Bind(PackageID);
                uint size;
                if (!GetFileSizeOnDisk(file.ItemPtr, out size))
                {
                    throw new Exception("Unable to get file size.");
                }
                return size;
            }

            public bool Defragment()
            {
                Bind(PackageID);
                return DefragmentPackage();
            }

            public HLLibPackageStream CreateStream(Item item)
            {
                Bind(PackageID);
                return new HLLibPackageStream(item.ItemPtr);
            }

            public class HLLibPackageStream : Stream, IDisposable
            {
                internal HLLibPackageStream(IntPtr item)
                {
                    IntPtr stream;
                    if (!CreateStream(item, out stream))
                    {
                        throw new Exception("Unable to create stream.");
                    }
                    StreamPtr = stream;
                    Open((uint)FileMode.ReadVolatile);
                }

                public new void Dispose()
                {
                    base.Dispose();
                    ReleaseStream(StreamPtr);
                }
            }

            private static PackageType GetTypeFromPath(string path)
            {
                // Get the package type from the filename extension.
                var ret = GetTypeFromName(path);

                // If the above fails, try getting the package type from the data at the start of the file.
                if (ret == PackageType.None && SysIO.File.Exists(path))
                {
                    SysIO.FileStream reader = null;
                    try
                    {
                        var lpBuffer = new byte[DefaultPackageTestBufferSize];
                        reader = new SysIO.FileStream(path, SysIO.FileMode.Open, SysIO.FileAccess.Read, SysIO.FileShare.ReadWrite);
                        var iBytesRead = reader.Read(lpBuffer, 0, lpBuffer.Length);
                        if (iBytesRead > 0)
                        {
                            var lpBytesRead = Marshal.AllocHGlobal(iBytesRead);
                            try
                            {
                                Marshal.Copy(lpBuffer, 0, lpBytesRead, iBytesRead);
                                ret = GetTypeFromMemory(lpBytesRead, (uint)iBytesRead);
                            }
                            finally
                            {
                                Marshal.FreeHGlobal(lpBytesRead);
                            }
                        }
                    }
                    finally
                    {
                        if (reader != null)
                        {
                            reader.Close();
                        }
                    }
                }
                return ret;
            }

            private static PackageType GetTypeFromStream(Stream stream)
            {
                return GetTypeFromStream(new IntPtr(stream.Pointer));
            }

            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlBindPackage")]
            [return: MarshalAs(UnmanagedType.U1)]
            private static extern bool Bind(uint uiPackage);

            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlGetPackageTypeFromName")]
            private static extern PackageType GetTypeFromName(string lpName);

            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlGetPackageTypeFromMemory")]
            private static extern PackageType GetTypeFromMemory(IntPtr lpBuffer, uint uiBufferSize);

            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlGetPackageTypeFromStream")]
            private static extern PackageType GetTypeFromStream(IntPtr pStream);

            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlCreatePackage")]
            [return: MarshalAs(UnmanagedType.U1)]
            private static extern bool Create(PackageType ePackageType, out uint uiPackage);

            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlDeletePackage")]
            private static extern void Delete(uint uiPackage);

            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlPackageGetType")]
            private static extern PackageType GetPackageType();

            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlPackageGetExtension")]
            private static extern string GetExtension();

            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlPackageGetDescription")]
            private static extern string GetDescription();

            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlPackageGetOpened")]
            [return: MarshalAs(UnmanagedType.U1)]
            private static extern bool IsOpened();

            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlPackageOpenFile")]
            [return: MarshalAs(UnmanagedType.U1)]
            private static extern bool OpenFile(string lpFileName, uint uiMode);

            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlPackageOpenMemory")]
            [return: MarshalAs(UnmanagedType.U1)]
            private static extern bool OpenMemory(IntPtr lpData, uint uiBufferSize, uint uiMode);

            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlPackageOpenProc")]
            [return: MarshalAs(UnmanagedType.U1)]
            private static extern bool OpenProc(IntPtr pUserData, uint uiMode);

            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlPackageOpenStream")]
            [return: MarshalAs(UnmanagedType.U1)]
            private static extern bool OpenStream(IntPtr pStream, uint uiMode);

            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlPackageClose")]
            private static extern void ClosePackage();

            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlPackageDefragment")]
            [return: MarshalAs(UnmanagedType.U1)]
            private static extern bool DefragmentPackage();

            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlPackageGetRoot")]
            private static extern IntPtr GetRoot();

            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlPackageGetAttributeCount")]
            private static extern uint GetAttributeCount();

            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlPackageGetAttributeName")]
            private static extern string GetPackageAttributeName(PackageAttribute eAttribute);

            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlPackageGetAttribute")]
            [return: MarshalAs(UnmanagedType.U1)]
            private static extern bool GetAttribute(PackageAttribute eAttribute, out AttributeStruct pAttributeStruct);

            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlPackageGetItemAttributeCount")]
            private static extern uint GetItemAttributeCount();

            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlPackageGetItemAttributeName")]
            private static extern string GetPackageItemAttributeName(PackageAttribute eAttribute);

            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlPackageGetItemAttribute")]
            [return: MarshalAs(UnmanagedType.U1)]
            private static extern bool GetItemAttribute(IntPtr pItem, PackageAttribute eAttribute, out AttributeStruct pAttributeStruct);

            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlPackageGetExtractable")]
            [return: MarshalAs(UnmanagedType.U1)]
            private static extern bool GetExtractable(IntPtr pItem, [MarshalAs(UnmanagedType.U1)]out bool pExtractable);

            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlPackageGetFileSize")]
            [return: MarshalAs(UnmanagedType.U1)]
            private static extern bool GetFileSize(IntPtr pItem, out uint pSize);

            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlPackageGetFileSizeOnDisk")]
            [return: MarshalAs(UnmanagedType.U1)]
            private static extern bool GetFileSizeOnDisk(IntPtr pItem, out uint pSize);

            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlPackageCreateStream")]
            [return: MarshalAs(UnmanagedType.U1)]
            private static extern bool CreateStream(IntPtr pItem, out IntPtr pStream);

            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlPackageReleaseStream")]
            private static extern void ReleaseStream(IntPtr pStream);
        }

        public class NCFFile
        {
            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlNCFFileGetRootPath")]
            public static extern string GetRootPath();

            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlNCFFileSetRootPath")]
            public static extern void SetRootPath(string lpRootPath);
        }

        public class WADFile : File
        {
            private uint _width;
            private uint _height;

            public int Width
            {
                get { GetImageDimensions(); return (int) _width; }
            }

            public int Height
            {
                get { GetImageDimensions(); return (int) _height; }
            }

            public WADFile(Item item) : base (item.ItemPtr)
            {
                _width = _height = 0;
            }

            internal WADFile(IntPtr file) : base(file)
            {
                _width = _height = 0;
            }

            public uint GetLumpType()
            {
                AttributeStruct attr;
                if (!GetItemAttribute(ItemPtr, PackageAttribute.WADItemType, out attr))
                {
                    throw new Exception("Error reading lump type");
                }
                var a = new Attribute(attr);
                return a.GetUInt();
            }

            private void GetImageDimensions()
            {
                if (_width > 0 && _height > 0)
                {
                    return;
                }
                uint size;
                if (!GetImageSize(ItemPtr, out size))
                {
                    throw new Exception("Unable to get image dimensions.");
                }
                var rubbish = Marshal.AllocHGlobal((int) size);
                try
                {
                    if (!GetImageData(ItemPtr, out _width, out _height, out rubbish))
                    {
                        throw new Exception("Unable to get image dimensions.");
                    }
                }
                finally
                {
                    Marshal.FreeHGlobal(rubbish);
                }
            }

            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlWADFileGetImageSizePaletted")]
            [return:MarshalAs(UnmanagedType.U1)]
            public static extern bool GetImageSizePaletted(IntPtr pItem, out uint uiPaletteDataSize, out uint uiPixelDataSize);

            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlWADFileGetImageDataPaletted")]
            [return:MarshalAs(UnmanagedType.U1)]
            public static extern bool GetImageDataPaletted(IntPtr pItem, out uint uiWidth, out uint uiHeight, out IntPtr lpPaletteData, out IntPtr lpPixelData);

            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlWADFileGetImageSize")]
            [return:MarshalAs(UnmanagedType.U1)]
            public static extern bool GetImageSize(IntPtr pItem, out uint uiPixelDataSize);

            [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "hlWADFileGetImageData")]
            [return:MarshalAs(UnmanagedType.U1)]
            public static extern bool GetImageData(IntPtr pItem, out uint uiWidth, out uint uiHeight, out IntPtr lpPixelData);
        }
        #endregion
    }
}

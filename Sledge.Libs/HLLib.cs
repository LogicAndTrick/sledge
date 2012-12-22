/*
 * HLExtract.Net
 * Copyright (C) 2008-2010 Ryan Gregg

 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.

 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 */

using System;
using System.Runtime.InteropServices;

public sealed class HLLib
{
    #region Constants
    public const int HL_VERSION_NUMBER = ((2 << 24) | (3 << 16) | (0 << 8) | 0);
    public const string HL_VERSION_STRING = "2.3.0";

    public const uint HL_ID_INVALID = 0xffffffff;

    public const uint HL_DEFAULT_PACKAGE_TEST_BUFFER_SIZE = 8;
    public const uint HL_DEFAULT_VIEW_SIZE = 131072;
    public const uint HL_DEFAULT_COPY_BUFFER_SIZE = 131072;
    #endregion

    #region Enumerations
    public enum HLOption
    {
        HL_VERSION = 0,
        HL_ERROR,
        HL_ERROR_SYSTEM,
        HL_ERROR_SHORT_FORMATED,
        HL_ERROR_LONG_FORMATED,
        HL_PROC_OPEN,
        HL_PROC_CLOSE,
        HL_PROC_READ,
        HL_PROC_WRITE,
        HL_PROC_SEEK,
        HL_PROC_TELL,
        HL_PROC_SIZE,
        HL_PROC_EXTRACT_ITEM_START,
        HL_PROC_EXTRACT_ITEM_END,
        HL_PROC_EXTRACT_FILE_PROGRESS,
        HL_PROC_VALIDATE_FILE_PROGRESS,
        HL_OVERWRITE_FILES,
        HL_PACKAGE_BOUND,
        HL_PACKAGE_ID,
        HL_PACKAGE_SIZE,
        HL_PACKAGE_TOTAL_ALLOCATIONS,
        HL_PACKAGE_TOTAL_MEMORY_ALLOCATED,
        HL_PACKAGE_TOTAL_MEMORY_USED,
        HL_READ_ENCRYPTED,
        HL_FORCE_DEFRAGMENT,
        HL_PROC_DEFRAGMENT_PROGRESS,
        HL_PROC_DEFRAGMENT_PROGRESS_EX
    }

    public enum HLFileMode
	{
        HL_MODE_INVALID = 0x00,
        HL_MODE_READ = 0x01,
        HL_MODE_WRITE = 0x02,
        HL_MODE_CREATE = 0x04,
        HL_MODE_VOLATILE = 0x08,
        HL_MODE_NO_FILEMAPPING = 0x10,
        HL_MODE_QUICK_FILEMAPPING = 0x20
	}

    public enum HLSeekMode
    {
        HL_SEEK_BEGINNING = 0,
        HL_SEEK_CURRENT,
        HL_SEEK_END
    }

    public enum HLDirectoryItemType
    {
        HL_ITEM_NONE = 0,
        HL_ITEM_FOLDER,
        HL_ITEM_FILE
    }

    public enum HLSortOrder
    {
        HL_ORDER_ASCENDING = 0,
        HL_ORDER_DESCENDING
    }

    public enum HLSortField
    {
        HL_FIELD_NAME = 0,
        HL_FIELD_SIZE
    }

    public enum HLFindType
    {
        HL_FIND_FILES = 0x01,
        HL_FIND_FOLDERS = 0x02,
        HL_FIND_NO_RECURSE = 0x04,
        HL_FIND_CASE_SENSITIVE = 0x08,
        HL_FIND_MODE_STRING = 0x10,
        HL_FIND_MODE_SUBSTRING = 0x20,
        HL_FIND_MODE_WILDCARD = 0x00,
        HL_FIND_ALL = HL_FIND_FILES | HL_FIND_FOLDERS
    }

    public enum HLStreamType
    {
        HL_STREAM_NONE = 0,
        HL_STREAM_FILE,
        HL_STREAM_GCF,
        HL_STREAM_MAPPING,
        HL_STREAM_MEMORY,
        HL_STREAM_PROC,
        HL_STREAM_NULL
    }

    public enum HLMappingType
    {
        HL_MAPPING_NONE = 0,
        HL_MAPPING_FILE,
        HL_MAPPING_MEMORY,
        HL_MAPPING_STREAM
    }

    public enum HLPackageType
    {
	    HL_PACKAGE_NONE = 0,
	    HL_PACKAGE_BSP,
	    HL_PACKAGE_GCF,
	    HL_PACKAGE_PAK,
	    HL_PACKAGE_VBSP,
	    HL_PACKAGE_WAD,
	    HL_PACKAGE_XZP,
	    HL_PACKAGE_ZIP,
	    HL_PACKAGE_NCF,
	    HL_PACKAGE_VPK
    }

    public enum HLAttributeType
    {
        HL_ATTRIBUTE_INVALID = 0,
        HL_ATTRIBUTE_BOOLEAN,
        HL_ATTRIBUTE_INTEGER,
        HL_ATTRIBUTE_UNSIGNED_INTEGER,
        HL_ATTRIBUTE_FLOAT,
        HL_ATTRIBUTE_STRING
    }

    public enum HLPackageAttribute
    {
        HL_BSP_PACKAGE_VERSION = 0,
        HL_BSP_PACKAGE_COUNT,
        HL_BSP_ITEM_WIDTH = 0,
        HL_BSP_ITEM_HEIGHT,
        HL_BSP_ITEM_PALETTE_ENTRIES,
        HL_BSP_ITEM_COUNT,

        HL_GCF_PACKAGE_VERSION = 0,
        HL_GCF_PACKAGE_ID,
        HL_GCF_PACKAGE_ALLOCATED_BLOCKS,
        HL_GCF_PACKAGE_USED_BLOCKS,
        HL_GCF_PACKAGE_BLOCK_LENGTH,
        HL_GCF_PACKAGE_LAST_VERSION_PLAYED,
        HL_GCF_PACKAGE_COUNT,
        HL_GCF_ITEM_ENCRYPTED = 0,
        HL_GCF_ITEM_COPY_LOCAL,
        HL_GCF_ITEM_OVERWRITE_LOCAL,
        HL_GCF_ITEM_BACKUP_LOCAL,
        HL_GCF_ITEM_FLAGS,
        HL_GCF_ITEM_FRAGMENTATION,
        HL_GCF_ITEM_COUNT,

        HL_NCF_PACKAGE_VERSION = 0,
        HL_NCF_PACKAGE_ID,
        HL_NCF_PACKAGE_LAST_VERSION_PLAYED,
        HL_NCF_PACKAGE_COUNT,
        HL_NCF_ITEM_ENCRYPTED = 0,
        HL_NCF_ITEM_COPY_LOCAL,
        HL_NCF_ITEM_OVERWRITE_LOCAL,
        HL_NCF_ITEM_BACKUP_LOCAL,
        HL_NCF_ITEM_FLAGS,
        HL_NCF_ITEM_COUNT,

        HL_PAK_PACKAGE_COUNT = 0,
        HL_PAK_ITEM_COUNT = 0,

        HL_VBSP_PACKAGE_VERSION = 0,
        HL_VBSP_PACKAGE_MAP_REVISION,
        HL_VBSP_PACKAGE_COUNT,
        HL_VBSP_ITEM_VERSION = 0,
        HL_VBSP_ITEM_FOUR_CC,
        HL_VBSP_ZIP_PACKAGE_DISK,
        HL_VBSP_ZIP_PACKAGE_COMMENT,
        HL_VBSP_ZIP_ITEM_CREATE_VERSION,
        HL_VBSP_ZIP_ITEM_EXTRACT_VERSION,
        HL_VBSP_ZIP_ITEM_FLAGS,
        HL_VBSP_ZIP_ITEM_COMPRESSION_METHOD,
        HL_VBSP_ZIP_ITEM_CRC,
        HL_VBSP_ZIP_ITEM_DISK,
        HL_VBSP_ZIP_ITEM_COMMENT,
        HL_VBSP_ITEM_COUNT,

	    HL_VPK_PACKAGE_COUNT = 0,
	    HL_VPK_ITEM_PRELOAD_BYTES = 0,
	    HL_VPK_ITEM_ARCHIVE,
        HL_VPK_ITEM_CRC,
	    HL_VPK_ITEM_COUNT,

        HL_WAD_PACKAGE_VERSION = 0,
        HL_WAD_PACKAGE_COUNT,
        HL_WAD_ITEM_WIDTH = 0,
        HL_WAD_ITEM_HEIGHT,
        HL_WAD_ITEM_PALETTE_ENTRIES,
        HL_WAD_ITEM_MIPMAPS,
        HL_WAD_ITEM_COMPRESSED,
        HL_WAD_ITEM_TYPE,
        HL_WAD_ITEM_COUNT,

        HL_XZP_PACKAGE_VERSION = 0,
        HL_XZP_PACKAGE_PRELOAD_BYTES,
        HL_XZP_PACKAGE_COUNT,
        HL_XZP_ITEM_CREATED = 0,
        HL_XZP_ITEM_PRELOAD_BYTES,
        HL_XZP_ITEM_COUNT,

        HL_ZIP_PACKAGE_DISK = 0,
        HL_ZIP_PACKAGE_COMMENT,
        HL_ZIP_PACKAGE_COUNT,
        HL_ZIP_ITEM_CREATE_VERSION = 0,
        HL_ZIP_ITEM_EXTRACT_VERSION,
        HL_ZIP_ITEM_FLAGS,
        HL_ZIP_ITEM_COMPRESSION_METHOD,
        HL_ZIP_ITEM_CRC,
        HL_ZIP_ITEM_DISK,
        HL_ZIP_ITEM_COMMENT,
        HL_ZIP_ITEM_COUNT
    }

    public enum HLValidation
    {
        HL_VALIDATES_OK = 0,
        HL_VALIDATES_ASSUMED_OK,
        HL_VALIDATES_INCOMPLETE,
        HL_VALIDATES_CORRUPT,
        HL_VALIDATES_CANCELED,
        HL_VALIDATES_ERROR
    }
    #endregion

    #region Structures
    [StructLayout(LayoutKind.Explicit)]
    public struct HLAttribute
    {
        [FieldOffset(0)]
        public HLAttributeType eAttributeType;
        [FieldOffset(4)]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst=252)]
        public char[] lpName;
        [FieldOffset(256)]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst=256)]
        public byte[] lpValue;

        public string GetName()
        {
            int iLength = 0;
            while(lpName[iLength] != 0)
            {
                iLength++;
            }
            try
            {
                return new string(lpName, 0, iLength);
            }
            catch
            {
                return string.Empty;
            }
        }

        public object GetData()
        {
            switch (eAttributeType)
            {
                case HLAttributeType.HL_ATTRIBUTE_BOOLEAN:
                    return hlAttributeGetBoolean(ref this);
                case HLAttributeType.HL_ATTRIBUTE_INTEGER:
                    return hlAttributeGetInteger(ref this);
                case HLAttributeType.HL_ATTRIBUTE_UNSIGNED_INTEGER:
                    return hlAttributeGetUnsignedInteger(ref this);
                case HLAttributeType.HL_ATTRIBUTE_FLOAT:
                    return hlAttributeGetFloat(ref this);
                case HLAttributeType.HL_ATTRIBUTE_STRING:
                    return hlAttributeGetString(ref this);
                default:
                    return null;
            }
        }

        public override string ToString()
        {
            switch (eAttributeType)
            {
                case HLAttributeType.HL_ATTRIBUTE_BOOLEAN:
                    return hlAttributeGetBoolean(ref this).ToString();
                case HLAttributeType.HL_ATTRIBUTE_INTEGER:
                    return hlAttributeGetInteger(ref this).ToString("#,##0");
                case HLAttributeType.HL_ATTRIBUTE_UNSIGNED_INTEGER:
                    if (lpValue[4] == 0)
                    {
                        return hlAttributeGetUnsignedInteger(ref this).ToString("#,##0");
                    }
                    else // Display as hexadecimal.
                    {
                        return "0x" + hlAttributeGetUnsignedInteger(ref this).ToString("x8");
                    }
                case HLAttributeType.HL_ATTRIBUTE_FLOAT:
                    return hlAttributeGetFloat(ref this).ToString("#,##0.00000000");
                case HLAttributeType.HL_ATTRIBUTE_STRING:
                    return hlAttributeGetString(ref this);
                default:
                    return string.Empty;
            }
        }
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
    public delegate uint HLSeekProc(Int64 iOffset, HLSeekMode eSeekMode, [MarshalAs(UnmanagedType.I4)]int pUserData);
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
    // VTFLib
    //

    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern void hlInitialize();
    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern void hlShutdown();

    //
    // Get/Set
    //

    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    [return:MarshalAs(UnmanagedType.U1)]
    public static extern bool hlGetBoolean(HLOption eOption);
    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    [return:MarshalAs(UnmanagedType.U1)]
    public static extern bool hlGetBooleanValidate(HLOption eOption, [MarshalAs(UnmanagedType.U1)]out bool pValue);
    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern void hlSetBoolean(HLOption eOption, [MarshalAs(UnmanagedType.U1)]bool bValue);

    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern int hlGetInteger(HLOption eOption);
    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    [return:MarshalAs(UnmanagedType.U1)]
    public static extern bool hlGetIntegerValidate(HLOption eOption, out int pValue);
    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern void hlSetInteger(HLOption eOption, int iValue);

    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint hlGetUnsignedInteger(HLOption eOption);
    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    [return:MarshalAs(UnmanagedType.U1)]
    public static extern bool hlGetUnsignedIntegerValidate(HLOption eOption, out uint pValue);
    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern void hlSetUnsignedInteger(HLOption eOption, uint uiValue);

    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern float hlGetFloat(HLOption eOption);
    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    [return:MarshalAs(UnmanagedType.U1)]
    public static extern bool hlGetFloatValidate(HLOption eOption, out float pValue);
    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern void hlSetFloat(HLOption eOption, float pValue);

    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern string hlGetString(HLOption eOption);
    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    [return:MarshalAs(UnmanagedType.U1)]
    public static extern bool hlGetStringValidate(HLOption eOption, out string pValue);
    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern void hlSetString(HLOption eOption, string lpValue);

    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr hlGetVoid(HLOption eOption);
    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    [return:MarshalAs(UnmanagedType.U1)]
    public static extern bool hlGetVoidValidate(HLOption eOption, out IntPtr pValue);
    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern void hlSetVoid(HLOption eOption, IntPtr lpValue);

    //
    // Attributes
    //

    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    [return:MarshalAs(UnmanagedType.U1)]
    public static extern bool hlAttributeGetBoolean(ref HLAttribute pAttribute);
    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern void hlAttributeSetBoolean(ref HLAttribute pAttribute, string lpName, [MarshalAs(UnmanagedType.U1)]bool bValue);

    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern int hlAttributeGetInteger(ref HLAttribute pAttribute);
    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern void hlAttributeSetInteger(ref HLAttribute pAttribute, string lpName, int iValue);

    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint hlAttributeGetUnsignedInteger(ref HLAttribute pAttribute);
    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern void hlAttributeSetUnsignedInteger(ref HLAttribute pAttribute, string lpName, uint uiValue, [MarshalAs(UnmanagedType.U1)]bool bHexadecimal);

    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern float hlAttributeGetFloat(ref HLAttribute pAttribute);
    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern void hlAttributeSetFloat(ref HLAttribute pAttribute, string lpName, float fValue);

    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern string hlAttributeGetString(ref HLAttribute pAttribute);
    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern void hlAttributeSetString(ref HLAttribute pAttribute, string lpName, string lpValue);

    //
    // Directory Item
    //

    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern HLDirectoryItemType hlItemGetType(IntPtr pItem);

    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern string hlItemGetName(IntPtr pItem);
    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint hlItemGetID(IntPtr pItem);
    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr hlItemGetData(IntPtr pItem);

    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint hlItemGetPackage(IntPtr pItem);
    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr hlItemGetParent(IntPtr pItem);

    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    [return:MarshalAs(UnmanagedType.U1)]
    public static extern bool hlItemGetSize(IntPtr pItem, out uint pSize);
    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    [return:MarshalAs(UnmanagedType.U1)]
    public static extern bool hlItemGetSizeEx(IntPtr pItem, out UInt64 pSize);
    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    [return:MarshalAs(UnmanagedType.U1)]
    public static extern bool hlItemGetSizeOnDisk(IntPtr pItem, out uint pSize);
    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    [return:MarshalAs(UnmanagedType.U1)]
    public static extern bool hlItemGetSizeOnDiskEx(IntPtr pItem, out UInt64 pSize);

    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    [return:MarshalAs(UnmanagedType.U1)]
    public static extern bool hlItemGetPath(IntPtr pItem, IntPtr lpPath, uint uiPathSize);
    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    [return:MarshalAs(UnmanagedType.U1)]
    public static extern bool hlItemExtract(IntPtr pItem, string lpPath);

    //
    // Directory Folder
    //

    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint hlFolderGetCount(IntPtr pItem);

    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr hlFolderGetItem(IntPtr pItem, uint uiIndex);
    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr hlFolderGetItemByName(IntPtr pItem, string lpName, HLFindType eFind);
    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr hlFolderGetItemByPath(IntPtr pItem, string lpPath, HLFindType eFind);

    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern void hlFolderSort(IntPtr pItem, HLSortField eField, HLSortOrder eOrder, [MarshalAs(UnmanagedType.U1)]bool bRecurse);

    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr hlFolderFindFirst(IntPtr pFolder, string lpSearch, HLFindType eFind);
    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr hlFolderFindNext(IntPtr pFolder, IntPtr pItem, string lpSearch, HLFindType eFind);

    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint hlFolderGetSize(IntPtr pItem, [MarshalAs(UnmanagedType.U1)]bool bRecurse);
    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern UInt64 hlFolderGetSizeEx(IntPtr pItem, [MarshalAs(UnmanagedType.U1)]bool bRecurse);
    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint hlFolderGetSizeOnDisk(IntPtr pItem, [MarshalAs(UnmanagedType.U1)]bool bRecurse);
    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern UInt64 hlFolderGetSizeOnDiskEx(IntPtr pItem, [MarshalAs(UnmanagedType.U1)]bool bRecurse);
    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint hlFolderGetFolderCount(IntPtr pItem, [MarshalAs(UnmanagedType.U1)]bool bRecurse);
    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint hlFolderGetFileCount(IntPtr pItem, [MarshalAs(UnmanagedType.U1)]bool bRecurse);

    //
    // Directory File
    //

    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint hlFileGetExtractable(IntPtr pItem);
    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern HLValidation hlFileGetValidation(IntPtr pItem);
    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint hlFileGetSize(IntPtr pItem);
    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint hlFileGetSizeOnDisk(IntPtr pItem);

    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    [return:MarshalAs(UnmanagedType.U1)]
    public static extern bool hlFileCreateStream(IntPtr pItem, out IntPtr pStream);
    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern void hlFileReleaseStream(IntPtr pItem, IntPtr pStream);

    //
    // Stream
    //

    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern HLStreamType hlStreamGetType(IntPtr pStream);

    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    [return:MarshalAs(UnmanagedType.U1)]
    public static extern bool hlStreamGetOpened(IntPtr pStream);
    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint hlStreamGetMode(IntPtr pStream);

    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    [return:MarshalAs(UnmanagedType.U1)]
    public static extern bool hlStreamOpen(IntPtr pStream, uint uiMode);
    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern void hlStreamClose(IntPtr pStream);

    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint hlStreamGetStreamSize(IntPtr pStream);
    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint hlStreamGetStreamPointer(IntPtr pStream);

    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint hlStreamSeek(IntPtr pStream, Int64 iOffset, HLSeekMode eSeekMode);

    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    [return:MarshalAs(UnmanagedType.U1)]
    public static extern bool hlStreamReadChar(IntPtr pStream, out char pChar);
    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint hlStreamRead(IntPtr pStream, IntPtr lpData, uint uiBytes);

    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    [return:MarshalAs(UnmanagedType.U1)]
    public static extern bool hlStreamWriteChar(IntPtr pStream, char iChar);
    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint hlStreamWrite(IntPtr pStream, IntPtr lpData, uint uiBytes);

    //
    // Package
    //

    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    [return:MarshalAs(UnmanagedType.U1)]
    public static extern bool hlBindPackage(uint uiPackage);

    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern HLPackageType hlGetPackageTypeFromName(string lpName);
    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern HLPackageType hlGetPackageTypeFromMemory(IntPtr lpBuffer, uint uiBufferSize);
    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern HLPackageType hlGetPackageTypeFromStream(IntPtr pStream);

    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    [return:MarshalAs(UnmanagedType.U1)]
    public static extern bool hlCreatePackage(HLPackageType ePackageType, out uint uiPackage);
    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern void hlDeletePackage(uint uiPackage);

    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern HLPackageType hlPackageGetType();
    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern string hlPackageGetExtension();
    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern string hlPackageGetDescription();

    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    [return:MarshalAs(UnmanagedType.U1)]
    public static extern bool hlPackageGetOpened();

    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    [return:MarshalAs(UnmanagedType.U1)]
    public static extern bool hlPackageOpenFile(string lpFileName, uint uiMode);
    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    [return:MarshalAs(UnmanagedType.U1)]
    public static extern bool hlPackageOpenMemory(IntPtr lpData, uint uiBufferSize, uint uiMode);
    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    [return:MarshalAs(UnmanagedType.U1)]
    public static extern bool hlPackageOpenProc(IntPtr pUserData, uint uiMode);
    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    [return:MarshalAs(UnmanagedType.U1)]
    public static extern bool hlPackageOpenStream(IntPtr pStream, uint uiMode);
    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern void hlPackageClose();

    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    [return:MarshalAs(UnmanagedType.U1)]
    public static extern bool hlPackageDefragment();

    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr hlPackageGetRoot();

    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint hlPackageGetAttributeCount();
    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern string hlPackageGetAttributeName(HLPackageAttribute eAttribute);
    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    [return:MarshalAs(UnmanagedType.U1)]
    public static extern bool hlPackageGetAttribute(HLPackageAttribute eAttribute, out HLAttribute pAttribute);

    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern uint hlPackageGetItemAttributeCount();
    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern string hlPackageGetItemAttributeName(HLPackageAttribute eAttribute);
    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    [return:MarshalAs(UnmanagedType.U1)]
    public static extern bool hlPackageGetItemAttribute(IntPtr pItem, HLPackageAttribute eAttribute, out HLAttribute pAttribute);

    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    [return:MarshalAs(UnmanagedType.U1)]
    public static extern bool hlPackageGetExtractable(IntPtr pItem, [MarshalAs(UnmanagedType.U1)]out bool pExtractable);
    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    [return:MarshalAs(UnmanagedType.U1)]
    public static extern bool hlPackageGetFileSize(IntPtr pItem, out uint pSize);
    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    [return:MarshalAs(UnmanagedType.U1)]
    public static extern bool hlPackageGetFileSizeOnDisk(IntPtr pItem, out uint pSize);
    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    [return:MarshalAs(UnmanagedType.U1)]
    public static extern bool hlPackageCreateStream(IntPtr pItem, out IntPtr pStream);
    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern void hlPackageReleaseStream(IntPtr pStream);

    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern string hlNCFFileGetRootPath();
    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern void hlNCFFileSetRootPath(string lpRootPath);

    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    [return:MarshalAs(UnmanagedType.U1)]
    public static extern bool hlWADFileGetImageSizePaletted(IntPtr pItem, out uint uiPaletteDataSize, out uint uiPixelDataSize);
    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    [return:MarshalAs(UnmanagedType.U1)]
    public static extern bool hlWADFileGetImageDataPaletted(IntPtr pItemm, out uint uiWidth, out uint uiHeight, out IntPtr lpPaletteData, out IntPtr lpPixelData);
    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    [return:MarshalAs(UnmanagedType.U1)]
    public static extern bool hlWADFileGetImageSize(IntPtr pItem, out uint uiPixelDataSize);
    [DllImport("HLLib.dll", CallingConvention = CallingConvention.Cdecl)]
    [return:MarshalAs(UnmanagedType.U1)]
    public static extern bool hlWADFileGetImageData(IntPtr pItem, out uint uiWidth, out uint uiHeight, out IntPtr lpPixelData);
    #endregion
}

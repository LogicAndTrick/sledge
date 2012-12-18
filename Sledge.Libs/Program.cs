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
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace HLExtract.Net
{
    class Program
    {
        static bool bSilent = false;
        static bool bPause = true;
        static string sDestination = string.Empty;

        static int Main(string[] args)
        {
	        // Arguments.
	        string sPackage = string.Empty;
            List<string> Commands = new List<string>();
            string sNCFRootPath = string.Empty;

	        bool bFileMapping = false;
	        bool bQuickFileMapping = false;
	        bool bVolatileAccess = false;
            bool bWriteAccess = false;
	        bool bOverwriteFiles = true;

	        // Package stuff.
	        HLLib.HLPackageType ePackageType = HLLib.HLPackageType.HL_PACKAGE_NONE;
	        uint uiPackage = HLLib.HL_ID_INVALID;
            uint uiMode = (uint)HLLib.HLFileMode.HL_MODE_INVALID;

	        if(HLLib.hlGetUnsignedInteger(HLLib.HLOption.HL_VERSION) < HLLib.HL_VERSION_NUMBER)
	        {
		        Console.WriteLine("Wrong HLLib version: v{0}.", HLLib.hlGetString(HLLib.HLOption.HL_VERSION));
		        return 1;
	        }

	        // Process switches.
	        if(args.Length == 1)
	        {
		        sPackage = args[0];
	        }
	        else
	        {
		        for(int i = 0; i < args.Length; i++)
		        {
                    if(string.Equals(args[i], "-p", StringComparison.CurrentCultureIgnoreCase) || string.Equals(args[i], "--package", StringComparison.CurrentCultureIgnoreCase))
			        {
				        if(sPackage.Length == 0 && i + 1 < args.Length)
				        {
                            sPackage = args[++i];
				        }
				        else
				        {
					        PrintUsage();
					        return 2;
				        }
			        }
                    else if(string.Equals(args[i], "-d", StringComparison.CurrentCultureIgnoreCase) || string.Equals(args[i], "--dest", StringComparison.CurrentCultureIgnoreCase))
			        {
				        if(sDestination.Length == 0 && i + 1 < args.Length)
				        {
                            sDestination = args[++i];
				        }
				        else
				        {
					        PrintUsage();
					        return 2;
				        }
			        }
                    else if(string.Equals(args[i], "-x", StringComparison.CurrentCultureIgnoreCase) || string.Equals(args[i], "--execute", StringComparison.CurrentCultureIgnoreCase))
			        {
				        if(sDestination.Length == 0 && i + 1 < args.Length)
				        {
                            Commands.Add(args[++i]);
				        }
				        else
				        {
					        PrintUsage();
					        return 2;
				        }
			        }
                    else if(string.Equals(args[i], "-n", StringComparison.CurrentCultureIgnoreCase) || string.Equals(args[i], "--ncfroot", StringComparison.CurrentCultureIgnoreCase))
			        {
				        if(sNCFRootPath.Length == 0 && i + 1 < args.Length)
				        {
                            sNCFRootPath = args[++i];
				        }
				        else
				        {
					        PrintUsage();
					        return 2;
				        }
			        }
                    else if(string.Equals(args[i], "-s", StringComparison.CurrentCultureIgnoreCase) || string.Equals(args[i], "--silent", StringComparison.CurrentCultureIgnoreCase))
			        {
				        bSilent = true;
			        }
                    else if(string.Equals(args[i], "-u", StringComparison.CurrentCultureIgnoreCase) || string.Equals(args[i], "--no-pause", StringComparison.CurrentCultureIgnoreCase))
			        {
				        bPause = false;
			        }
                    else if(string.Equals(args[i], "-m", StringComparison.CurrentCultureIgnoreCase) || string.Equals(args[i], "--filemapping", StringComparison.CurrentCultureIgnoreCase))
			        {
				        bFileMapping = true;
			        }
                    else if(string.Equals(args[i], "-q", StringComparison.CurrentCultureIgnoreCase) || string.Equals(args[i], "--quick-filemapping", StringComparison.CurrentCultureIgnoreCase))
			        {
				        bFileMapping = true;
				        bQuickFileMapping = true;
			        }
                    else if(string.Equals(args[i], "-v", StringComparison.CurrentCultureIgnoreCase) || string.Equals(args[i], "--volatile", StringComparison.CurrentCultureIgnoreCase))
			        {
				        bVolatileAccess = true;
			        }
                    else if(string.Equals(args[i], "-w", StringComparison.CurrentCultureIgnoreCase) || string.Equals(args[i], "--write", StringComparison.CurrentCultureIgnoreCase))
			        {
				        bWriteAccess = true;
			        }
                    else if(string.Equals(args[i], "-o", StringComparison.CurrentCultureIgnoreCase) || string.Equals(args[i], "--no-overwrite", StringComparison.CurrentCultureIgnoreCase))
			        {
				        bOverwriteFiles = false;
			        }
			        else
			        {
				        PrintUsage();
				        return 2;
			        }
		        }
	        }

	        // Make sure we have something to do.
	        if(sPackage.Length == 0)
	        {
		        PrintUsage();
		        return 2;
	        }

	        // If the destination directory is not specified, make it the input directory.
	        if(sDestination.Length == 0)
	        {
                int iIndex = sPackage.LastIndexOfAny(new char[]{'\\', '/'});
		        if(iIndex != -1)
		        {
			        sDestination = sPackage.Substring(0, iIndex + 1);
		        }
	        }

	        HLLib.hlInitialize();

            // Keep the delegates alive so they don't get garbage collected.
            HLLib.HLExtractItemStartProc HLExtractItemStartProc = new HLLib.HLExtractItemStartProc(ExtractItemStartCallback);
            HLLib.HLExtractItemEndProc HLExtractItemEndProc = new HLLib.HLExtractItemEndProc(ExtractItemEndCallback);
            HLLib.HLExtractFileProgressProc HLExtractFileProgressProc = new HLLib.HLExtractFileProgressProc(FileProgressCallback);
            HLLib.HLValidateFileProgressProc HLValidateFileProgressProc = new HLLib.HLValidateFileProgressProc(FileProgressCallback);
            HLLib.HLDefragmentFileProgressExProc HLDefragmentFileProgressProc = new HLLib.HLDefragmentFileProgressExProc(DefragmentProgressCallback);

	        HLLib.hlSetBoolean(HLLib.HLOption.HL_OVERWRITE_FILES, bOverwriteFiles);
	        HLLib.hlSetVoid(HLLib.HLOption.HL_PROC_EXTRACT_ITEM_START, Marshal.GetFunctionPointerForDelegate(HLExtractItemStartProc));
	        HLLib.hlSetVoid(HLLib.HLOption.HL_PROC_EXTRACT_ITEM_END, Marshal.GetFunctionPointerForDelegate(HLExtractItemEndProc));
            HLLib.hlSetVoid(HLLib.HLOption.HL_PROC_EXTRACT_FILE_PROGRESS, Marshal.GetFunctionPointerForDelegate(HLExtractFileProgressProc));
	        HLLib.hlSetVoid(HLLib.HLOption.HL_PROC_VALIDATE_FILE_PROGRESS, Marshal.GetFunctionPointerForDelegate(HLValidateFileProgressProc));
	        HLLib.hlSetVoid(HLLib.HLOption.HL_PROC_DEFRAGMENT_PROGRESS_EX, Marshal.GetFunctionPointerForDelegate(HLDefragmentFileProgressProc));

	        // Get the package type from the filename extension.
	        ePackageType = HLLib.hlGetPackageTypeFromName(sPackage);

	        // If the above fails, try getting the package type from the data at the start of the file.
	        if(ePackageType == HLLib.HLPackageType.HL_PACKAGE_NONE && File.Exists(sPackage))
	        {
                FileStream Reader = null;
                try
                {
                    byte[] lpBuffer = new byte[HLLib.HL_DEFAULT_PACKAGE_TEST_BUFFER_SIZE];
                    Reader = new FileStream(sPackage, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    int iBytesRead = Reader.Read(lpBuffer, 0, lpBuffer.Length);
                    if(iBytesRead > 0)
                    {
                        IntPtr lpBytesRead = Marshal.AllocHGlobal(iBytesRead);
                        try
                        {
                            Marshal.Copy(lpBuffer, 0, lpBytesRead, iBytesRead);
                            ePackageType = HLLib.hlGetPackageTypeFromMemory(lpBytesRead, (uint)iBytesRead);
                        }
                        finally
                        {
                            Marshal.FreeHGlobal(lpBytesRead);
                        }
                    }
                }
                finally
                {
                    if(Reader != null)
                    {
                        Reader.Close();
                    }
                }
	        }

	        if(ePackageType == HLLib.HLPackageType.HL_PACKAGE_NONE)
	        {
                Console.WriteLine("Error loading {0}:", sPackage);
                Console.WriteLine("Unsupported package type.");

		        HLLib.hlShutdown();
                Pause();
		        return 3;
	        }

	        // Create a package element, the element is allocated by the library and cleaned
	        // up by the library.  An ID is generated which must be bound to apply operations
	        // to the package.
	        if(!HLLib.hlCreatePackage(ePackageType, out uiPackage))
	        {
                Console.WriteLine("Error loading {0}:", sPackage);
                Console.WriteLine(HLLib.hlGetString(HLLib.HLOption.HL_ERROR_SHORT_FORMATED));

                HLLib. hlShutdown();
                Pause();
		        return 3;
	        }

	        HLLib.hlBindPackage(uiPackage);

	        uiMode = (uint)HLLib.HLFileMode.HL_MODE_READ;
            uiMode |= !bFileMapping ? (uint)HLLib.HLFileMode.HL_MODE_NO_FILEMAPPING : 0;
            uiMode |= bQuickFileMapping ? (uint)HLLib.HLFileMode.HL_MODE_QUICK_FILEMAPPING : 0;
            uiMode |= bVolatileAccess ? (uint)HLLib.HLFileMode.HL_MODE_VOLATILE : 0;
            uiMode |= bWriteAccess ? (uint)HLLib.HLFileMode.HL_MODE_WRITE : 0;

	        // Open the package.
	        // Of the above modes, only HL_MODE_READ is required.  HL_MODE_WRITE is present
	        // only for future use.  File mapping is recommended as an efficient way to load
	        // packages.  Quick file mapping maps the entire file (instead of bits as they are
	        // needed) and thus should only be used in Windows 2000 and up (older versions of
	        // Windows have poor virtual memory management which means large files won't be able
	        // to find a continues block and will fail to load).  Volatile access allows HLLib
	        // to share files with other applications that have those file open for writing.
	        // This is useful for, say, loading .gcf files while Steam is running.
	        if(!HLLib.hlPackageOpenFile(sPackage, uiMode))
	        {
                Console.WriteLine("Error loading {0}:", sPackage);
                Console.WriteLine(HLLib.hlGetString(HLLib.HLOption.HL_ERROR_SHORT_FORMATED));

                HLLib. hlShutdown();
                Pause();
		        return 3;
	        }

	        // If we have a .ncf file, the package file data is stored externally.  In order to
	        // validate the file data etc., HLLib needs to know where to look.  Tell it where.
	        if(ePackageType == HLLib.HLPackageType.HL_PACKAGE_NCF && sNCFRootPath.Length > 0)
	        {
		        HLLib.hlNCFFileSetRootPath(sNCFRootPath);
	        }

	        if(!bSilent)
		        Console.WriteLine("{0} opened.", sPackage);

	        // Interactive console mode.
	        EnterConsole(uiPackage, Commands);

	        // Close the package.
	        HLLib.hlPackageClose();

	        if(!bSilent)
		        Console.WriteLine("{0} closed.", sPackage);

	        // Free up the allocated memory.
	        HLLib.hlDeletePackage(uiPackage);

	        HLLib.hlShutdown();

	        return 0;
        }

        static void Pause()
        {
            if(bPause)
            {
                Console.Write("Press any key to continue . . . ");
                Console.ReadKey(true);
            }
        }

        static void PrintUsage()
        {
            System.Reflection.AssemblyName Name = System.Reflection.Assembly.GetExecutingAssembly().GetName();

	        Console.WriteLine("HLExtract.Net v{0}.{1}.{2} using HLLib v{3}", Name.Version.Major, Name.Version.Minor, Name.Version.Build, HLLib.hlGetString(HLLib.HLOption.HL_VERSION));
	        Console.WriteLine();
	        Console.WriteLine("Correct HLExtract.Net usage:");
	        Console.WriteLine(" -p <filepath>       (Package to load.)");
	        Console.WriteLine(" -d <path>           (Destination extraction directory.)");
            Console.WriteLine(" -x <command>        (Execute console command.)");
	        Console.WriteLine(" -s                  (Silent mode.)");
            Console.WriteLine(" -u                  (Don't pause on error..)");
	        Console.WriteLine(" -m                  (Use file mapping.)");
	        Console.WriteLine(" -q                  (Use quick file mapping.)");
	        Console.WriteLine(" -v                  (Allow volatile access.)");
            Console.WriteLine(" -w                  (Allow write access.)");
	        Console.WriteLine(" -o                  (Don't overwrite files.)");
	        Console.WriteLine(" -n <path>           (NCF file's root path.)");
	        Console.WriteLine();
	        Console.WriteLine("Example HLExtract.Net usage:");
	        Console.WriteLine("HLExtract.Net.exe -p \"C:\\half-life.gcf\" -d \"C:\\backup\"");
	        Console.WriteLine("HLExtract.Net.exe -p \"C:\\half-life.gcf\" -m -v");
            Console.WriteLine("HLExtract.Net.exe -p \"C:\\half-life.gcf\" -w -x defragment -x exit");
	        Console.WriteLine();
	        Console.WriteLine("Batching HLExtract.Net:");
            Console.WriteLine("for %%F in (*.gcf) do HLExtract.Net.exe -p \"%%F\" -u -v -x \"info .\" -x exit");
            Console.WriteLine("for %%F in (*.gcf) do HLExtract.Net.exe -p \"%%F\" -s -u -x \"validate .\" -x exit");
            Console.WriteLine("for %%F in (*.gcf) do HLExtract.Net.exe -p \"%%F\" -s -u -w -x defragment -x exit");

            Pause();
        }

        static readonly uint MAX_PATH_SIZE = 512;

        static string GetPath(IntPtr pItem)
        {
            string sPath = string.Empty;
            IntPtr lpPath = IntPtr.Zero;
            try
            {
                lpPath = Marshal.AllocHGlobal((int)MAX_PATH_SIZE);
                HLLib.hlItemGetPath(pItem, lpPath, MAX_PATH_SIZE);
                sPath = Marshal.PtrToStringAnsi(lpPath);
            }
            finally
            {
                if(lpPath != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(lpPath);
                }
            }
            return sPath;
        }

#region Progress Callbacks
        static uint uiProgressLast;

        static void ProgressStart()
        {
	        uiProgressLast = 0;
	        Console.Write("0%");
        }

        static void ProgressUpdate(UInt64 uiBytesDone, UInt64 uiBytesTotal)
        {
	        if(!bSilent)
	        {
		        uint uiProgress = uiBytesTotal == 0 ? 100 : (uint)(uiBytesDone * 100 / uiBytesTotal);
		        while(uiProgress >= uiProgressLast + 10)
		        {
			        uiProgressLast += 10;
			        if(uiProgressLast == 100)
			        {
				        Console.Write("100% ");
			        }
			        else if(uiProgressLast == 50)
			        {
				        Console.Write("50%");
			        }
			        else
			        {
				        Console.Write(".");
			        }
		        }
	        }
        }

        static void ExtractItemStartCallback(int iItem)
        {
            IntPtr pItem = new IntPtr(iItem);
	        if(!bSilent)
	        {
		        if(HLLib.hlItemGetType(pItem) == HLLib.HLDirectoryItemType.HL_ITEM_FILE)
		        {
			        Console.Write("  Extracting {0}: ", HLLib.hlItemGetName(pItem));
			        ProgressStart();
		        }
		        else
		        {
			        Console.WriteLine("  Extracting {0}:", HLLib.hlItemGetName(pItem));
		        }
	        }
        }

        static void FileProgressCallback(int iFile, uint uiBytesExtracted, uint uiBytesTotal, ref bool pCancel)
        {
	        ProgressUpdate((UInt64)uiBytesExtracted, (UInt64)uiBytesTotal);
        }

        static void ExtractItemEndCallback(int iItem, bool bSuccess)
        {
            IntPtr pItem = new IntPtr(iItem);
	        if(bSuccess)
	        {
		        if(!bSilent)
		        {
                    uint uiSize = 0;
			        HLLib.hlItemGetSize(pItem, out uiSize);
			        if(HLLib.hlItemGetType(pItem) == HLLib.HLDirectoryItemType.HL_ITEM_FILE)
			        {
				        Console.WriteLine("OK ({0} B)", uiSize);
			        }
			        else
			        {
				        Console.WriteLine("  Done {0}: OK ({1} B)", HLLib.hlItemGetName(pItem), uiSize);
			        }
		        }
	        }
	        else
	        {
		        if(!bSilent)
		        {
			        if(HLLib.hlItemGetType(pItem) == HLLib.HLDirectoryItemType.HL_ITEM_FILE)
			        {
				        Console.WriteLine("Errored");
				        Console.WriteLine("    {0}", HLLib.hlGetString(HLLib.HLOption.HL_ERROR_SHORT_FORMATED));
			        }
			        else
			        {
				        Console.WriteLine("  Done {0}: Errored", HLLib.hlItemGetName(pItem));
			        }
		        }
		        else
		        {
			        if(HLLib.hlItemGetType(pItem) == HLLib.HLDirectoryItemType.HL_ITEM_FILE)
			        {
				        Console.WriteLine("  Error extracting {0}:", GetPath(pItem));
				        Console.WriteLine("    {0}", HLLib.hlGetString(HLLib.HLOption.HL_ERROR_SHORT_FORMATED));
			        }
			        else
			        {
				        Console.WriteLine("  Error extracting {0}.", GetPath(pItem));
			        }
		        }
	        }
        }

        static void DefragmentProgressCallback(int iFile, uint uiFilesDefragmented, uint uiFilesTotal, UInt64 uiBytesDefragmented, UInt64 uiBytesTotal, ref bool pCancel)
        {
	        ProgressUpdate(uiBytesDefragmented, uiBytesTotal);
        }
#endregion

        static readonly string[] ValidationNames = new string[] { "OK", "Assumed OK", "Incomplete", "Corrupt", "Canceled", "Error" };

        static string GetValidation(HLLib.HLValidation eValidation)
        {
            if(eValidation >= HLLib.HLValidation.HL_VALIDATES_OK && eValidation <= HLLib.HLValidation.HL_VALIDATES_ERROR)
            {
                return ValidationNames[(uint)eValidation];
            }
            return string.Empty;
        }

        static HLLib.HLValidation Validate(IntPtr pItem)
        {
	        HLLib.HLValidation eValidation = HLLib.HLValidation.HL_VALIDATES_OK, eTest;

	        switch(HLLib.hlItemGetType(pItem))
	        {
	        case HLLib.HLDirectoryItemType.HL_ITEM_FOLDER:
		        if(!bSilent)
		        {
			        Console.WriteLine("  Validating {0}:", HLLib.hlItemGetName(pItem));
		        }

		        uint uiItemCount = HLLib.hlFolderGetCount(pItem);
		        for(uint i = 0; i < uiItemCount; i++)
		        {
			        eTest = Validate(HLLib.hlFolderGetItem(pItem, i));
			        if(eTest > eValidation)
			        {
				        eValidation = eTest;
			        }
		        }

		        if(!bSilent)
		        {
			        Console.WriteLine("  Done {0}: {1}", HLLib.hlItemGetName(pItem), GetValidation(eValidation));
		        }
		        break;
	        case HLLib.HLDirectoryItemType.HL_ITEM_FILE:
		        if(!bSilent)
		        {
			        Console.Write("  Validating {0}: ", HLLib.hlItemGetName(pItem));
			        ProgressStart();
		        }

		        eValidation = HLLib.hlFileGetValidation(pItem);

		        if(bSilent)
		        {
			        switch(eValidation)
			        {
			        case HLLib.HLValidation.HL_VALIDATES_INCOMPLETE:
			        case HLLib.HLValidation.HL_VALIDATES_CORRUPT:
				        Console.WriteLine("  Validating {0}: {1}", GetPath(pItem), GetValidation(eValidation));
				        break;
			        }
		        }
		        else
		        {
                    Console.WriteLine(GetValidation(eValidation));
		        }
		        break;
	        }

	        return eValidation;
        }

        static void EnterConsole(uint uiPackage, List<string> Commands)
        {
	        IntPtr pItem = HLLib.hlPackageGetRoot();

	        while(true)
	        {
                string sLine;
                if(Commands.Count > 0)
                {
                    sLine = Commands[0].Trim();

                    Console.WriteLine("{0}>{1}", HLLib.hlItemGetName(pItem), sLine);
                    Commands.RemoveAt(0);
                }
                else
                {
                    // Command prompt.
                    Console.Write("{0}>", HLLib.hlItemGetName(pItem));

                    // Get and parse line.
                    sLine = Console.ReadLine().Trim();
                }
                if(sLine == null)
                {
                    break;
                }

                int iIndex;
                for(iIndex = 0; iIndex < sLine.Length; iIndex++)
                {
                    if(!Char.IsLetter(sLine[iIndex]))
                    {
                        break;
                    }
                }

                string sCommand, sArgument;
                if(iIndex == sLine.Length)
                {
                    sCommand = sLine;
                    sArgument = string.Empty;
                }
                else
                {
                    sCommand = sLine.Substring(0, iIndex).TrimEnd();
                    sArgument = sLine.Substring(iIndex).TrimStart();
                }

		        // Cycle through commands.

		        //
		        // Directory listing.
		        // Good example of CDirectoryItem::GetType().
		        //
		        if(String.Equals(sCommand, "dir", StringComparison.CurrentCultureIgnoreCase))
		        {
			        uint uiItemCount = HLLib.hlFolderGetCount(pItem);
			        uint uiFolderCount = 0, uiFileCount = 0;

			        Console.WriteLine("Directory of {0}:", GetPath(pItem));

			        Console.WriteLine();

			        if(sArgument.Length == 0)
			        {
				        // List all items in the current folder.
				        for(uint i = 0; i < uiItemCount; i++)
				        {
					        IntPtr pSubItem = HLLib.hlFolderGetItem(pItem, i);
					        if(HLLib.hlItemGetType(pSubItem) == HLLib.HLDirectoryItemType.HL_ITEM_FOLDER)
					        {
						        uiFolderCount++;
						        Console.WriteLine("  <{0}>", HLLib.hlItemGetName(pSubItem));
					        }
					        else if(HLLib.hlItemGetType(pSubItem) == HLLib.HLDirectoryItemType.HL_ITEM_FILE)
					        {
						        uiFileCount++;
						        Console.WriteLine("  {0}", HLLib.hlItemGetName(pSubItem));
					        }
				        }
			        }
			        else
			        {
				        IntPtr pSubItem = HLLib.hlFolderFindFirst(pItem, sArgument, HLLib.HLFindType.HL_FIND_ALL | HLLib.HLFindType.HL_FIND_NO_RECURSE);
				        while(pSubItem != IntPtr.Zero)
				        {
					        if(HLLib.hlItemGetType(pSubItem) == HLLib.HLDirectoryItemType.HL_ITEM_FOLDER)
					        {
						        uiFolderCount++;
						        Console.WriteLine("  <{0}>", HLLib.hlItemGetName(pSubItem));
					        }
					        else if(HLLib.hlItemGetType(pSubItem) == HLLib.HLDirectoryItemType.HL_ITEM_FILE)
					        {
						        uiFileCount++;
						        Console.WriteLine("  {0}", HLLib.hlItemGetName(pSubItem));
					        }

					        pSubItem = HLLib.hlFolderFindNext(pItem, pSubItem, sArgument, HLLib.HLFindType.HL_FIND_ALL | HLLib.HLFindType.HL_FIND_NO_RECURSE);
				        }
			        }

			        Console.WriteLine();

			        // Could also have used hlFolderGetFolderCount() and
			        // hlFolderGetFileCount().

			        Console.WriteLine("Summary:");
			        Console.WriteLine();
			        Console.WriteLine("  {0} Folder{1}.", uiFolderCount, uiFolderCount != 1 ? "s" : "");
			        Console.WriteLine("  {0} File{1}.", uiFileCount, uiFileCount != 1 ? "s" : "");
			        Console.WriteLine();
		        }
		        //
		        // Change directory.
		        // Good example of CDirectoryFolder::GetParent() and item casting.
		        //
		        else if(String.Equals(sCommand, "cd", StringComparison.CurrentCultureIgnoreCase))
		        {
			        if(sArgument.Length == 0)
			        {
				        Console.WriteLine("No argument for command cd supplied.");
			        }
			        else
			        {
				        if(String.Equals(sArgument, ".", StringComparison.CurrentCultureIgnoreCase))
				        {

				        }
				        else if(String.Equals(sArgument, "..", StringComparison.CurrentCultureIgnoreCase))
				        {
					        if(HLLib.hlItemGetParent(pItem) != IntPtr.Zero)
					        {
						        pItem = HLLib.hlItemGetParent(pItem);
					        }
					        else
					        {
						        Console.WriteLine("Folder does not have a parent.");
					        }
				        }
				        else
				        {
					        bool bFound = false;
					        uint uiItemCount = HLLib.hlFolderGetCount(pItem);
					        for(uint i = 0; i < uiItemCount; i++)
					        {
						        IntPtr pSubItem = HLLib.hlFolderGetItem(pItem, i);
						        if(HLLib.hlItemGetType(pSubItem) == HLLib.HLDirectoryItemType.HL_ITEM_FOLDER && String.Equals(sArgument, HLLib.hlItemGetName(pSubItem), StringComparison.CurrentCultureIgnoreCase))
						        {
							        bFound = true;
							        pItem = pSubItem;
							        break;
						        }
					        }

					        if(!bFound)
					        {
						        Console.WriteLine("{0} not found.", sArgument);
					        }
				        }
			        }
		        }
		        //
		        // Go to the root folder.
		        //
		        else if(String.Equals(sCommand, "root", StringComparison.CurrentCultureIgnoreCase))
		        {
			        pItem = HLLib.hlPackageGetRoot();
		        }
		        //
		        // Item information.
		        // Good example of CPackageUtility helper functions.
		        //
		        else if(String.Equals(sCommand, "info", StringComparison.CurrentCultureIgnoreCase))
		        {
			        if(sArgument.Length == 0)
			        {
				        Console.WriteLine("No argument for command info supplied.");
			        }
			        else
			        {
				        IntPtr pSubItem = HLLib.hlFolderGetItemByPath(pItem, sArgument, HLLib.HLFindType.HL_FIND_ALL);

				        if(pSubItem != IntPtr.Zero)
				        {
					        Console.WriteLine("Information for {0}:", GetPath(pSubItem));
					        Console.WriteLine();

					        switch(HLLib.hlItemGetType(pSubItem))
					        {
					        case HLLib.HLDirectoryItemType.HL_ITEM_FOLDER:
						        Console.WriteLine("  Type: Folder");
						        Console.WriteLine("  Size: {0} B", HLLib.hlFolderGetSizeEx(pSubItem, true));
						        Console.WriteLine("  Size On Disk: {0} B", HLLib.hlFolderGetSizeOnDiskEx(pSubItem, true));
						        Console.WriteLine("  Folders: {0}", HLLib.hlFolderGetFolderCount(pSubItem, true));
						        Console.WriteLine("  Files: {0}", HLLib.hlFolderGetFileCount(pSubItem, true));
						        break;
					        case HLLib.HLDirectoryItemType.HL_ITEM_FILE:
						        Console.WriteLine("  Type: File");
						        Console.WriteLine("  Extractable: {0}", HLLib.hlFileGetExtractable(pSubItem) != 0 ? "True" : "False");
						        //Console.WriteLine("  Validates: {0}", HLLib.hlFileGetValidates(pSubItem) ? "True" : "False");
						        Console.WriteLine("  Size: {0} B", HLLib.hlFileGetSize(pSubItem));
						        Console.WriteLine("  Size On Disk: {0} B", HLLib.hlFileGetSizeOnDisk(pSubItem));
						        break;
					        }

					        uint uiAttributeCount = HLLib.hlPackageGetItemAttributeCount();
					        for(uint i = 0; i < uiAttributeCount; i++)
					        {
                                HLLib.HLAttribute Attribute;
						        if(HLLib.hlPackageGetItemAttribute(pSubItem, (HLLib.HLPackageAttribute)i, out Attribute))
						        {
                                    if(Attribute.eAttributeType != HLLib.HLAttributeType.HL_ATTRIBUTE_INVALID)
                                    {
                                        Console.WriteLine("  {0}: {1}", Attribute.GetName(), Attribute.ToString());
                                    }
						        }
					        }

					        Console.WriteLine();
				        }
				        else
				        {
					        Console.WriteLine("{0} not found.", sArgument);
				        }
			        }
		        }
		        //
		        // Extract item.
		        // Good example of CPackageUtility extract functions.
		        //
		        else if(String.Equals(sCommand, "extract", StringComparison.CurrentCultureIgnoreCase))
		        {
			        if(sArgument.Length == 0)
			        {
				        Console.WriteLine("No argument for command extract supplied.");
			        }
			        else
			        {
                        IntPtr pSubItem;
				        if(String.Equals(sArgument, ".", StringComparison.CurrentCultureIgnoreCase))
				        {
					        pSubItem = pItem;
				        }
				        else
				        {
					        pSubItem = HLLib.hlFolderGetItemByName(pItem, sArgument, HLLib.HLFindType.HL_FIND_ALL);
				        }

				        if(pSubItem != IntPtr.Zero)
				        {
					        // Extract the item.
					        // Item is extracted to cDestination\Item->GetName().
					        if(!bSilent)
					        {
						        Console.WriteLine("Extracting {0}...", HLLib.hlItemGetName(pSubItem));
						        Console.WriteLine();
					        }

					        HLLib.hlItemExtract(pSubItem, sDestination);

					        if(!bSilent)
					        {
						        Console.WriteLine("");
						        Console.WriteLine("Done.");
					        }
				        }
				        else
				        {
					        Console.WriteLine("{0} not found.", sArgument);
				        }
			        }
		        }
		        //
		        // Validate item.
		        // Validates the checksums of each item.
		        //
		        else if(String.Equals(sCommand, "validate", StringComparison.CurrentCultureIgnoreCase))
		        {
			        if(sArgument.Length == 0)
			        {
				        Console.WriteLine("No argument for command extract supplied.");
			        }
			        else
			        {
                        IntPtr pSubItem;
				        if(String.Equals(sArgument, ".", StringComparison.CurrentCultureIgnoreCase))
				        {
					        pSubItem = pItem;
				        }
				        else
				        {
					        pSubItem = HLLib.hlFolderGetItemByName(pItem, sArgument, HLLib.HLFindType.HL_FIND_ALL);
				        }

				        if(pSubItem != IntPtr.Zero)
				        {
					        if(!bSilent)
					        {
						        Console.WriteLine("Validating {0}...", HLLib.hlItemGetName(pSubItem));
						        Console.WriteLine();
					        }

					        Validate(pSubItem);

					        if(!bSilent)
					        {
						        Console.WriteLine();
						        Console.WriteLine("Done.");
					        }
				        }
				        else
				        {
					        Console.WriteLine("{0} not found.", sArgument);
				        }
			        }
		        }
                //
		        // Defragment package.
		        // Validates the checksums of each item.
		        //
		        else if(String.Equals(sCommand, "defragment", StringComparison.CurrentCultureIgnoreCase))
		        {
                    bool bForceDefragment = false;
			        if(sArgument.Length != 0)
			        {
                        try
                        {
                            bForceDefragment = Convert.ToBoolean(sArgument);
                        }
                        catch
                        {
                        }
			        }

			        if(!bSilent)
			        {
				        Console.WriteLine("Defragmenting...");
				        Console.WriteLine();
			        }

                    HLLib.hlSetBoolean(HLLib.HLOption.HL_FORCE_DEFRAGMENT, bForceDefragment);

                    Console.Write("  Progress: ");
                    ProgressStart();
                    if(!HLLib.hlPackageDefragment())
                    {
                        Console.Write(" {0}", HLLib.hlGetString(HLLib.HLOption.HL_ERROR_SHORT_FORMATED));
                    }

			        if(!bSilent)
			        {
                        Console.WriteLine();

				        Console.WriteLine();
				        Console.WriteLine("Done.");
			        }
		        }
		        //
		        // Find items.
		        // Good example of recursive directory navigation (Search() function).
		        //
		        else if(String.Equals(sCommand, "find", StringComparison.CurrentCultureIgnoreCase))
		        {
			        if(sArgument.Length == 0)
			        {
				        Console.WriteLine("No argument for command find supplied.");
			        }
			        else
			        {
				        // Search for the requested items.
				        if(!bSilent)
				        {
					        Console.WriteLine("Searching for {0}...", sArgument);
					        Console.WriteLine();
				        }

				        uint uiFolderCount = 0, uiFileCount = 0;
				        IntPtr pSubItem = HLLib.hlFolderFindFirst(pItem, sArgument, HLLib.HLFindType.HL_FIND_ALL);
				        while(pSubItem != IntPtr.Zero)
				        {
                            switch(HLLib.hlItemGetType(pSubItem))
                            {
                                case HLLib.HLDirectoryItemType.HL_ITEM_FOLDER:
                                    uiFolderCount++;

                                    Console.WriteLine("  Found folder: {0}", GetPath(pSubItem));
                                    break;
                                case HLLib.HLDirectoryItemType.HL_ITEM_FILE:
                                    uiFileCount++;

                                    Console.WriteLine("  Found file: {0}", GetPath(pSubItem));
                                    break;
                            }

					        pSubItem = HLLib.hlFolderFindNext(pItem, pSubItem, sArgument, HLLib.HLFindType.HL_FIND_ALL);
				        }

				        if(!bSilent)
				        {
					        if(uiFolderCount != 0 || uiFileCount != 0)
					        {
						        Console.WriteLine();
					        }

					        Console.WriteLine("  {0} folder{1} and {2} file{3} found.", uiFolderCount, uiFolderCount != 1 ? "s" : "", uiFileCount, uiFileCount != 1 ? "s" : "");
					        Console.WriteLine();
				        }
			        }
		        }
		        //
		        // Type files.
		        // Good example of reading files into memory.
		        //
		        else if(String.Equals(sCommand, "type", StringComparison.CurrentCultureIgnoreCase))
		        {
			        if(sArgument.Length == 0)
			        {
				        Console.WriteLine("No argument for command type supplied.");
			        }
			        else
			        {
				        IntPtr pSubItem = HLLib.hlFolderGetItemByName(pItem, sArgument, HLLib.HLFindType.HL_FIND_FILES);

				        if(pSubItem != IntPtr.Zero)
				        {
					        if(!bSilent)
					        {
						        Console.WriteLine("Type for {0}:", GetPath(pSubItem));
						        Console.WriteLine();
					        }

                            IntPtr pStream;
					        if(HLLib.hlFileCreateStream(pSubItem, out pStream))
					        {
						        if(HLLib.hlStreamOpen(pStream, (uint)HLLib.HLFileMode.HL_MODE_READ))
						        {
                                    char iChar;
							        while(HLLib.hlStreamReadChar(pStream, out iChar))
							        {
								        if((iChar >= ' ' && iChar <= '~') || iChar == '\n' || iChar == '\t')
								        {
									        Console.Write(iChar);
								        }
							        }

							        HLLib.hlStreamClose(pStream);
						        }
						        else
						        {
                                    Console.WriteLine("Error typing {0}:", HLLib.hlItemGetName(pSubItem));
							        Console.WriteLine(HLLib.hlGetString(HLLib.HLOption.HL_ERROR_SHORT_FORMATED));
						        }

						        HLLib.hlFileReleaseStream(pSubItem, pStream);
					        }
					        else
					        {
                                Console.WriteLine("Error typing {0}:", HLLib.hlItemGetName(pSubItem));
						        Console.WriteLine(HLLib.hlGetString(HLLib.HLOption.HL_ERROR_SHORT_FORMATED));
					        }

					        if(!bSilent)
					        {
						        Console.WriteLine();
						        Console.WriteLine("Done.");
					        }
				        }
				        else
				        {
					        Console.WriteLine("{0} not found.", sArgument);
				        }
			        }
		        }
		        //
		        // Open item.
		        // Good example of opening packages inside packages.
		        //
		        else if(String.Equals(sCommand, "open", StringComparison.CurrentCultureIgnoreCase))
		        {
			        if(sArgument.Length == 0)
			        {
				        Console.WriteLine("No argument for command open supplied.");
			        }
			        else
			        {
				        IntPtr pSubItem = HLLib.hlFolderGetItemByName(pItem, sArgument, HLLib.HLFindType.HL_FIND_FILES);

				        if(pSubItem != IntPtr.Zero)
				        {
                            IntPtr pStream;
					        if(HLLib.hlFileCreateStream(pSubItem, out pStream))
					        {
						        if(HLLib.hlStreamOpen(pStream, (uint)HLLib.HLFileMode.HL_MODE_READ))
						        {
							        HLLib.HLPackageType ePackageType = HLLib.hlGetPackageTypeFromStream(pStream);

                                    uint uiSubPackage;
							        if(HLLib.hlCreatePackage(ePackageType, out uiSubPackage))
							        {
								        HLLib.hlBindPackage(uiSubPackage);
								        if(HLLib.hlPackageOpenStream(pStream, (uint)HLLib.HLFileMode.HL_MODE_READ))
								        {
									        if(!bSilent)
										        Console.WriteLine("{0} opened.", HLLib.hlItemGetName(pSubItem));

									        EnterConsole(uiSubPackage, Commands);

									        HLLib.hlPackageClose();

									        if(!bSilent)
										        Console.WriteLine("{0} closed.", HLLib.hlItemGetName(pSubItem));
								        }
								        else
								        {
                                            Console.WriteLine("Error opening {0}:", HLLib.hlItemGetName(pSubItem));
						                    Console.WriteLine(HLLib.hlGetString(HLLib.HLOption.HL_ERROR_SHORT_FORMATED));
								        }

								        HLLib.hlDeletePackage(uiSubPackage);

								        HLLib.hlBindPackage(uiPackage);
							        }
							        else
							        {
                                        Console.WriteLine("Error opening {0}:", HLLib.hlItemGetName(pSubItem));
						                Console.WriteLine(HLLib.hlGetString(HLLib.HLOption.HL_ERROR_SHORT_FORMATED));
							        }

							        HLLib.hlStreamClose(pStream);
						        }
						        else
						        {
                                    Console.WriteLine("Error opening {0}:", HLLib.hlItemGetName(pSubItem));
						            Console.WriteLine(HLLib.hlGetString(HLLib.HLOption.HL_ERROR_SHORT_FORMATED));
						        }

						        HLLib.hlFileReleaseStream(pSubItem, pStream);
					        }
					        else
					        {
                                Console.WriteLine("Error opening {0}:", HLLib.hlItemGetName(pSubItem));
						        Console.WriteLine(HLLib.hlGetString(HLLib.HLOption.HL_ERROR_SHORT_FORMATED));
					        }
				        }
				        else
				        {
					        Console.WriteLine("{0} not found.", sArgument);
				        }
			        }
		        }
		        //
		        // Clear screen.
		        //
		        else if(String.Equals(sCommand, "status", StringComparison.CurrentCultureIgnoreCase))
		        {
			        Console.WriteLine("Total size: {0} B", HLLib.hlGetUnsignedInteger(HLLib.HLOption.HL_PACKAGE_SIZE));
			        Console.WriteLine("Total mapping allocations: {0}", HLLib.hlGetUnsignedInteger(HLLib.HLOption.HL_PACKAGE_TOTAL_ALLOCATIONS));
			        Console.WriteLine("Total mapping memory allocated: {0} B", HLLib.hlGetUnsignedInteger(HLLib.HLOption.HL_PACKAGE_TOTAL_MEMORY_ALLOCATED));
			        Console.WriteLine("Total mapping memory used: {0} B", HLLib.hlGetUnsignedInteger(HLLib.HLOption.HL_PACKAGE_TOTAL_MEMORY_USED));

			        uint uiAttributeCount = HLLib.hlPackageGetAttributeCount();
			        for(uint i = 0; i < uiAttributeCount; i++)
			        {
                        HLLib.HLAttribute Attribute;
				        if(HLLib.hlPackageGetAttribute((HLLib.HLPackageAttribute)i, out Attribute))
				        {
                            if(Attribute.eAttributeType != HLLib.HLAttributeType.HL_ATTRIBUTE_INVALID)
                            {
                                Console.WriteLine("{0}: {1}", Attribute.GetName(), Attribute.ToString());
                            }
				        }
			        }
		        }
                else if(String.Equals(sCommand, "exec", StringComparison.CurrentCultureIgnoreCase) || String.Equals(sCommand, "execute", StringComparison.CurrentCultureIgnoreCase))
                {
                    if(sArgument.Length == 0)
                    {
                        Console.WriteLine("No argument for command open supplied.");
                    }
                    else
                    {
                        string[] NewCommands = sArgument.Split(new char[] { '|' });
                        Commands.AddRange(NewCommands);
                    }
                }
                else if(String.Equals(sCommand, "cls", StringComparison.CurrentCultureIgnoreCase))
                {
                    try
                    {
                        Console.Clear();
                    }
                    catch
                    {
                    }
                }
                else if(String.Equals(sCommand, "help", StringComparison.CurrentCultureIgnoreCase))
                {
                    Console.WriteLine("Valid commands:");
                    Console.WriteLine();
                    Console.WriteLine("dir <filter>       (Directory list.)");
                    Console.WriteLine("cd <folder>        (Change directroy.)");
                    Console.WriteLine("info <item>        (Item information.)");
                    Console.WriteLine("extract <item>     (Extract item.)");
                    Console.WriteLine("validate <item>    (Validate item.)");
                    Console.WriteLine("defragment [force] (Defragment package.)");
                    Console.WriteLine("find <filter>      (Find item.)");
                    Console.WriteLine("type <file>        (Type a file.)");
                    Console.WriteLine("open <file>        (Open a nested package.)");
                    Console.WriteLine("root               (Go to the root folder.)");
                    Console.WriteLine("status             (Package information.)");
                    Console.WriteLine("execute <cmd>|...  (Execute 1 or more pipe delimited commands.)");
                    Console.WriteLine("cls                (Clear the screen.)");
                    Console.WriteLine("help               (Program help.)");
                    Console.WriteLine("exit               (Quit program.)");
                    Console.WriteLine();
                }
                else if(String.Equals(sCommand, "exit", StringComparison.CurrentCultureIgnoreCase))
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Unkown command: {0}", sCommand);
                }
	        }
        }
    }
}

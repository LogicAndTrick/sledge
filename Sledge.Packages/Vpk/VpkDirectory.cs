using System;
using System.Collections.Generic;
using System.IO;

namespace Sledge.Packages.Vpk
{
    // https://developer.valvesoftware.com/wiki/VPK_File_Format
    public class VpkDirectory : IPackage
    {
        private const uint Signature = 0x55aa1234;
        private const string DirString = "_dir";
        internal const ushort DirectoryIndex = 0x7fff;

        public FileInfo File { get; private set; }
        public uint Version { get; private set; }
        internal uint TreeLength { get; private set; }
        internal uint HeaderLength { get; private set; }

        internal List<VpkEntry> Entries { get; private set; }
        internal Dictionary<ushort, FileInfo> Chunks { get; private set; }

        public VpkDirectory(FileInfo file)
        {
            File = file;
            Entries = new List<VpkEntry>();
            Chunks = new Dictionary<ushort, FileInfo>();

            var nameWithoutExt = Path.GetFileNameWithoutExtension(file.Name);
            var ext = Path.GetExtension(file.Name);
            if (!nameWithoutExt.EndsWith(DirString)) throw new PackageException("This is not a valid VPK directory file.");
            
            var baseName = nameWithoutExt.Substring(0, nameWithoutExt.Length - DirString.Length);

            // Scan and find all chunk files that match this vpk directory
            var matchingFiles = file.Directory.GetFiles(baseName + "_???" + ext);
            foreach (var mf in matchingFiles)
            {
                var index = mf.Name.Substring(baseName.Length + 1, 3);
                ushort num;
                if (ushort.TryParse(index, out num))
                {
                    Chunks.Add(num, mf);
                }
            }
            Chunks[DirectoryIndex] = File;

            // Read the data from the vpk
            using (var br = new BinaryReader(OpenFile(file)))
            {
                var sig = br.ReadUInt32();
                if (sig != Signature) throw new PackageException("Unknown package signature: Expected 0x" + Signature.ToString("x8") + ", got 0x" + sig.ToString("x8") + ".");

                Version = br.ReadUInt32();
                TreeLength = br.ReadUInt32();
                switch (Version)
                {
                    case 1:
                        HeaderLength = 12;
                        break;
                    case 2:
                        HeaderLength = 28;
                        br.ReadInt32(); // Unknown1
                        var footerLength = br.ReadUInt32();
                        br.ReadInt32(); // Unknown3
                        br.ReadInt32(); // Unknown4
                        break;
                    default:
                        throw new PackageException("Unknown version number: Expected 1 or 2, got " + Version + ".");
                }

                // Read all the entries from the vpk
                ReadDirectoryEntries(br);
            }
        }
        
        internal FileStream OpenFile(FileInfo file)
        {
            return new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 4096, FileOptions.RandomAccess);
        }

        private void ReadDirectoryEntries(BinaryReader br)
        {
            String extension, path, filename;
            while ((extension = br.ReadNullTerminatedString()).Length > 0)
            {
                while ((path = br.ReadNullTerminatedString()).Length > 0)
                {
                    // Single space = root directory
                    path = path == " " ? "" : path.Replace('\\', '/').Trim('/');
                    while ((filename = br.ReadNullTerminatedString()).Length > 0)
                    {
                        // get me some file information
                        var entry = ReadEntry(br, path + "/" + filename + "." + extension);
                        Entries.Add(entry);
                    }
                }
            }
        }

        private VpkEntry ReadEntry(BinaryReader br, string path)
        {
            var crc = br.ReadUInt32();
            var preloadBytes = br.ReadUInt16();
            var archiveIndex = br.ReadUInt16(); // 0x7fff = directory archive
            var entryOffset = br.ReadUInt32(); // If archive directory, relative to END of directory structure
            var entryLength = br.ReadUInt32(); // If 0, preload data contains the entire file
            var terminator = br.ReadUInt16();
            if (terminator != VpkEntry.EntryTerminator) throw new PackageException("Invalid terminator. Expected " + VpkEntry.EntryTerminator.ToString("x8") + ", got " + terminator.ToString("x8") + ".");

            var preloadData = br.ReadBytes(preloadBytes);
            return new VpkEntry(this, path, crc, preloadData, archiveIndex, entryOffset, entryLength);
        }

        public IEnumerable<IPackageEntry> GetEntries()
        {
            return Entries;
        }

        public byte[] ExtractEntry(IPackageEntry entry)
        {
            using (var sr = new BinaryReader(OpenStream(entry)))
            {
                return sr.ReadBytes((int) sr.BaseStream.Length);
            }
        }

        public Stream OpenStream(IPackageEntry entry)
        {
            var pe = entry as VpkEntry;
            if (pe == null) throw new ArgumentException("This package is only compatible with VpkEntry objects.");
            return new VpkEntryStream(pe, this);
        }

        internal Stream OpenChunk(VpkEntry entry)
        {
            var file = Chunks[entry.ArchiveIndex];
            var stream = OpenFile(file);
            var offset = entry.ArchiveIndex == DirectoryIndex ? HeaderLength + TreeLength + entry.EntryOffset : entry.EntryOffset;
            stream.Position = offset;
            return stream;
        }

        public IPackageStreamSource GetStreamSource()
        {
            return new VpkPackageStreamSource(this);
        }

        public void Dispose()
        {
            Entries.Clear();
            Chunks.Clear();
        }
    }
}

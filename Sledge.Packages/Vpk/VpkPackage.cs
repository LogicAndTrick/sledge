using System;
using System.Collections.Generic;
using System.IO;

namespace Sledge.Packages.Vpk
{
    // https://developer.valvesoftware.com/wiki/VPK_File_Format
    public class VpkDirectory : IDisposable
    {
        private const uint Signature = 0x55aa1234;
        private const string DirString = "_dir";
        private const ushort DirectoryIndex = 0x7fff;

        public FileInfo File { get; private set; }
        public uint Version { get; private set; }
        internal uint TreeLength { get; private set; }
        internal uint HeaderLength { get; private set; }

        private readonly List<VpkEntry> _entries;
        private readonly Dictionary<ushort, FileInfo> _chunks;

        public VpkDirectory(FileInfo file)
        {
            File = file;
            _entries = new List<VpkEntry>();
            _chunks = new Dictionary<ushort, FileInfo>();

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
                    _chunks.Add(num, mf);
                }
            }
            _chunks[DirectoryIndex] = File;

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
        
        private FileStream OpenFile(FileInfo file)
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
                        _entries.Add(entry);
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
            return new VpkEntry(path, crc, preloadData, archiveIndex, entryOffset, entryLength);
        }

        public IEnumerable<VpkEntry> GetEntries()
        {
            return _entries;
        }

        public byte[] ExtractEntry(VpkEntry entry)
        {
            using (var sr = new BinaryReader(OpenStream(entry)))
            {
                return sr.ReadBytes(entry.TotalLength);
            }
        }

        public Stream OpenStream(VpkEntry entry)
        {
            return new VpkEntryStream(entry, this);
        }

        internal Stream OpenChunk(VpkEntry entry)
        {
            var file = _chunks[entry.ArchiveIndex];
            var stream = OpenFile(file);
            var offset = entry.ArchiveIndex == DirectoryIndex ? HeaderLength + TreeLength + entry.EntryOffset : entry.EntryOffset;
            stream.Position = offset;
            return stream;
        }

        public void Dispose()
        {
            _entries.Clear();
            _chunks.Clear();
        }
    }
}

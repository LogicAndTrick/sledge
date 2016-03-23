using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Sledge.Packages.Zip
{
	public class ZipArchive : IDisposable
	{
		private List<ZipEntry> _entries;
		private Dictionary<string, FileEntry> _files;
		private MemoryStream _stream;

		public string Path { get; set; }

		public ZipArchive(MemoryStream stream)
		{
			_entries = new List<ZipEntry>();
			_stream = stream;
			var br = new BinaryReader(_stream, Encoding.UTF8);
			//using ()
			{
				ZipEntry entry;
				do
				{
					entry = ReadEntry(br);
					_entries.Add(entry);
				} while (entry != null && entry.Type != ZipEntryType.EndOfCentralDirectory);
				_files = _entries.OfType<FileEntry>().ToDictionary(x => x.FileName, x => x);
			}
			_stream.Seek(0, SeekOrigin.Begin);
		}

		public void Dispose()
		{
			_stream.Dispose();
		}

		public IEnumerable<string> GetDirectories()
		{
			return _files.Where(x => x.Value.UncompressedSize == 0).Select(x => x.Key);
		}

		public IEnumerable<string> GetFiles()
		{
			return _files.Where(x => x.Value.UncompressedSize > 0).Select(x => x.Key);
		}

		public MemoryStream GetFileStream(string path)
		{
			var file = _files[path];
			return file.GetStream(_stream);
		}

		private ZipEntry ReadEntry(BinaryReader br)
		{
			var type = (ZipEntryType)br.ReadUInt32();
			br.BaseStream.Seek(-4, SeekOrigin.Current);
			switch (type)
			{
				case ZipEntryType.File:
					return new FileEntry(br);
				case ZipEntryType.DataDescriptor:
					return new DataDescriptorEntry(br);
				case ZipEntryType.CentralDirectory:
					return new CentralDirectoryEntry(br);
				case ZipEntryType.EndOfCentralDirectory:
					return new EndOfCentralDirectoryEntry(br);
			}
			throw new NotSupportedException("Unknown entry type: " + ((uint)type).ToString("X"));
		}

		private class ZipEntry
		{
			public long Offset { get; set; }
			public ZipEntryType Type { get; set; }
		}

		private class FileEntry : ZipEntry
		{
			public CompressionMethod CompressionMethod { get; set; }
			public uint CompressedSize { get; set; }
			public uint UncompressedSize { get; set; }
			public string FileName { get; set; }
			public long DataStartOffset { get; set; }

			public FileEntry(BinaryReader br)
			{
				Offset = br.BaseStream.Position;
				Type = ZipEntryType.File;

				if (br.ReadUInt32() != (uint)ZipEntryType.File) throw new Exception("ZIP header is incorrect");
				br.BaseStream.Seek(4, SeekOrigin.Current);
				CompressionMethod = (CompressionMethod)br.ReadUInt16();
				br.BaseStream.Seek(8, SeekOrigin.Current);
				CompressedSize = br.ReadUInt32();
				UncompressedSize = br.ReadUInt32();
				var fileNameLength = br.ReadUInt16();
				var extraLength = br.ReadUInt16();
				FileName = new string(br.ReadChars(fileNameLength));
				br.BaseStream.Seek(extraLength, SeekOrigin.Current);
				DataStartOffset = br.BaseStream.Position;
				br.BaseStream.Seek(CompressedSize, SeekOrigin.Current);
			}

			public MemoryStream GetStream(MemoryStream container)
			{
				byte[] bytesOut = new byte[UncompressedSize];
				var oldPos = 0L;
                switch (CompressionMethod)
				{
					case CompressionMethod.None:

						oldPos = container.Position;
						container.Position = DataStartOffset;
						container.Read(bytesOut, 0, (int)CompressedSize);
						container.Position = oldPos;
						return new MemoryStream(bytesOut);
					//return new SubStream(container, DataStartOffset, CompressedSize);
					case CompressionMethod.Deflate:

						byte[] bytesIn = new byte[CompressedSize];
						oldPos = container.Position;
						container.Position = DataStartOffset;
                        container.Read(bytesIn, 0, (int)CompressedSize);
						MemoryStream bytesInMemoryStream = new MemoryStream(bytesIn);
						var rtn = new System.IO.Compression.DeflateStream(bytesInMemoryStream, System.IO.Compression.CompressionMode.Decompress, true);
						rtn.Read(bytesOut, 0, (int)UncompressedSize);
						var ms = new MemoryStream(bytesOut);
						container.Position = oldPos;
						return ms;
					default:
						throw new NotSupportedException("This zip compression method (" + CompressionMethod + ") is not supported.");
				}
			}
		}

		private class DataDescriptorEntry : ZipEntry
		{
			public uint CompressedSize { get; set; }
			public uint UncompressedSize { get; set; }

			public DataDescriptorEntry(BinaryReader br)
			{
				Offset = br.BaseStream.Position;
				Type = ZipEntryType.DataDescriptor;

				if (br.ReadUInt32() != (uint)ZipEntryType.DataDescriptor) throw new Exception("ZIP header is incorrect");

				var crc = br.ReadUInt32();
				CompressedSize = br.ReadUInt32();
				UncompressedSize = br.ReadUInt32();
			}
		}

		private class CentralDirectoryEntry : ZipEntry
		{
			public CompressionMethod CompressionMethod { get; set; }
			public uint CompressedSize { get; set; }
			public uint UncompressedSize { get; set; }
			public string FileName { get; set; }
			public uint FileOffset { get; set; }

			public CentralDirectoryEntry(BinaryReader br)
			{
				Offset = br.BaseStream.Position;
				Type = ZipEntryType.CentralDirectory;

				if (br.ReadUInt32() != (uint)ZipEntryType.CentralDirectory) throw new Exception("ZIP header is incorrect");
				br.BaseStream.Seek(6, SeekOrigin.Current);
				CompressionMethod = (CompressionMethod)br.ReadUInt16();
				br.BaseStream.Seek(8, SeekOrigin.Current);
				CompressedSize = br.ReadUInt32();
				UncompressedSize = br.ReadUInt32();
				var fileNameLength = br.ReadUInt16();
				var extraLength = br.ReadUInt16();
				var commentLength = br.ReadUInt16();
				br.BaseStream.Seek(8, SeekOrigin.Current);
				FileOffset = br.ReadUInt32();
				FileName = new string(br.ReadChars(fileNameLength));
				br.BaseStream.Seek(extraLength + commentLength, SeekOrigin.Current);
			}
		}

		private class EndOfCentralDirectoryEntry : ZipEntry
		{
			public uint CentralOffset { get; set; }

			public EndOfCentralDirectoryEntry(BinaryReader br)
			{
				Offset = br.BaseStream.Position;
				Type = ZipEntryType.EndOfCentralDirectory;

				if (br.ReadUInt32() != (uint)ZipEntryType.EndOfCentralDirectory) throw new Exception("ZIP header is incorrect");

				br.BaseStream.Seek(12, SeekOrigin.Current);
				CentralOffset = br.ReadUInt32();

				var commentLength = br.ReadUInt16();
				br.BaseStream.Seek(commentLength, SeekOrigin.Current);
			}
		}

		private enum ZipEntryType : uint
		{
			File = 0x04034b50,
			DataDescriptor = 0x08074b50,
			CentralDirectory = 0x02014b50,
			EndOfCentralDirectory = 0x06054b50,
		}

		private enum CompressionMethod : ushort
		{
			None = 0, // The file is stored (no compression)
			Shrink = 1, // The file is Shrunk
			Compression1 = 2, // The file is Reduced with compression factor 1
			Compression2 = 3, // The file is Reduced with compression factor 2
			Compression3 = 4, // The file is Reduced with compression factor 3
			Compression4 = 5, // The file is Reduced with compression factor 4
			Imploded = 6, // The file is Imploded
						  //　7, // Reserved for Tokenizing compression algorithm
			Deflate = 8, // The file is Deflated
			Deflate64 = 9, // Enhanced Deflating using Deflate64(tm)
			IbmTerseOld = 10, // PKWARE Data Compression Library Imploding (old IBM TERSE)
							  // 11, // Reserved by PKWARE
			Bzip2 = 12, // File is compressed using BZIP2 algorithm
						// 13, // Reserved by PKWARE
			Lzma = 14, // LZMA (EFS)
					   // 15, // Reserved by PKWARE
					   // 16, // Reserved by PKWARE
					   // 17, // Reserved by PKWARE
			IbmTerse = 18, // File is compressed using IBM TERSE (new)
			Lz77 = 19, // IBM LZ77 z Architecture (PFS)
			WavPack = 97, // WavPack compressed data
			Ppmd = 98, // PPMd version I, Rev 1
		}
	}
}

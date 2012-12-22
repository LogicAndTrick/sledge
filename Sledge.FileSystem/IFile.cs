using System.Collections.Generic;
using System.IO;

namespace Sledge.FileSystem
{
    /// <summary>
    /// Represents a file or container in any file system
    /// </summary>
    public interface IFile
    {
        /// <summary>
        /// If you really need to know the implementation of this file, it is exposed via this property.
        /// </summary>
        FileSystemType Type { get; }

        /// <summary>
        /// The file's parent container
        /// </summary>
        IFile Parent { get; }

        /// <summary>
        /// The file's fully resolved path name
        /// </summary>
        string FullPathName { get; }

        /// <summary>
        /// The file's full name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The file's name with no extension
        /// </summary>
        string NameWithoutExtension { get; }

        /// <summary>
        /// The file's extension, without the leading dot
        /// </summary>
        string Extension { get; }

        /// <summary>
        /// True if the file exists, false otherwise
        /// </summary>
        bool Exists { get; }

        /// <summary>
        /// Returns the size of the file in bytes
        /// </summary>
        long Size { get; }

        /// <summary>
        /// True if this a file container, or false if it is just a file
        /// </summary>
        bool IsContainer { get; }

        /// <summary>
        /// The number of children containers in this container
        /// </summary>
        int NumChildren { get; }

        /// <summary>
        /// The number of files in this container
        /// </summary>
        int NumFiles { get; }

        /// <summary>
        /// Opens the file for reading. The caller is responsible for disposing of the stream.
        /// </summary>
        /// <returns>The file in stream format</returns>
        Stream Open();

        /// <summary>
        /// Read all bytes from the file and return the resulting array.
        /// </summary>
        /// <returns>The entire file contents</returns>
        byte[] ReadAll();

        /// <summary>
        /// Read the specified number of bytes from the file and return the resulting array.
        /// </summary>
        /// <param name="offset">The number of bytes to skip before starting to read</param>
        /// <param name="count">The number of bytes to read</param>
        /// <returns>The file contents, starting at the specified offset and reading the specified number of bytes</returns>
        byte[] Read(long offset, long count);

        /// <summary>
        /// Find all files in the parent container that are related to this file.
        /// Related files have the same name before the first '.' in the filename.
        /// </summary>
        /// <returns>All the related files in the parent container</returns>
        IEnumerable<IFile> GetRelatedFiles();

        /// <summary>
        /// Find the file in the parent container is related to this file and has the given extension.
        /// Related files have the same name before the first '.' in the filename.
        /// </summary>
        /// <returns>The related file in the parent container with the matching extension, or null if it doesn't exist.</returns>
        IFile GetRelatedFile(string extension);

        /// <summary>
        /// Get the child container in this container with the given name
        /// </summary>
        /// <param name="name">The name of the container</param>
        /// <returns>The container</returns>
        IFile GetChild(string name);

        /// <summary>
        /// Get all the children containers in this container
        /// </summary>
        /// <returns>The list of containers</returns>
        IEnumerable<IFile> GetChildren();

        /// <summary>
        /// Get all the children containers in this container that match the given regular expression
        /// </summary>
        /// <param name="regex">The match expression</param>
        /// <returns>The list of containers that match the given expression</returns>
        IEnumerable<IFile> GetChildren(string regex);

        /// <summary>
        /// Get the file in this container with the given name
        /// </summary>
        /// <param name="name">The name of the file</param>
        /// <returns>The file</returns>
        IFile GetFile(string name);

        /// <summary>
        /// Get all the files in this container
        /// </summary>
        /// <returns>The list of files</returns>
        IEnumerable<IFile> GetFiles();

        /// <summary>
        /// Get all the files in this container that match the given regular expression
        /// </summary>
        /// <param name="regex">The match expression</param>
        /// <returns>The list of files that match the given expression</returns>
        IEnumerable<IFile> GetFiles(string regex);

        /// <summary>
        /// Get all the files in this container that have the given extension
        /// </summary>
        /// <param name="extension">The extension to match</param>
        /// <returns>The list of files that have the given extension</returns>
        IEnumerable<IFile> GetFilesWithExtension(string extension);
    }
}
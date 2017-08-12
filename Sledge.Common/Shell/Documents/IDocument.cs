namespace Sledge.Common.Shell.Documents
{
    public interface IDocument
    {
        /// <summary>
        /// Name of this document, will be shown in the UI tab
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// The physical location on disk of this document
        /// </summary>
        string FileName { get; set; }

        /// <summary>
        /// Mountable control for this document
        /// </summary>
        object Control { get; }
    }
}
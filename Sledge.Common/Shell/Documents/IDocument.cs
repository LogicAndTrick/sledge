namespace Sledge.Common.Shell.Documents
{
    public interface IDocument
    {
        /// <summary>
        /// Name of this document, will be shown in the UI tab
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Mountable control for this document
        /// </summary>
        object Control { get; }
    }
}
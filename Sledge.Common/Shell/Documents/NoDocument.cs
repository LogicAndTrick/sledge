namespace Sledge.Common.Shell.Documents
{
    /// <summary>
    /// An empty document.
    /// </summary>
    public class NoDocument : IDocument
    {
        public string Name { get; set; }
        public string FileName { get; set; }
        public object Control => null;
        public bool HasUnsavedChanges => false;
    }
}
namespace Sledge.Common.Shell.Documents
{
    public class NoDocument : IDocument
    {
        public string Name { get; set; }
        public string FileName { get; set; }
        public object Control => null;
        public bool HasUnsavedChanges => false;
    }
}
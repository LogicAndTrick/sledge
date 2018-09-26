namespace Sledge.Common.Shell.Documents
{
    public class DocumentCloseMessage
    {
        public IDocument Document { get; }
        public bool Cancelled { get; private set; }

        public DocumentCloseMessage(IDocument document)
        {
            Document = document;
            Cancelled = false;
        }

        public void Cancel() => Cancelled = true;
    }
}

namespace Sledge.EditorNew.Documents
{
    public class DummyDocument : IDocument
    {
        public string Text { get; set; }

        public DummyDocument(string text)
        {
            Text = text;
        }

        public void Activate()
        {
            // 
        }

        public void Deactivate()
        {
            // 
        }

        public void Close()
        {
            
        }
    }
}
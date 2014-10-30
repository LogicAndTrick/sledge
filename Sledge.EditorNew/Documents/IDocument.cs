using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sledge.EditorNew.Documents
{
    public interface IDocument
    {
        void Activate();
        void Deactivate();
        void Close();
        string Text { get; set; }
    }

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

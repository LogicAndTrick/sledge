using System.Collections.Generic;
using Sledge.Editor.Menu;
using Sledge.Editor.Tools;

namespace Sledge.Editor.Documents
{
    public static class DocumentManager
    {
        public static List<Document> Documents { get; private set; }
        public static Document CurrentDocument { get; private set; }

        static DocumentManager()
        {
            Documents = new List<Document>();
        }

        public static void Add(Document doc)
        {
            Documents.Add(doc);
        }

        public static void Remove(Document doc)
        {
            doc.Close();
            Documents.Remove(doc);
        }

        public static void SwitchTo(Document doc)
        {
            if (CurrentDocument != null) CurrentDocument.SetInactive();
            CurrentDocument = doc;
            ToolManager.SetDocument(doc);
            if (CurrentDocument != null) CurrentDocument.SetActive();
            MenuManager.Rebuild();
        }

        public static void AddAndSwitch(Document doc)
        {
            Add(doc);
            SwitchTo(doc);
        }
    }
}

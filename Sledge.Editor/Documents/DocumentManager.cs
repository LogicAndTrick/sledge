using System.Collections.Generic;
using Sledge.Common.Mediator;
using Sledge.Editor.Tools;

namespace Sledge.Editor.Documents
{
    public static class DocumentManager
    {
        public static List<Document> Documents { get; private set; }
        public static Document CurrentDocument { get; private set; }

        private static int _untitledCount = 1;

        public static string GetUntitledDocumentName()
        {
            return "Untitled " + _untitledCount++;
        }

        static DocumentManager()
        {
            Documents = new List<Document>();
        }

        public static void Add(Document doc)
        {
            Documents.Add(doc);
            Mediator.Publish(EditorMediator.DocumentOpened, doc);
        }

        public static void Remove(Document doc)
        {
            var current = doc == CurrentDocument;
            var index = Documents.IndexOf(doc);

            if (current && Documents.Count > 1)
            {
                var ni = index + 1;
                if (ni >= Documents.Count) ni = index - 1;
                SwitchTo(Documents[ni]);
            }

            doc.Close();
            Documents.Remove(doc);
            Mediator.Publish(EditorMediator.DocumentClosed, doc);

            if (Documents.Count == 0)
            {
                SwitchTo(null);
                Mediator.Publish(EditorMediator.DocumentAllClosed);
            }
            
        }

        public static void SwitchTo(Document doc)
        {
            if (CurrentDocument != null)
            {
                CurrentDocument.SetInactive();
                Mediator.Publish(EditorMediator.DocumentDeactivated, CurrentDocument);
            }

            CurrentDocument = doc;
            ToolManager.SetDocument(doc);

            if (CurrentDocument != null)
            {
                CurrentDocument.SetActive();
                Mediator.Publish(EditorMediator.DocumentActivated, CurrentDocument);
            }

            Mediator.Publish(EditorMediator.UpdateMenu);
        }

        public static void AddAndSwitch(Document doc)
        {
            Add(doc);
            SwitchTo(doc);
        }
    }
}

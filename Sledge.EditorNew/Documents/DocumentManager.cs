using System.Collections.Generic;
using Sledge.Common.Mediator;

namespace Sledge.EditorNew.Documents
{
    public static class DocumentManager
    {
        public static List<IDocument> Documents { get; private set; }
        public static IDocument CurrentDocument { get; private set; }

        private static int _untitledCount = 1;

        public static string GetUntitledDocumentName()
        {
            return "Untitled " + _untitledCount++;
        }

        static DocumentManager()
        {
            Documents = new List<IDocument>();
        }

        public static void Add(IDocument doc)
        {
            Documents.Add(doc);
            Mediator.Publish(EditorMediator.DocumentOpened, doc);
        }

        public static void Remove(IDocument doc)
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

        public static void SwitchTo(IDocument doc)
        {
            if (CurrentDocument != null)
            {
                CurrentDocument.Deactivate();
                Mediator.Publish(EditorMediator.DocumentDeactivated, CurrentDocument);
            }

            CurrentDocument = doc;
            // todo ToolManager.SetDocument(doc);

            if (CurrentDocument != null)
            {
                CurrentDocument.Activate();
                Mediator.Publish(EditorMediator.DocumentActivated, CurrentDocument);
            }

            Mediator.Publish(EditorMediator.UpdateMenu);
        }

        public static void AddAndSwitch(IDocument doc)
        {
            Add(doc);
            SwitchTo(doc);
        }
    }
}

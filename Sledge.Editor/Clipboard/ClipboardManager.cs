using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sledge.Common.Mediator;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Documents;
using Sledge.Providers;
using Sledge.Providers.Map;

namespace Sledge.Editor.Clipboard
{
    public static class ClipboardManager
    {
        public static int SizeOfClipboardRing { get; set; }
        private static readonly List<string> Ring;

        static ClipboardManager()
        {
            SizeOfClipboardRing = 10;
            Ring = new List<string>();
        }

        public static void Clear()
        {
            Ring.Clear();
        }

        public static void Push(IEnumerable<MapObject> copiedObjects)
        {
            // Remove extra entries if required
            while (Ring.Count > SizeOfClipboardRing - 1) Ring.RemoveAt(0);
            var item = CreateCopyStream(copiedObjects);
            if (Ring.Contains(item)) Ring.Remove(item);
            Ring.Add(item);
            System.Windows.Forms.Clipboard.SetText(item);
            Mediator.Publish(EditorMediator.ClipboardChanged);
        }

        public static IEnumerable<string> GetClipboardRing()
        {
            return new List<string>(Ring);
        }

        public static IEnumerable<MapObject> GetPastedContent(Document document)
        {
            if (!System.Windows.Forms.Clipboard.ContainsText()) return null;

            var str = System.Windows.Forms.Clipboard.GetText();
            if (!str.StartsWith("clipboard")) return null;

            return ExtractCopyStream(document, str);
        }

        /// <summary>
        /// Returns true if the paste operation will probably work.
        /// </summary>
        /// <returns>True if the start of the clipboard stream looks parsable.</returns>
        public static bool CanPaste()
        {
            if (!System.Windows.Forms.Clipboard.ContainsText()) return false;
            var str = System.Windows.Forms.Clipboard.GetText();
            return str.StartsWith("clipboard");
        }

        public static IEnumerable<MapObject> CloneFlatHeirarchy(Document document, IEnumerable<MapObject> objects)
        {
            return ExtractCopyStream(document, CreateCopyStream(objects));
        }

        private static string CreateCopyStream(IEnumerable<MapObject> copiedObjects)
        {
            var item = VmfProvider.CreateCopyStream(copiedObjects.ToList()).ToString();
            return item;
        }

        private static IEnumerable<MapObject> ExtractCopyStream(Document document, string str)
        {
            using (var tr = new StringReader(str))
            {
                try
                {
                    var gs = GenericStructure.Parse(tr);
                    return VmfProvider.ExtractCopyStream(gs.FirstOrDefault(), document.Map.IDGenerator);
                }
                catch
                {
                    return null;
                }
            }
        }
    }
}

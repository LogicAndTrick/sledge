using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
            var item = VmfProvider.CreateCopyStream(copiedObjects.ToList()).ToString();
            if (Ring.Contains(item)) Ring.Remove(item);
            Ring.Add(item);
            System.Windows.Forms.Clipboard.SetText(item);
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

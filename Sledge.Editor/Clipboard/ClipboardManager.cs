using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sledge.Editor.Clipboard
{
    public static class ClipboardManager
    {
        public static int SizeOfClipboardRing { get; set; }
        private static readonly List<ClipboardItem> Ring;

        static ClipboardManager()
        {
            SizeOfClipboardRing = 10;
            Ring = new List<ClipboardItem>();
        }

        public static void Clear()
        {
            Ring.Clear();
        }

        public static void Push(ClipboardItem item)
        {
            // Remove extra entries if required
            while (Ring.Count > SizeOfClipboardRing - 1) Ring.RemoveAt(0);
            Ring.Add(item);
        }

        public static ClipboardItem Fetch()
        {
            return Ring.LastOrDefault();
        }

        public static IEnumerable<ClipboardItem> GetClipboardRing()
        {
            return new List<ClipboardItem>(Ring);
        }
    }
}

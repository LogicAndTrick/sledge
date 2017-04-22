using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Primitives.MapObjects;

namespace Sledge.BspEditor.Components
{
    [Export]
    public class ClipboardManager
    {
        public int SizeOfClipboardRing { get; set; }
        private readonly List<string> Ring;

        public ClipboardManager()
        {
            SizeOfClipboardRing = 10;
            Ring = new List<string>();
        }

        public void Clear()
        {
            Ring.Clear();
        }

        public void Push(IEnumerable<IMapObject> copiedObjects)
        {
            // Remove extra entries if required
            while (Ring.Count > SizeOfClipboardRing - 1) Ring.RemoveAt(0);
            var item = CreateCopyStream(copiedObjects);
            if (Ring.Contains(item)) Ring.Remove(item);
            Ring.Add(item);
            System.Windows.Forms.Clipboard.SetText(item);
            Oy.Publish("BspEditor:ClipboardChanged", this);
        }

        public IEnumerable<string> GetClipboardRing()
        {
            return new List<string>(Ring);
        }

        public IEnumerable<IMapObject> GetPastedContent(MapDocument document)
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
        public bool CanPaste()
        {
            if (!System.Windows.Forms.Clipboard.ContainsText()) return false;
            var str = System.Windows.Forms.Clipboard.GetText();
            return str.StartsWith("clipboard");
        }

        public IEnumerable<IMapObject> CloneFlatHeirarchy(MapDocument document, IEnumerable<IMapObject> objects)
        {
            return ExtractCopyStream(document, CreateCopyStream(objects));
        }

        private static string CreateCopyStream(IEnumerable<IMapObject> copiedObjects)
        {
            // todo !serialisation copy/paste
            return $"{copiedObjects.Count()} objects copied";
            //var item = VmfProvider.CreateCopyStream(copiedObjects.ToList()).ToString();
            //return item;
        }

        private static IEnumerable<IMapObject> ExtractCopyStream(MapDocument document, string str)
        {
            // todo !serialisation copy/paste
            yield break;
            //using (var tr = new StringReader(str))
            //{
            //    try
            //    {
            //        var gs = GenericStructure.Parse(tr);
            //        return VmfProvider.ExtractCopyStream(gs.FirstOrDefault(), document.Map.NumberGenerator);
            //    }
            //    catch
            //    {
            //        return null;
            //    }
            //}
        }
    }
}

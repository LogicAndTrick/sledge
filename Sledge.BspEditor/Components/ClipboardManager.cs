using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Primitives;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common.Transport;

namespace Sledge.BspEditor.Components
{
    [Export]
    public class ClipboardManager
    {
        [Import] private MapElementFactory _factory;
        [Import] private SerialisedObjectFormatter _formatter;

        public int SizeOfClipboardRing { get; set; }
        private readonly List<string> Ring;

        private static string SerialisedName => "Sledge.BspEditor.Clipboard";

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
            if (!str.StartsWith(SerialisedName)) return null;

            return ExtractCopyStream(str);
        }

        /// <summary>
        /// Returns true if the paste operation will probably work.
        /// </summary>
        /// <returns>True if the start of the clipboard stream looks parsable.</returns>
        public bool CanPaste()
        {
            if (!System.Windows.Forms.Clipboard.ContainsText()) return false;
            var str = System.Windows.Forms.Clipboard.GetText();
            return str.StartsWith(SerialisedName);
        }

        private string CreateCopyStream(IEnumerable<IMapObject> copiedObjects)
        {
            var clip = new SerialisedObject(SerialisedName);
            foreach (var obj in copiedObjects)
            {
                var so = _factory.Serialise(obj);
                clip.Children.Add(so);
            }
            using (var ms = new MemoryStream())
            {
                _formatter.Serialize(ms, clip);
                ms.Position = 0;
                using (var sr = new StreamReader(ms))
                {
                    return sr.ReadToEnd();
                }
            }
        }

        private IEnumerable<IMapObject> ExtractCopyStream(string str)
        {
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(str)))
            {
                try
                {
                    var clip = _formatter.Deserialize(ms).First(x => x.Name == SerialisedName);
                    return clip.Children.Select(x => _factory.Deserialise(x)).OfType<IMapObject>();
                }
                catch
                {
                    return new IMapObject[0];
                }
            }
        }
    }
}

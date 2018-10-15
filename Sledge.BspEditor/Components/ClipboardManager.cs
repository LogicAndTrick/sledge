using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Primitives;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common.Translations;
using Sledge.Common.Transport;

namespace Sledge.BspEditor.Components
{
    [Export]
    [AutoTranslate]
    public class ClipboardManager
    {
        public class ClipboardEntry
        {
            public string Description { get; set; }
            public string Contents { get; set; }

            public ClipboardEntry(string description, string contents)
            {
                Description = description;
                Contents = contents;
            }

            public override string ToString()
            {
                return Description;
            }
        }

        private readonly MapElementFactory _factory;
        private readonly SerialisedObjectFormatter _formatter;

        public int SizeOfClipboardRing { get; set; }
        private readonly List<ClipboardEntry> Ring;
        
        public string NothingSelected { get; set; }
        public string NumSelected { get; set; }

        private static string SerialisedName => "Sledge.BspEditor.Clipboard";

        [ImportingConstructor]
        public ClipboardManager(
            [Import] Lazy<MapElementFactory> factory,
            [Import] Lazy<SerialisedObjectFormatter> formatter
        )
        {
            _factory = factory.Value;
            _formatter = formatter.Value;
            SizeOfClipboardRing = 10;
            Ring = new List<ClipboardEntry>();
        }

        public void Clear()
        {
            Ring.Clear();
        }

        public void Push(IEnumerable<IMapObject> copiedObjects)
        {
            // Remove extra entries if required
            while (Ring.Count > SizeOfClipboardRing - 1) Ring.RemoveAt(0);

            var list = copiedObjects.ToList();
            var contents = CreateCopyStream(list);
            Ring.Add(new ClipboardEntry(GetDescription(list), contents));

            System.Windows.Forms.Clipboard.SetText(contents);
            Oy.Publish("BspEditor:ClipboardChanged", this);
        }

        private string GetDescription(List<IMapObject> list)
        {
            switch (list.Count)
            {
                case 0:
                    return NothingSelected;
                case 1:
                    return list[0].GetType().Name;
                default:
                    return string.Format(NumSelected, list.Count);
            }
        }

        public IEnumerable<ClipboardEntry> GetClipboardRing()
        {
            return new List<ClipboardEntry>(Ring);
        }

        public IEnumerable<IMapObject> GetPastedContent(MapDocument document)
        {
            return GetPastedContent(document, (d, o) => o);
        }

        public IEnumerable<IMapObject> GetPastedContent(MapDocument document, Func<MapDocument, IMapObject, IMapObject> existingIdTransform)
        {
            if (!System.Windows.Forms.Clipboard.ContainsText()) return null;

            var str = System.Windows.Forms.Clipboard.GetText();
            if (!str.StartsWith(SerialisedName)) return null;

            var ecc = ExtractCopyStream(str);
            return ReIndex(ecc, document, existingIdTransform);
        }

        private IEnumerable<IMapObject> ReIndex(IEnumerable<IMapObject> objects, MapDocument document, Func<MapDocument, IMapObject, IMapObject> existingIdTransform)
        {
            var rand = new Random();
            foreach (var o in objects)
            {
                if (document.Map.Root.Hierarchy.HasDescendant(o.ID))
                {
                    // If this object already exists in the tree, transform it through the callback
                    yield return existingIdTransform(document, o);
                }
                else
                {
                    yield return o;
                }
            }
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

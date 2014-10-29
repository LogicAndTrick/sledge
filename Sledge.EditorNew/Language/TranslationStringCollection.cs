using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace Sledge.EditorNew.Language
{
    public class TranslationStringCollection : Dictionary<string, string>
    {
        private string _name;
        private TranslationStringCollection _parent;

        public TranslationStringCollection(string name, string resourceFolder)
        {
            using (var fs = FileFetcher(name, resourceFolder))
            {
                Read(fs, x => FileFetcher(x, resourceFolder));
            }
        }

        public TranslationStringCollection(string name, Func<string, Stream> resourceFetcher)
        {
            using (var rs = resourceFetcher(name))
            {
                Read(rs, resourceFetcher);
            }
        }

        public string Fetch(string key)
        {
            if (ContainsKey(key)) return this[key];
            if (_parent != null) return _parent.Fetch(key);
            return null;
        }

        private Stream FileFetcher(string name, string folder)
        {
            const string ext = ".xml";
            return new FileStream(Path.Combine(folder, name + ext), FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        private void Read(Stream stream, Func<string, Stream> resourceFetcher)
        {
            var xd = XDocument.Load(stream);
            var lang = xd.Root;
            if (lang == null) return;

            var name = lang.Attribute("name");
            if (name != null)
            {
                _name = name.Value;
            }

            var parent = lang.Attribute("parent");
            if (parent != null && !String.IsNullOrWhiteSpace(parent.Value))
            {
                _parent = new TranslationStringCollection(parent.Value, resourceFetcher);
            }

            ReadStrings(lang, "");
            ReadGroups(lang, "");
        }

        private void ReadStrings(XContainer node, string prefix)
        {
            foreach (var str in node.Elements("string"))
            {
                var key = str.Attribute("key");
                var val = str.Attribute("value");
                if (key != null && val != null && !String.IsNullOrWhiteSpace(key.Value) && !String.IsNullOrWhiteSpace(val.Value))
                {
                    Add(prefix + key.Value, val.Value);
                }
            }
        }

        private void ReadGroups(XContainer node, string prefix)
        {
            foreach (var gr in node.Elements("group"))
            {
                var name = gr.Attribute("name");
                if (name == null || String.IsNullOrWhiteSpace(name.Value)) continue;
                ReadStrings(gr, prefix + name.Value + "/");
            }
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common.Transport;

namespace Sledge.BspEditor.Primitives.MapData
{
    public class FaceSelection : IMapData, IEnumerable<Face>
    {
        private readonly Dictionary<IMapObject, HashSet<Face>> _selectedObjects;

        public FaceSelection()
        {
            _selectedObjects = new Dictionary<IMapObject, HashSet<Face>>();
        }

        public FaceSelection(SerialisedObject obj)
        {

        }

        [Export(typeof(IMapElementFormatter))]
        public class FaceSelectionFormatter : StandardMapElementFormatter<FaceSelection> { }

        public bool IsEmpty => _selectedObjects.Any(x => x.Value.Count > 0);
        public int Count => _selectedObjects.Aggregate(0, (a, b) => a + b.Value.Count);
        
        public void Add(IMapObject parent, params Face[] faces)
        {
            if (faces.Length == 0) return;
            if (!_selectedObjects.ContainsKey(parent))
            {
                _selectedObjects.Add(parent, new HashSet<Face>());
            }
            _selectedObjects[parent].UnionWith(faces);
        }

        public void Remove(IMapObject parent, params Face[] faces)
        {
            if (!_selectedObjects.ContainsKey(parent)) return;
            _selectedObjects[parent].ExceptWith(faces);
            if (_selectedObjects[parent].Count == 0)
            {
                _selectedObjects.Remove(parent);
            }
        }
        public void Clear()
        {
            _selectedObjects.Clear();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Nothing.
        }

        public IMapElement Clone()
        {
            var c = new FaceSelection();
            foreach (var kv in _selectedObjects)
            {
                c._selectedObjects.Add(kv.Key, new HashSet<Face>(kv.Value));
            }
            return c;
        }

        public IMapElement Copy(UniqueNumberGenerator numberGenerator)
        {
            return Clone();
        }

        public SerialisedObject ToSerialisedObject()
        {
            var so = new SerialisedObject("FaceSelection");
            var strs = new List<string>();
            foreach (var kv in _selectedObjects)
            {
                foreach (var x in kv.Value)
                {
                    strs.Add(Convert.ToString(kv.Key.ID, CultureInfo.InvariantCulture) + ":" + Convert.ToString(x.ID, CultureInfo.InvariantCulture));
                }
            }
            so.Set("SelectedFaces", String.Join(",", strs));
            return so;
        }

        public IEnumerator<Face> GetEnumerator()
        {
            return _selectedObjects.SelectMany(x => x.Value).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerable<KeyValuePair<IMapObject, Face>> GetSelectedFaces()
        {
            return _selectedObjects.SelectMany(x => x.Value.Select(v => new KeyValuePair<IMapObject, Face>(x.Key, v)));
        }

        public IEnumerable<IMapObject> GetSelectedParents()
        {
            return _selectedObjects.Keys;
        }

        public bool IsSelected(IMapObject parent, Face face)
        {
            return _selectedObjects.ContainsKey(parent) && _selectedObjects[parent].Contains(face);
        }

        public bool IsSelected(Face face)
        {
            return _selectedObjects.Any(x => x.Value.Contains(face));
        }
    }
}

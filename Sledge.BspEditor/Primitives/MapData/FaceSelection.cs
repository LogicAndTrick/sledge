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
        public bool AffectsRendering => false;

        private readonly Dictionary<IMapObject, HashSet<long>> _selectedFaces;

        public FaceSelection()
        {
            _selectedFaces = new Dictionary<IMapObject, HashSet<long>>();
        }

        public FaceSelection(SerialisedObject obj)
        {
            _selectedFaces = new Dictionary<IMapObject, HashSet<long>>();
        }

        [Export(typeof(IMapElementFormatter))]
        public class FaceSelectionFormatter : StandardMapElementFormatter<FaceSelection> { }

        public bool IsEmpty => !_selectedFaces.Any(x => x.Value.Count > 0);
        public int Count => _selectedFaces.Aggregate(0, (a, b) => a + b.Value.Count);
        
        public void Add(IMapObject parent, params Face[] faces)
        {
            if (faces.Length == 0) return;
            if (!_selectedFaces.ContainsKey(parent))
            {
                _selectedFaces.Add(parent, new HashSet<long>());
            }
            _selectedFaces[parent].UnionWith(faces.Select(x => x.ID));
        }

        public void Remove(IMapObject parent, params Face[] faces)
        {
            if (!_selectedFaces.ContainsKey(parent)) return;
            _selectedFaces[parent].ExceptWith(faces.Select(x => x.ID));
            if (_selectedFaces[parent].Count == 0)
            {
                _selectedFaces.Remove(parent);
            }
        }
        public void Clear()
        {
            _selectedFaces.Clear();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Nothing.
        }

        public IMapElement Clone()
        {
            var c = new FaceSelection();
            foreach (var kv in _selectedFaces)
            {
                c._selectedFaces.Add(kv.Key, new HashSet<long>(kv.Value));
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
            foreach (var kv in _selectedFaces)
            {
                foreach (var x in kv.Value)
                {
                    strs.Add(Convert.ToString(kv.Key.ID, CultureInfo.InvariantCulture) + ":" + Convert.ToString(x, CultureInfo.InvariantCulture));
                }
            }
            so.Set("SelectedFaces", String.Join(",", strs));
            return so;
        }

        public IEnumerable<KeyValuePair<IMapObject, Face>> GetSelectedFaces()
        {
            return from kv in _selectedFaces
                from f in kv.Value
                let face = kv.Key.Data.OfType<Face>().FirstOrDefault(x => x.ID == f)
                where face != null
                select new KeyValuePair<IMapObject, Face>(kv.Key, face);
        }

        public IEnumerable<IMapObject> GetSelectedParents()
        {
            return _selectedFaces.Keys;
        }

        public bool IsSelected(IMapObject parent, Face face)
        {
            return _selectedFaces.ContainsKey(parent) && _selectedFaces[parent].Contains(face.ID);
        }

        public bool IsSelected(Face face)
        {
            return _selectedFaces.Any(x => x.Value.Contains(face.ID));
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<Face> GetEnumerator()
        {
            return GetSelectedFaces().Select(x => x.Value).GetEnumerator();
        }
    }
}

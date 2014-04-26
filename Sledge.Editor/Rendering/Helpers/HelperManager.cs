using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Documents;
using Sledge.UI;

namespace Sledge.Editor.Rendering.Helpers
{
    /// <summary>
    /// Helpers are objects that either replace or augment a map object. A few of them operate on a document instead of a map object.
    /// Examples of a replacement: Model replacing the bounding box of a prop entity, sprite rendering for an entity instead of a box
    /// Examples of an augmentation: A line connecting a trigger and a target entity, wireframe sphere showing ambient entity radius
    /// Examples of a document helper: Rendering a pointfile, rendering cordon bounds
    /// </summary>
    public class HelperManager
    {
        private readonly Document _document;
        private readonly Dictionary<IHelper, List<MapObject>> _helperCache;
        private List<IHelper> _helpers; 

        public HelperManager(Document document)
        {
            _document = document;
            _helperCache = new Dictionary<IHelper, List<MapObject>>();
            ResetHelpers();
        }

        public void AddHelper(IHelper helper)
        {
            _helpers.Add(helper);
            UpdateCache();
        }

        public void ClearHelpers()
        {
            _helpers.Clear();
            UpdateCache();
        }

        public void ResetHelpers()
        {
            _helpers = typeof(IHelper).Assembly.GetTypes()
                .Where(x => typeof(IHelper).IsAssignableFrom(x))
                .Where(x => !x.IsInterface)
                .Select(Activator.CreateInstance)
                .OfType<IHelper>()
                .ToList();
            _helpers.ForEach(x => x.Document = _document);
            UpdateCache();
        }

        public void ClearCache()
        {
            _helperCache.Clear();
        }

        public void UpdateCache()
        {
            ReIndex();
        }

        private IEnumerable<MapObject> GetAllVisible(MapObject root)
        {
            var list = new List<MapObject>();
            FindRecursive(list, root, x => !x.IsVisgroupHidden);
            return list.Where(x => !x.IsCodeHidden).ToList();
        }

        private void FindRecursive(ICollection<MapObject> items, MapObject root, Predicate<MapObject> matcher)
        {
            if (!matcher(root)) return;
            items.Add(root);
            foreach (var mo in root.GetChildren())
            {
                FindRecursive(items, mo, matcher);
            }
        }

        private void ReIndex()
        {
            ClearCache();
            var all = GetAllVisible(_document.Map.WorldSpawn);
            var helpers = _helpers.Where(x => x.Is2DHelper || x.Is3DHelper).ToList();
            foreach (var mo in all)
            {
                var obj = mo;
                var hide2 = false;
                var hide3 = false;
                foreach (var helper in helpers.Where(x => x.IsValidFor(obj)))
                {
                    if (helper.HelperType == HelperType.Replace)
                    {
                        hide2 = helper.Is2DHelper;
                        hide3 = helper.Is3DHelper;
                    }
                    if (!_helperCache.ContainsKey(helper)) _helperCache.Add(helper, new List<MapObject>());
                    _helperCache[helper].Add(obj);
                }
                obj.IsRenderHidden2D = hide2;
                obj.IsRenderHidden3D = hide3;
            }
        }

        public void Render(ViewportBase viewport)
        {
            var vp2 = viewport as Viewport2D;
            var vp3 = viewport as Viewport3D;
            foreach (var helper in _helpers)
            {
                // Render document
                if (helper.IsDocumentHelper)
                {
                    helper.RenderDocument(viewport, _document);
                }
                // Render 2D
                if (helper.Is2DHelper && vp2 != null && _helperCache.ContainsKey(helper))
                {
                    helper.BeforeRender2D(vp2);
                    foreach (var obj in helper.Order(vp2, _helperCache[helper]))
                    {
                        helper.Render2D(vp2, obj);
                    }
                    helper.AfterRender2D(vp2);
                }
                // Render 3D
                if (helper.Is3DHelper && vp3 != null && _helperCache.ContainsKey(helper))
                {
                    helper.BeforeRender3D(vp3);
                    foreach (var obj in helper.Order(vp3, _helperCache[helper]))
                    {
                        helper.Render3D(vp3, obj);
                    }
                    helper.AfterRender3D(vp3);
                }
            }
        }
    }
}

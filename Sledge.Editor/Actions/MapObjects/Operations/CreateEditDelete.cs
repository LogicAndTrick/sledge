using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sledge.Common.Mediator;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Documents;

namespace Sledge.Editor.Actions.MapObjects.Operations
{
    public class CreateEditDelete : IAction
    {
        protected class DeleteReference
        {
            public long ParentID { get; set; }
            public bool IsSelected { get; set; }
            public MapObject Object { get; set; }
            public bool TopMost { get; set; }

            public DeleteReference(MapObject o, long parentID, bool isSelected, bool topMost)
            {
                Object = o;
                ParentID = parentID;
                IsSelected = isSelected;
                TopMost = topMost;
            }
        }

        protected class EditReference
        {
            public long ID { get; set; }
            public MapObject Before { get; set; }
            public MapObject After { get; set; }
            public Action<Document, MapObject> Action { get; set; }

            public EditReference(long id, MapObject before, MapObject after)
            {
                ID = id;
                Before = before.Clone();
                After = after.Clone();
                Action = null;
            }

            public EditReference(MapObject obj, Action<Document, MapObject> action)
            {
                ID = obj.ID;
                Before = obj.Clone();
                After = null;
                Action = action;
            }

            public void Perform(Document document)
            {
                var root = document.Map.WorldSpawn;
                var obj = root.FindByID(ID);
                if (obj == null) return;

                // Unclone will reset children, need to reselect them if needed
                var deselect = obj.FindAll().Where(x => x.IsSelected).ToList();
                document.Selection.Deselect(deselect);

                if (Action != null) Action(document, obj);
                else obj.Unclone(After);

                var select = obj.FindAll().Where(x => deselect.Any(y => x.ID == y.ID));
                document.Selection.Select(select);

                document.Map.UpdateAutoVisgroups(obj, true);
            }

            public void Reverse(Document document)
            {
                var root = document.Map.WorldSpawn;
                var obj = root.FindByID(ID);
                if (obj == null) return;

                // Unclone will reset children, need to reselect them if needed
                var deselect = obj.FindAll().Where(x => x.IsSelected).ToList();
                document.Selection.Deselect(deselect);

                obj.Unclone(Before);

                var select = obj.FindAll().Where(x => deselect.Any(y => x.ID == y.ID));
                document.Selection.Select(select);

                document.Map.UpdateAutoVisgroups(obj, true);
            }
        }

        private List<long> _createdIds;
        private List<MapObject> _objectsToCreate;

        private List<long> _idsToDelete;
        private List<DeleteReference> _deletedObjects;

        private List<EditReference> _editObjects;

        public CreateEditDelete()
        {
            _objectsToCreate = new List<MapObject>();
            _idsToDelete = new List<long>();
            _editObjects = new List<EditReference>();
        }

        public void Create(params MapObject[] objects)
        {
            _objectsToCreate.AddRange(objects);
        }

        public void Create(IEnumerable<MapObject> objects)
        {
            _objectsToCreate.AddRange(objects);
        }

        public void Delete(params long[] ids)
        {
            _idsToDelete.AddRange(ids);
        }

        public void Delete(IEnumerable<long> ids)
        {
            _idsToDelete.AddRange(ids);
        }

        public void Edit(MapObject before, MapObject after)
        {
            _editObjects.Add(new EditReference(before.ID, before, after));
        }

        public void Edit(IEnumerable<MapObject> before, IEnumerable<MapObject> after)
        {
            var b = before.ToList();
            var a = after.ToList();
            var ids = b.Select(x => x.ID).Where(x => a.Any(y => x == y.ID));
            _editObjects.AddRange(ids.Select(x => new EditReference(x, b.First(y => y.ID == x), a.First(y => y.ID == x))));
        }

        public void Edit(MapObject before, Action<Document, MapObject> action)
        {
            _editObjects.Add(new EditReference(before, action));
        }

        public void Edit(IEnumerable<MapObject> objects, Action<Document, MapObject> action)
        {
            _editObjects.AddRange(objects.Select(x => new EditReference(x, action)));
        }

        public virtual void Dispose()
        {
            _createdIds = null;
            _objectsToCreate = null;

            _idsToDelete = null;
            _deletedObjects = null;

            _editObjects = null;
        }

        public virtual void Reverse(Document document)
        {
            // Edit
            _editObjects.ForEach(x => x.Reverse(document));

            // Create
            _objectsToCreate = document.Map.WorldSpawn.Find(x => _createdIds.Contains(x.ID));
            if (_objectsToCreate.Any(x => x.IsSelected))
            {
                document.Selection.Deselect(_objectsToCreate.Where(x => x.IsSelected));
            }
            _objectsToCreate.ForEach(x => x.SetParent(null));
            _createdIds = null;

            // Delete
            _idsToDelete = _deletedObjects.Select(x => x.Object.ID).ToList();
            foreach (var dr in _deletedObjects.Where(x => x.TopMost))
            {
                dr.Object.SetParent(document.Map.WorldSpawn.FindByID(dr.ParentID));
                document.Map.UpdateAutoVisgroups(dr.Object, true);
            }
            document.Selection.Select(_deletedObjects.Where(x => x.IsSelected).Select(x => x.Object));
            _deletedObjects = null;

            if (_objectsToCreate.Any() || _idsToDelete.Any())
            {
                Mediator.Publish(EditorMediator.DocumentTreeStructureChanged);
            }
            else if (_editObjects.Any())
            {
                Mediator.Publish(EditorMediator.DocumentTreeStructureChanged, _editObjects.Select(x => document.Map.WorldSpawn.FindByID(x.ID)));
            }

            Mediator.Publish(EditorMediator.VisgroupsChanged);
        }

        public virtual void Perform(Document document)
        {
            // Create
            _createdIds = _objectsToCreate.Select(x => x.ID).ToList();
            _objectsToCreate.ForEach(x => x.SetParent(document.Map.WorldSpawn));

            // Select objects if IsSelected is true
            var sel = _objectsToCreate.Where(x => x.IsSelected).ToList();
            sel.RemoveAll(x => x.BoundingBox == null); // Don't select objects with no bbox
            if (sel.Any()) document.Selection.Select(sel);

            document.Map.UpdateAutoVisgroups(_objectsToCreate, true);
            _objectsToCreate = null;

            // Delete
            var objects = document.Map.WorldSpawn.Find(x => _idsToDelete.Contains(x.ID) && x.Parent != null);

            // Recursively check for parent groups that will be empty after these objects have been deleted
            IList<MapObject> emptyParents;
            do
            {
                // Exclude world objects, but we want to remove Group and (brush) Entity objects as they are invalid if empty.
                emptyParents = objects.Where(x => x.Parent != null && !(x.Parent is World) && x.Parent.Children.All(objects.Contains)).ToList();
                foreach (var ep in emptyParents)
                {
                    // Swap the child object for its parent
                    objects.Remove(ep);
                    if (!objects.Contains(ep.Parent)) objects.Add(ep.Parent);
                }
            } while (emptyParents.Any()); // If we changed the collection, we need to re-check

            _deletedObjects = objects.Select(x => new DeleteReference(x, x.Parent.ID, x.IsSelected, !objects.Contains(x.Parent))).ToList();
            document.Selection.Deselect(objects);
            foreach (var dr in _deletedObjects.Where(x => x.TopMost))
            {
                dr.Object.SetParent(null);
            }
            _idsToDelete = null;

            // Edit
            _editObjects.ForEach(x => x.Perform(document));

            if (_createdIds.Any() || _deletedObjects.Any())
            {
                Mediator.Publish(EditorMediator.DocumentTreeStructureChanged);
            }
            else if (_editObjects.Any())
            {
                Mediator.Publish(EditorMediator.DocumentTreeStructureChanged, _editObjects.Select(x => document.Map.WorldSpawn.FindByID(x.ID)));
            }

            Mediator.Publish(EditorMediator.VisgroupsChanged);
        }
    }
}
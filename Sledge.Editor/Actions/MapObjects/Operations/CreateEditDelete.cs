using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sledge.Common.Mediator;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Actions.MapObjects.Operations.EditOperations;
using Sledge.Editor.Documents;

namespace Sledge.Editor.Actions.MapObjects.Operations
{
    public class CreateEditDelete : IAction
    {
        private class CreateReference
        {
            public long ParentID { get; set; }
            public bool IsSelected { get; set; }
            public MapObject MapObject { get; set; }

            public CreateReference(long parentID, MapObject mapObject)
            {
                ParentID = parentID;
                IsSelected = mapObject.IsSelected;
                MapObject = mapObject;
            }
        }

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
            public IEditOperation EditOperation { get; set; }

            public EditReference(MapObject obj, IEditOperation editOperation)
            {
                ID = obj.ID;
                Before = obj.Clone();
                EditOperation = editOperation;
            }

            public void Perform(Document document)
            {
                var root = document.Map.WorldSpawn;
                var obj = root.FindByID(ID);
                if (obj == null) return;

                // Unclone will reset children, need to reselect them if needed
                var deselect = obj.FindAll().Where(x => x.IsSelected).ToList();
                document.Selection.Deselect(deselect);

                EditOperation.PerformOperation(obj);

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

        public bool SkipInStack { get { return false; } }
        public bool ModifiesState { get { return true; } }

        private List<long> _createdIds;
        private List<CreateReference> _objectsToCreate;

        private List<long> _idsToDelete;
        private List<DeleteReference> _deletedObjects;

        private List<EditReference> _editObjects;

        public CreateEditDelete()
        {
            _objectsToCreate = new List<CreateReference>();
            _idsToDelete = new List<long>();
            _editObjects = new List<EditReference>();
        }

        public void Create(long parentId, params MapObject[] objects)
        {
            _objectsToCreate.AddRange(objects.Select(x => new CreateReference(parentId, x)));
        }

        public void Create(long parentId, IEnumerable<MapObject> objects)
        {
            _objectsToCreate.AddRange(objects.Select(x => new CreateReference(parentId, x)));
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
            _editObjects.Add(new EditReference(before, new CopyPropertiesEditOperation(after)));
        }

        public void Edit(IEnumerable<MapObject> before, IEnumerable<MapObject> after)
        {
            var b = before.ToList();
            var a = after.ToList();
            var ids = b.Select(x => x.ID).Where(x => a.Any(y => x == y.ID));
            _editObjects.AddRange(ids.Select(x => new EditReference(b.First(y => y.ID == x), new CopyPropertiesEditOperation(a.First(y => y.ID == x)))));
        }

        public void Edit(MapObject before, IEditOperation editOperation)
        {
            _editObjects.Add(new EditReference(before, editOperation));
        }

        public void Edit(IEnumerable<MapObject> objects, IEditOperation editOperation)
        {
            _editObjects.AddRange(objects.Select(x => new EditReference(x, editOperation)));
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
            _objectsToCreate = document.Map.WorldSpawn.Find(x => _createdIds.Contains(x.ID)).Select(x => new CreateReference(x.Parent.ID, x)).ToList();
            if (_objectsToCreate.Any(x => x.MapObject.IsSelected))
            {
                document.Selection.Deselect(_objectsToCreate.Where(x => x.MapObject.IsSelected).Select(x => x.MapObject));
            }
            _objectsToCreate.ForEach(x => x.MapObject.SetParent(null));
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

            Mediator.Publish(EditorMediator.SelectionChanged);
            Mediator.Publish(EditorMediator.VisgroupsChanged);
        }

        public virtual void Perform(Document document)
        {
            // Create
            _createdIds = _objectsToCreate.Select(x => x.MapObject.ID).ToList();
            _objectsToCreate.ForEach(x => x.MapObject.SetParent(document.Map.WorldSpawn.FindByID(x.ParentID)));

            // Select objects if IsSelected is true
            var sel = _objectsToCreate.Where(x => x.IsSelected).ToList();
            sel.RemoveAll(x => x.MapObject.BoundingBox == null); // Don't select objects with no bbox
            if (sel.Any()) document.Selection.Select(sel.Select(x => x.MapObject));

            document.Map.UpdateAutoVisgroups(_objectsToCreate.Select(x => x.MapObject.Parent is World ? x.MapObject : x.MapObject.Parent), true);
            _objectsToCreate = null;

            // Delete
            var objects = document.Map.WorldSpawn.Find(x => _idsToDelete.Contains(x.ID) && x.Parent != null).SelectMany(x => x.FindAll()).ToList();

            // Recursively check for parent groups that will be empty after these objects have been deleted
            IList<MapObject> emptyParents;
            do
            {
                // Exclude world objects, but we want to remove Group and (brush) Entity objects as they are invalid if empty.
                emptyParents = objects.Where(x => x.Parent != null && !(x.Parent is World) && x.Parent.GetChildren().All(objects.Contains) && !objects.Contains(x.Parent)).ToList();
                foreach (var ep in emptyParents)
                {
                    // Add the parent object into the delete list
                    objects.Add(ep.Parent);
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

            Mediator.Publish(EditorMediator.SelectionChanged);
            Mediator.Publish(EditorMediator.VisgroupsChanged);
        }
    }
}
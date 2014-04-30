using System;
using System.Collections.Generic;
using System.Linq;
using Sledge.Common.Mediator;
using Sledge.DataStructures.GameData;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Documents;

namespace Sledge.Editor.Actions.MapObjects.Entities
{
    public class EditEntityData : IAction
    {
        private class EntityReference
        {
            public long ID { get; set; }
            public EntityData Before { get; set; }
            public EntityData After { get; set; }
        }

        public bool SkipInStack { get { return false; } }
        public bool ModifiesState { get { return true; } }

        private List<EntityReference> _objects;

        public EditEntityData()
        {
            _objects = new List<EntityReference>();
        }

        public void AddEntity(MapObject obj, EntityData newData)
        {
            _objects.Add(new EntityReference {ID = obj.ID, Before = obj.GetEntityData().Clone(), After = newData});
        }

        public bool IsEmpty()
        {
            return _objects.Count == 0;
        }

        public void Dispose()
        {
            _objects = null;
        }

        public void Reverse(Document document)
        {
            var changed = new List<MapObject>();
            foreach (var r in _objects)
            {
                var obj = document.Map.WorldSpawn.FindByID(r.ID);
                changed.Add(obj);
                if (obj is Entity) SetEntityData((Entity)obj, r.Before, document.GameData);
                else if (obj is World) SetEntityData((World)obj, r.Before);
            }
            Mediator.Publish(EditorMediator.EntityDataChanged, changed);
            Mediator.Publish(EditorMediator.DocumentTreeObjectsChanged, changed);

            document.Map.UpdateAutoVisgroups(changed, true);
            Mediator.Publish(EditorMediator.VisgroupsChanged);
        }

        public void Perform(Document document)
        {
            var changed = new List<MapObject>();
            foreach (var r in _objects)
            {
                var obj = document.Map.WorldSpawn.FindByID(r.ID);
                changed.Add(obj);
                if (obj is Entity) SetEntityData((Entity) obj, r.After, document.GameData);
                else if (obj is World) SetEntityData((World) obj, r.After);

                if (obj != null) obj.UpdateBoundingBox();
            }
            Mediator.Publish(EditorMediator.EntityDataChanged, changed);
            Mediator.Publish(EditorMediator.DocumentTreeObjectsChanged, changed);

            document.Map.UpdateAutoVisgroups(changed, true);
            Mediator.Publish(EditorMediator.VisgroupsChanged);
        }

        private void SetEntityData(Entity ent, EntityData data, GameData gameData)
        {
            ent.EntityData = data;
            ent.GameData = gameData.Classes.FirstOrDefault(x => String.Equals(x.Name, data.Name, StringComparison.CurrentCultureIgnoreCase) && x.ClassType != ClassType.Base);
        }

        private void SetEntityData(World world, EntityData data)
        {
            world.EntityData = data;
        }
    }
}
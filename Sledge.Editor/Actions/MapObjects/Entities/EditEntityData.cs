using System.Collections.Generic;
using System.Linq;
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

        private List<EntityReference> _objects;

        public EditEntityData()
        {
            _objects = new List<EntityReference>();
        }

        public void AddEntity(MapObject obj, EntityData newData)
        {
            _objects.Add(new EntityReference {ID = obj.ID, Before = obj.GetEntityData().Clone(), After = newData});
        }

        public void Dispose()
        {
            _objects = null;
        }

        public void Reverse(Document document)
        {
            foreach (var r in _objects)
            {
                var obj = document.Map.WorldSpawn.FindByID(r.ID);
                if (obj is Entity) SetEntityData((Entity)obj, r.Before, document.GameData);
                else if (obj is World) SetEntityData((World)obj, r.Before);
            }
        }

        public void Perform(Document document)
        {
            foreach (var r in _objects)
            {
                var obj = document.Map.WorldSpawn.FindByID(r.ID);
                if (obj is Entity) SetEntityData((Entity) obj, r.After, document.GameData);
                else if (obj is World) SetEntityData((World) obj, r.After);
            }
        }

        private void SetEntityData(Entity ent, EntityData data, GameData gameData)
        {
            ent.EntityData = data;
            ent.GameData = gameData.Classes.FirstOrDefault(x => x.Name.ToLower() == data.Name.ToLower());
        }

        private void SetEntityData(World world, EntityData data)
        {
            world.EntityData = data;
        }
    }
}
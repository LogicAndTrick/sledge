using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Commands;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Editing.Properties;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Modification.Operations.Selection;
using Sledge.BspEditor.Modification.Operations.Tree;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common;
using Sledge.Common.Shell.Commands;
using Sledge.Common.Shell.Context;
using Sledge.Common.Shell.Hotkeys;
using Sledge.Common.Shell.Menu;
using Sledge.Common.Translations;
using Sledge.DataStructures.GameData;
using Sledge.QuickForms;
using Sledge.QuickForms.Items;

namespace Sledge.BspEditor.Editing.Commands
{
    [AutoTranslate]
    [Export(typeof(ICommand))]
    [MenuItem("Tools", "", "Entity", "D")]
    [CommandID("BspEditor:Tools:TieToEntity")]
    [MenuImage(typeof(Resources), nameof(Resources.Menu_TieToEntity))]
    [DefaultHotkey("Ctrl+T")]
    public class TieToEntity : BaseCommand
    {
        public override string Name { get; set; } = "Tie to entity";
        public override string Details { get; set; } = "Create an entity out of the selection.";
        
        public string EntitySelectedTitle { get; set; }
        public string OneEntitySelectedMessage { get; set; }
        public string MultipleEntitiesSelectedMessage { get; set; }
        public string KeepExisting { get; set; }
        public string CreateNew { get; set; }
        public string OK { get; set; }
        public string Cancel { get; set; }

        protected override bool IsInContext(IContext context, MapDocument document)
        {
            return base.IsInContext(context, document) && !document.Selection.IsEmpty;
        }

        protected override async Task Invoke(MapDocument document, CommandParameters parameters)
        {
            var existingEntities = document.Selection.OfType<Entity>().ToList();

            Entity existing = null;
            var confirmed = false;
            if (existingEntities.Count == 0)
            {
                // No entities selected, just create it straight up
                confirmed = true;
            }
            else if (existingEntities.Count == 1)
            {
                // One entity selected, user chooses to merge or create a new entity
                using (
                    var qf = new QuickForm(EntitySelectedTitle) {Width = 400}
                        .Label(String.Format(OneEntitySelectedMessage, existingEntities[0].EntityData?.Name))
                        .DialogButtons(
                            (KeepExisting, DialogResult.Yes),
                            (CreateNew, DialogResult.No),
                            (Cancel, DialogResult.Cancel)
                        )
                )
                {
                    var result = await qf.ShowDialogAsync();
                    if (result == DialogResult.Yes) existing = existingEntities[0];
                    confirmed = result != DialogResult.Cancel;
                }
            }
            else if (existingEntities.Count > 1)
            {
                // Multiple entities selected, user chooses which one to keep
                using (
                    var qf = new QuickForm(EntitySelectedTitle) { Width = 400 }
                        .Label(MultipleEntitiesSelectedMessage)
                        .ComboBox("Entity", "", existingEntities.Select(x => new EntityContainer { Entity = x }))
                        .OkCancel(OK, Cancel)
                )
                {
                    var result = await qf.ShowDialogAsync();
                    if (result == DialogResult.OK) existing = (qf.Object("Entity") as EntityContainer)?.Entity;
                    confirmed = result != DialogResult.Cancel;
                }
            }

            if (!confirmed) return;

            var ops = new List<IOperation>();

            var gameData = await document.Environment.GetGameData();
            var def = document.Environment.DefaultBrushEntity;
            var defaultEntityClass = (
                from g in gameData.Classes
                where g.ClassType == ClassType.Solid
                orderby String.Equals(g.Name, def, StringComparison.InvariantCultureIgnoreCase) ? 0 : 1,
                        String.Equals(g.Name, "trigger_once", StringComparison.InvariantCultureIgnoreCase) ? 0 : 1,
                        g.Description.ToLower()
                select g
            ).FirstOrDefault() ?? new GameDataObject("trigger_once", "", ClassType.Solid);

            if (existing == null)
            {
                // If the entity doesn't exist we need to create it
                existing = new Entity(document.Map.NumberGenerator.Next("MapObject"))
                {
                    Data =
                    {
                        new EntityData { Name = defaultEntityClass.Name },
                        new ObjectColor(Colour.GetDefaultEntityColour())
                    }
                };
                ops.Add(new Attach(document.Map.Root.ID, existing));
            }
            else
            {
                // If the entity is a descendant of the selection, it would cause havok
                ops.Add(new Detatch(existing.Hierarchy.Parent.ID, existing));
                ops.Add(new Attach(document.Map.Root.ID, existing));
            }

            // Any other entities in the selection should be destroyed
            foreach (var entity in existingEntities.Where(x => !ReferenceEquals(x, existing)))
            {
                var children = entity.Hierarchy.Where(x => !(x is Entity)).ToList();
                ops.Add(new Detatch(entity.ID, children));
                ops.Add(new Attach(existing.ID, children));
                ops.Add(new Detatch(entity.Hierarchy.Parent.ID, entity));
            }

            // All other parents should be added to the entity
            foreach (var obj in document.Selection.GetSelectedParents().Except(existingEntities))
            {
                ops.Add(new Detatch(obj.Hierarchy.Parent.ID, obj));
                ops.Add(new Attach(existing.ID, obj));
            }

            if (ops.Any())
            {
                ops.Add(new Select(document.Selection.Except(existingEntities).Union(new[] { existing })));
                await MapDocumentOperation.Perform(document, new Transaction(ops));
                await Oy.Publish("Context:Add", new ContextInfo("BspEditor:ObjectProperties"));
            }
        }

        private class EntityContainer
        {
            public Entity Entity { get; set; }
            public override string ToString()
            {
                if (Entity.EntityData == null) return Convert.ToString(Entity.ID);
                return Entity.EntityData.Properties.ContainsKey("targetname")
                    ? Entity.EntityData.Properties["targetname"] + " (" + Entity.EntityData.Name + ")"
                    : Entity.EntityData.Name;
            }
        }
    }
}
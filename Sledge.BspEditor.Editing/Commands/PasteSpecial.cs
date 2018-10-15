using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sledge.BspEditor.Commands;
using Sledge.BspEditor.Components;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Editing.Components;
using Sledge.BspEditor.Editing.Properties;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Modification.Operations.Tree;
using Sledge.BspEditor.Primitives;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common.Shell.Commands;
using Sledge.Common.Shell.Hotkeys;
using Sledge.Common.Shell.Menu;
using Sledge.Common.Translations;
using Sledge.DataStructures.Geometric;

namespace Sledge.BspEditor.Editing.Commands
{
    [AutoTranslate]
    [Export(typeof(ICommand))]
    [MenuItem("Edit", "", "Clipboard", "H")]
    [CommandID("BspEditor:Edit:PasteSpecial")]
    [MenuImage(typeof(Resources), nameof(Resources.Menu_PasteSpecial))]
    [DefaultHotkey("Ctrl+Shift+V")]
    public class PasteSpecial : BaseCommand
    {
        private readonly Lazy<ClipboardManager> _clipboard;
        private readonly Lazy<ITranslationStringProvider> _translator;

        public override string Name { get; set; } = "Paste Special...";
        public override string Details { get; set; } = "Paste multiple copies";

        [ImportingConstructor]
        public PasteSpecial(
            [Import] Lazy<ClipboardManager> clipboard,
            [Import] Lazy<ITranslationStringProvider> translator
        )
        {
            _clipboard = clipboard;
            _translator = translator;
        }

        protected override async Task Invoke(MapDocument document, CommandParameters parameters)
        {
            if (_clipboard.Value.CanPaste())
            {
                var content = _clipboard.Value.GetPastedContent(document).ToList();
                if (!content.Any()) return;

                using (var psd = new PasteSpecialDialog(new Box(content.Select(x => x.BoundingBox))))
                {
                    _translator.Value.Translate(psd);
                    if (psd.ShowDialog() == DialogResult.OK)
                    {
                        var objs = GetPastedContent(document, content, psd);
                        if (objs.Any())
                        {
                            var op = new Attach(document.Map.Root.ID, objs);
                            await MapDocumentOperation.Perform(document, op);
                        }
                    }
                }
            }
        }

        private List<IMapObject> GetPastedContent(MapDocument document, List<IMapObject> objectsToPaste, PasteSpecialDialog dialog)
        {
            var origin = GetPasteOrigin(document, dialog.StartPoint, objectsToPaste);
            var objects = new List<IMapObject>();
            var grouping = dialog.Grouping;
            var makeEntitesUnique = dialog.MakeEntitiesUnique;
            var numCopies = dialog.NumberOfCopies;
            var offset = dialog.AccumulativeOffset;
            var rotation = dialog.AccumulativeRotation;

            if (objectsToPaste.Count == 1)
            {
                // Only one object - no need to group.
                grouping = PasteSpecialDialog.PasteSpecialGrouping.None;
            }

            Group allGroup = null;
            if (grouping == PasteSpecialDialog.PasteSpecialGrouping.All)
            {
                // Use one group for all copies
                allGroup = new Group(document.Map.NumberGenerator.Next("MapObject"));
                // Add the group to the tree
                objects.Add(allGroup);
            }

            // Get a list of all entity names if needed
            var names = new List<string>();
            if (makeEntitesUnique)
            {
                names = document.Map.Root.Find(x => x is Entity)
                    .Select(x => x.Data.GetOne<EntityData>())
                    .Where(x => x != null && x.Properties.ContainsKey("targetname"))
                    .Select(x => x.Properties["targetname"])
                    .ToList();
            }

            // Start at i = 1 so the original isn't duped with no offets
            for (var i = 1; i <= numCopies; i++)
            {
                var copyOrigin = origin + (offset * i);
                var copyRotation = rotation * i;
                var copy = CreateCopy(document.Map.NumberGenerator, copyOrigin, copyRotation, names, objectsToPaste, makeEntitesUnique, dialog.PrefixEntityNames, dialog.EntityNamePrefix).ToList();
                var grouped = GroupCopy(document.Map.NumberGenerator, allGroup, copy, grouping);
                objects.AddRange(grouped);
            }

            return objects;
        }

        private Vector3 GetPasteOrigin(MapDocument document, PasteSpecialDialog.PasteSpecialStartPoint startPoint, List<IMapObject> objectsToPaste)
        {
            // Find the starting point of the paste
            Vector3 origin;
            switch (startPoint)
            {
                case PasteSpecialDialog.PasteSpecialStartPoint.CenterOriginal:
                    // Use the original origin
                    var box = new Box(objectsToPaste.Select(x => x.BoundingBox));
                    origin = box.Center;
                    break;
                case PasteSpecialDialog.PasteSpecialStartPoint.CenterSelection:
                    // Use the selection origin
                    origin = document.Selection.GetSelectionBoundingBox().Center;
                    break;
                default:
                    // Use the map origin
                    origin = Vector3.Zero;
                    break;
            }
            return origin;
        }

        private IEnumerable<IMapObject> CreateCopy(UniqueNumberGenerator gen, Vector3 origin, Vector3 rotation, List<string> names, List<IMapObject> objectsToPaste, bool makeEntitesUnique, bool prefixEntityNames, string entityNamePrefix)
        {
            var box = new Box(objectsToPaste.Select(x => x.BoundingBox));
            
            var rads = rotation * (float) Math.PI / 180;
            var mov = Matrix4x4.CreateTranslation(-box.Center); // Move to zero
            var rot = Matrix4x4.CreateFromQuaternion(Quaternion.CreateFromYawPitchRoll(rads.Y, rads.X, rads.Z)); // Do rotation
            var fin = Matrix4x4.CreateTranslation(origin); // Move to final origin
            var transform = mov * rot * fin;

            foreach (var mo in objectsToPaste)
            {
                // Copy, transform and fix entity names
                var copy = (IMapObject) mo.Copy(gen);

                // Transform the object
                copy.Transform(transform);

                // Paste special will always texture lock (always uniform too, only translation and rotation possible)
                foreach (var t in copy.Data.OfType<ITextured>()) t.Texture.TransformUniform(transform);

                FixEntityNames(copy, names, makeEntitesUnique, prefixEntityNames, entityNamePrefix);
                yield return copy;
            }
        }

        private void FixEntityNames(IMapObject obj, List<string> names, bool makeEntitesUnique, bool prefixEntityNames, string entityNamePrefix)
        {
            if (!makeEntitesUnique && !prefixEntityNames) return;

            var ents = obj.Find(x => x is Entity).OfType<Entity>().Where(x => x.EntityData != null);
            foreach (var entity in ents)
            {
                // Find the targetname property
                if (!entity.EntityData.Properties.ContainsKey("targetname")) continue;
                var prop = entity.EntityData.Properties["targetname"];

                // Skip unnamed entities
                if (String.IsNullOrWhiteSpace(prop)) continue;

                // Add the prefix before the unique check
                if (prefixEntityNames)
                {
                    prop = entityNamePrefix + prop;
                }

                // Make the name unique
                if (makeEntitesUnique)
                {
                    var name = prop;

                    // Find a unique new name for the entity
                    var newName = name;
                    var counter = 1;
                    while (names.Contains(newName))
                    {
                        newName = name + "_" + counter;
                        counter++;
                    }

                    // Set the new name and add it into the list
                    entity.EntityData.Properties["targetname"] = newName;
                    names.Add(newName);
                }
            }
        }

        private IEnumerable<IMapObject> GroupCopy(UniqueNumberGenerator gen, IMapObject allGroup, List<IMapObject> copy, PasteSpecialDialog.PasteSpecialGrouping grouping)
        {
            switch (grouping)
            {
                case PasteSpecialDialog.PasteSpecialGrouping.None:
                    // No grouping - add directly to tree
                    return copy;
                case PasteSpecialDialog.PasteSpecialGrouping.Individual:
                    // Use one group per copy
                    var group = new Group(gen.Next("MapObject"));
                    copy.ForEach(x => x.Hierarchy.Parent = group);
                    return new List<IMapObject> { group };
                case PasteSpecialDialog.PasteSpecialGrouping.All:
                    // Use one group for all copies
                    copy.ForEach(x => x.Hierarchy.Parent = allGroup);
                    return new IMapObject[0];
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
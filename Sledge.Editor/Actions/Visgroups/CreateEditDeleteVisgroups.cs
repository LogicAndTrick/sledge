using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Sledge.Common.Mediator;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Documents;

namespace Sledge.Editor.Actions.Visgroups
{
    public class CreateEditDeleteVisgroups : IAction
    {
        public bool SkipInStack { get { return false; } }
        public bool ModifiesState { get { return true; } }

        private List<Visgroup> _newVisgroups;
        private List<Tuple<int, string, Color>> _beforeChanges;
        private List<Tuple<int, string, Color>> _afterchanges;
        private List<Visgroup> _deletedVisgroups;
        private Dictionary<int, List<MapObject>> _removedObjects;
        private List<MapObject> _madeVisible;

        public CreateEditDeleteVisgroups(IEnumerable<Visgroup> newVisgroups, IEnumerable<Visgroup> changedVisgroups, IEnumerable<Visgroup> deletedVisgroups)
        {
            _newVisgroups = newVisgroups.ToList();
            _afterchanges = changedVisgroups.Select(x => Tuple.Create(x.ID, x.Name, x.Colour)).ToList();
            _deletedVisgroups = deletedVisgroups.ToList();
        }

        public void Dispose()
        {
            _newVisgroups = _deletedVisgroups = null;
            _beforeChanges = _afterchanges = null;
        }

        public void Reverse(Document document)
        {
            // Deleted
            foreach (var del in _deletedVisgroups)
            {
                document.Map.Visgroups.Add(del);
                _removedObjects[del.ID].ForEach(x => x.Visgroups.Add(del.ID));
            }
            _madeVisible.ForEach(x => x.IsVisgroupHidden = true);
            _removedObjects = null;
            _madeVisible = null;

            // Changed
            _afterchanges = new List<Tuple<int, string, Color>>();
            foreach (var bc in _beforeChanges)
            {
                var vis = document.Map.Visgroups.First(x => x.ID == bc.Item1);
                _afterchanges.Add(Tuple.Create(vis.ID, vis.Name, vis.Colour));
                vis.Name = bc.Item2;
                vis.Colour = bc.Item3;
            }
            _beforeChanges = null;

            // New
            document.Map.Visgroups.RemoveAll(x => _newVisgroups.Any(y => y.ID == x.ID));

            Mediator.Publish(EditorMediator.VisgroupsChanged);
            Mediator.Publish(EditorMediator.DocumentTreeStructureChanged);
        }

        public void Perform(Document document)
        {
            // New
            document.Map.Visgroups.AddRange(_newVisgroups);

            // Changed
            _beforeChanges = new List<Tuple<int, string, Color>>();
            foreach (var ac in _afterchanges)
            {
                var vis = document.Map.Visgroups.First(x => x.ID == ac.Item1);
                _beforeChanges.Add(Tuple.Create(vis.ID, vis.Name, vis.Colour));
                vis.Name = ac.Item2;
                vis.Colour = ac.Item3;
            }
            _afterchanges = null;

            // Deleted
            _madeVisible = new List<MapObject>();
            _removedObjects = new Dictionary<int, List<MapObject>>();
            var all = document.Map.WorldSpawn.Find(x => x.Visgroups.Any());
            foreach (var del in _deletedVisgroups)
            {
                var id = del.ID;
                document.Map.Visgroups.RemoveAll(x => x.ID == id);
                var rem = all.Where(x => x.IsInVisgroup(id, false)).ToList();
                _removedObjects.Add(id, rem);
                foreach (var mo in rem)
                {
                    mo.Visgroups.Remove(id);
                    if (!mo.IsVisgroupHidden || mo.Visgroups.Any()) continue;

                    // Object has no more visgroups but is invisible, we need to show it
                    mo.IsVisgroupHidden = false;
                    _madeVisible.Add(mo);
                }
            }

            Mediator.Publish(EditorMediator.VisgroupsChanged);
            Mediator.Publish(EditorMediator.DocumentTreeStructureChanged);
        }
    }
}
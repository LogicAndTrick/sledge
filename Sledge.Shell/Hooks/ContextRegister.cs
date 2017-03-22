using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Threading.Tasks;
using LogicAndTrick.Gimme;
using LogicAndTrick.Gimme.Providers;
using LogicAndTrick.Oy;
using Sledge.Common.Commands;
using Sledge.Common.Hooks;

namespace Sledge.Shell.Hooks
{
    [Export(typeof(IInitialiseHook))]
    [Export(typeof(IResourceProvider<>))]
    public class ContextRegister : SyncResourceProvider<string>, IInitialiseHook
    {
        public Task OnInitialise(CompositionContainer container)
        {
            // Listen for dynamically added/removed commands
            Oy.Subscribe<string>("Context:Add", c => Add(c));
            Oy.Subscribe<string>("Context:Remove", c => Remove(c));

            // Register the resource provider
            Gimme.Register(this);

            return Task.FromResult(0);
        }

        // Context resource provider
        public override bool CanProvide(string location)
        {
            return location == "meta://context";
        }

        public override IEnumerable<string> Fetch(string location, List<string> resources)
        {
            return _current.ToList();
        }

        private readonly HashSet<string> _current;

        public ContextRegister()
        {
            _current = new HashSet<string>();
        }

        private void Add(string context)
        {
            _current.Add(context);
            Oy.Publish("Context:Added", context);
        }

        private void Remove(string context)
        {
            _current.Remove(context);
            Oy.Publish("Context:Removed", context);
        }
    }
}
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using LogicAndTrick.Oy;
using Sledge.Common.Shell.Context;
using Sledge.Common.Shell.Hooks;

namespace Sledge.Shell.Registers
{
    /// <summary>
    /// The context register controls and dispatches the context.
    /// </summary>
    [Export(typeof(IStartupHook))]
    [Export(typeof(IInitialiseHook))]
    [Export(typeof(IContext))]
    public class ContextRegister : IStartupHook, IInitialiseHook, IContext
    {
        public Task OnStartup()
        {
            // Listen for dynamically added/removed commands
            Oy.Subscribe<ContextInfo>("Context:Add", c => Add(c));
            Oy.Subscribe<ContextInfo>("Context:Remove", c => Remove(c.ID));
            Oy.Subscribe<string>("Context:Remove", c => Remove(c));

            return Task.FromResult(0);
        }

        public async Task OnInitialise()
        {
            // We run this on initialise so anything that listens for context changes
            // to set visibilities/active states will be notified at startup
            await Oy.Publish("Context:Changed", this);
        }

        private readonly ConcurrentDictionary<string, ContextInfo> _context;

        public ContextRegister()
        {
            _context = new ConcurrentDictionary<string, ContextInfo>();

            // Always register the Debug context when debugging
#if DEBUG
            Add(new ContextInfo("Debug"));
#endif
        }

        /// <summary>
        /// Add context. Existing context with the same key will be replaced.
        /// </summary>
        /// <param name="info">The context info to add</param>
        private void Add(ContextInfo info)
        {
            _context[info.ID] = info;
            Oy.Publish("Context:Changed", this);
        }

        /// <summary>
        /// Remove context
        /// </summary>
        /// <param name="context">The context to remove</param>
        private void Remove(string context)
        {
            ContextInfo o;
            _context.TryRemove(context, out o);
            Oy.Publish("Context:Changed", this);
        }
        
        // IContext implementation

        public bool HasAll(params string[] context)
        {
            return context.All(x => _context.ContainsKey(x));
        }

        public bool HasAny(params string[] context)
        {
            return context.Any(x => _context.ContainsKey(x));
        }

        public bool Has<T>(string context, T value)
        {
            ContextInfo info;
            return _context.TryGetValue(context, out info) &&
                    info.Value.IsAlive &&
                    Equals(info.Value.Target, value);
        }

        public T Get<T>(string context, T defaultValue = default(T))
        {
            ContextInfo info;
            return _context.TryGetValue(context, out info) && info.Value != null && info.Value.IsAlive && info.Value.Target is T
                ? (T) info.Value.Target
                : defaultValue;
        }

        public bool TryGet<T>(string context, out T value)
        {
            ContextInfo info;

            if (_context.TryGetValue(context, out info) && info.ValueIs<T>())
            {
                value = (T) info.Value.Target;
                return true;
            }

            value = default(T);
            return false;
        }

        public IEnumerable<string> GetAll()
        {
            return _context.Keys.ToList();
        }
    }
}
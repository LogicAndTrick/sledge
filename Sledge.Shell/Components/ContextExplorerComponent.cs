using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using LogicAndTrick.Oy;
using Sledge.Common.Shell.Components;
using Sledge.Common.Shell.Context;
using Sledge.Common.Translations;

namespace Sledge.Shell.Components
{
#if DEBUG_EXTRA
    [AutoTranslate]
    [Export(typeof(ISidebarComponent))]
    [OrderHint("Y")]
#endif
    public class ContextExplorerComponent : ISidebarComponent
    {
        private readonly ListBox _control;

        public string Title { get; set; } = "Context Explorer";
        public object Control => _control;

        public ContextExplorerComponent()
        {
            _control = new ListBox();
            Oy.Subscribe<IContext>("Context:Changed", ContextChanged);
        }

        private async Task ContextChanged(IContext context)
        {
            if (!context.HasAny("Debug")) return;

            if (!_control.IsHandleCreated) return;

            _control.BeginInvoke((MethodInvoker) delegate
            {
                _control.BeginUpdate();
                _control.Items.Clear();

                foreach (var key in context.GetAll())
                {
                    object p;
                    if (!context.TryGet(key, out p)) p = null;
                    var str = key;
                    if (p != null) str += ": " + GetDisplayName(p);
                    _control.Items.Add(str);
                }

                _control.EndUpdate();
            });
        }

        public bool IsInContext(IContext context)
        {
            return context.HasAny("Debug");
        }

        private string GetDisplayName(object obj)
        {
            if (obj == null) return "";
            var ty = obj.GetType();

            // Try and find an uninherited ToString() method
            var toStringMethod = ty.GetMethod("ToString", BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance, null, CallingConventions.Any, Type.EmptyTypes, null);
            if (toStringMethod != null)
            {
                return obj.ToString();
            }

            // Look for description-sounding text fields
            var prop = ty.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty)
                .Where(x => x.PropertyType == typeof(string))
                .FirstOrDefault(x => x.Name == "Name" || x.Name == "Title" || x.Name == "Description" || x.Name == "Details");
            if (prop != null)
            {
                return (string) prop.GetValue(obj);
            }

            // Give up
            return obj.ToString();
        }
    }
}

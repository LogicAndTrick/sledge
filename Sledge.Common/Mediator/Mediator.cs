using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Sledge.Common.Mediator
{
    /// <summary>
    /// The mediator is a static event/communications manager.
    /// </summary>
    public static class Mediator
    {
        /// <summary>
        /// Helper method to execute the a function with the same name as the message. Called by the listener if desired.
        /// </summary>
        /// <param name="obj">The object to call the method on</param>
        /// <param name="message">The name of the method</param>
        /// <param name="parameter">The parameter. If this is an array, the multi-parameter method will be given priority over the single- and zero-parameter methods</param>
        public static bool ExecuteDefault(object obj, string message, object parameter)
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
            var t = obj.GetType();
            MethodInfo method = null;
            object[] parameters = null;
            if (parameter is object[])
            {
                var arr = (object[]) parameter;
                method = t.GetMethod(message, flags, null, arr.Select(x => x.GetType()).ToArray(), null);
                parameters = arr;
            }
            if (method == null && parameter != null)
            {
                method = t.GetMethod(message, flags, null, new[] { parameter.GetType() }, null);
                parameters = new[] {parameter};
            }
            if (method == null)
            {
                method = t.GetMethod(message, flags);
                parameters = null;
            }
            if (method != null)
            {
                method.Invoke(obj, parameters);
                return true;
            }
            return false;
        }

        private static readonly MultiDictionary<string, WeakReference> Listeners;

        static Mediator()
        {
            Listeners = new MultiDictionary<string, WeakReference>();
        }

        public static void Subscribe(string message, IMediatorListener obj)
        {
            Listeners.AddValue(message, new WeakReference(obj));
        }

        public static void Subscribe(Enum message, IMediatorListener obj)
        {
            Listeners.AddValue(message.ToString(), new WeakReference(obj));
        }

        public static void Unsubscribe(string message, IMediatorListener obj)
        {
            if (!Listeners.ContainsKey(message)) return;
            var l = Listeners[message];
            l.RemoveAll(x => !x.IsAlive || x.Target == null || x.Target == obj);
        }

        public static void UnsubscribeAll(IMediatorListener obj)
        {
            foreach (var listener in Listeners.Values)
            {
                listener.RemoveAll(x => !x.IsAlive || x.Target == null || x.Target == obj);
            }
        }

        public static void Publish(Enum message, object parameter = null)
        {
            Publish(message.ToString(), parameter);
        }

        public static void Publish(string message, object parameter = null)
        {
            if (!Listeners.ContainsKey(message)) return;
            var list = new List<WeakReference>(Listeners[message]);
            foreach (var reference in list)
            {
                if (!reference.IsAlive)
                {
                    Listeners.RemoveValue(message, reference);
                }
                else if (reference.Target != null)
                {
                    var method = reference.Target.GetType().GetMethod("Notify", new[] { typeof(string), typeof(object) });
                    if (method != null) method.Invoke(reference.Target, new object[] { message, parameter });
                }
            }
        }
    }
}
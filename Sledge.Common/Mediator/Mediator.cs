using System;
using System.Collections.Generic;

namespace Sledge.Common.Mediator
{
    /// <summary>
    /// The mediator is a static event/communications manager.
    /// </summary>
    public static class Mediator
    {
        private static readonly MultiDictionary<string, WeakReference> Listeners;

        static Mediator()
        {
            Listeners = new MultiDictionary<string, WeakReference>();
        }

        public static void Subscribe(string message, IMediatorListener obj)
        {
            Listeners.AddValue(message, new WeakReference(obj));
        }

        public static void Unsubscribe(string message, IMediatorListener obj)
        {
            if (!Listeners.ContainsKey(message)) return;
            var l = Listeners[message];
            l.RemoveAll(x => !x.IsAlive || x.Target == null || x.Target == obj);
        }

        public static void Publish(string message, object parameter)
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
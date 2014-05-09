using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Sledge.Common.Mediator
{
    /// <summary>
    /// The mediator is a static event/communications manager.
    /// </summary>
    public static class Mediator
    {
        public delegate void MediatorExceptionEventHandler(object sender, MediatorExceptionEventArgs e);
        public static event MediatorExceptionEventHandler MediatorException;
        private static void OnMediatorException(object sender, string message, object parameter, Exception ex)
        {
            if (MediatorException != null)
            {
                var st = new StackTrace();
                var frames = st.GetFrames() ?? new StackFrame[0];
                var msg = "Mediator exception: " + message + "(" + parameter + ")";
                foreach (var frame in frames)
                {
                    var method = frame.GetMethod();
                    msg += "\r\n    " + method.ReflectedType.FullName + "." + method.Name;
                }
                MediatorException(sender, new MediatorExceptionEventArgs(message, new Exception(msg, ex)));
            }
        }

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
                method = t.GetMethod(message, flags, null, arr.Select(x => x == null ? typeof (object) : x.GetType()).ToArray(), null);
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
                if (method != null) parameters = method.GetParameters().Select(x => (object) null).ToArray();
            }
            if (method != null)
            {
                var sync = obj as ISynchronizeInvoke;
                if (sync != null && sync.InvokeRequired) sync.Invoke(new Action(() => method.Invoke(obj, parameters)), null);
                else method.Invoke(obj, parameters);
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

        public static void Publish(Enum message, params object[] parameters)
        {
            object parameter = null;
            if (parameters.Length == 1) parameter = parameters[0];
            else if (parameters.Length > 1) parameter = parameters;
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
                    if (method != null)
                    {
                        try
                        {
                            method.Invoke(reference.Target, new[] { message, parameter });
                        }
                        catch (Exception ex)
                        {
                            OnMediatorException(method, message, parameter, ex);
                        }
                    }
                }
            }
        }
    }
}
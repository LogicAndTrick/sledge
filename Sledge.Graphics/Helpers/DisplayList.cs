using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;

namespace Sledge.Graphics.Helpers
{
    public class DisplayList : IDisposable
    {
        private static Dictionary<string, int> Lists;
        private static string CurrentList;
        public static object ListLock { get; private set; }

        static DisplayList()
        {
            Lists = new Dictionary<string, int>();
            CurrentList = null;
            ListLock = new object();
        }

        public static bool Exists(string name)
        {
            return Lists.ContainsKey(name);
        }

        public static void Create(string name)
        {
            if (Lists.ContainsKey(name)) {
                Delete(name);
            }
            var num = GL.GenLists(1);
            Lists.Add(name, num);
        }

        public static void Begin(string name)
        {
            if (!Lists.ContainsKey(name)) {
                throw new Exception("This list does not exist!");
            } else if (CurrentList != null) {
                throw new Exception("Another list (" + CurrentList + ") is already in progress!");
            }
            GL.NewList(Lists[name], ListMode.Compile);
            CurrentList = name;
        }

        public static void End(string name)
        {
            if (CurrentList == null) {
                throw new Exception("There is currently no list in progress to end");
            } else if (CurrentList != name) {
                throw new Exception("Cannot end " + name + ", as " + CurrentList + " is the current list.");
            }
            GL.EndList();
            CurrentList = null;
        }

        public static void Call(string name)
        {
            if (!Lists.ContainsKey(name)) {
                throw new Exception("This list does not exist!");
            }
            GL.CallList(Lists[name]);
        }

        public static void Delete(string name)
        {
            if (Lists.ContainsKey(name)) {
                GL.DeleteLists(Lists[name], 1);
                Lists.Remove(name);
            }
        }

        public static void DeleteAll()
        {
            foreach (var e in Lists) {
                GL.DeleteLists(e.Value, 1);
            }
            Lists.Clear();
        }

        public static DisplayList Using(string name)
        {
            return new DisplayList(name);
        }

        private string Name;

        public DisplayList(string name)
        {
            Name = name;
            Create(Name);
            Begin(Name);
        }

        public void Dispose()
        {
            End(Name);
        }
    }
}

using System.Linq;
using OpenTK.Input;

namespace Sledge.EditorNew.UI
{
    public static class Input
    {
        public static bool Ctrl
        {
            get { return IsAnyKeyDown(Key.ControlLeft, Key.ControlRight); }
        }

        public static bool Shift
        {
            get { return IsAnyKeyDown(Key.ShiftLeft, Key.ShiftRight); }
        }

        public static bool Alt
        {
            get { return IsAnyKeyDown(Key.AltLeft, Key.AltRight); }
        }

        public static bool IsKeyDown(Key key)
        {
            return IsAnyKeyDown(key);
        }

        public static bool IsKeyUp(Key key)
        {
            return IsAnyKeyUp(key);
        }

        public static bool IsAnyKeyDown(params Key[] keys)
        {
            var ks = Keyboard.GetState();
            return keys.Any(ks.IsKeyDown);
        }

        public static bool AreAllKeysDown(params Key[] keys)
        {
            var ks = Keyboard.GetState();
            return keys.All(ks.IsKeyDown);
        }

        public static bool IsAnyKeyUp(params Key[] keys)
        {
            var ks = Keyboard.GetState();
            return keys.Any(ks.IsKeyUp);
        }

        public static bool AreAllKeysUp(params Key[] keys)
        {
            var ks = Keyboard.GetState();
            return keys.All(ks.IsKeyUp);
        }
    }
}

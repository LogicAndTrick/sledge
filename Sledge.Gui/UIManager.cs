using System;

namespace Sledge.Gui
{
    public static class UIManager
    {
        public static IUIManager Manager { get; set; }

        public static void Initialise()
        {
            var platform = DetectPlatform();
            var implementation = DetectImplementation(platform);
            Initialise(implementation);
        }

        private static void Initialise(string implementation)
        {
            throw new NotImplementedException();
        }

        private static string DetectImplementation(UIPlatform platform)
        {
            throw new NotImplementedException();
        }

        private static UIPlatform DetectPlatform()
        {
            throw new NotImplementedException();
        }
    }
}
using System;
using System.Globalization;
using System.Threading;
using Sledge.Shell;

namespace Sledge.Editor
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            // From: https://stackoverflow.com/a/9160150
            // This is the easiest and most consistent way to force decimals to be used in winforms controls.
            // Users expect decimals to be displayed, since that's how VHE and other editors behave.
            // This is a deviation from expected behaviour of an application, which is why this is in
            // the editor bootstrapper, and not in the shell.

            var forceDecimalCulture = (CultureInfo)Thread.CurrentThread.CurrentCulture.Clone();

            forceDecimalCulture.NumberFormat.NumberDecimalSeparator = ".";
            forceDecimalCulture.NumberFormat.NumberGroupSeparator = ",";
            forceDecimalCulture.NumberFormat.NumberGroupSizes = new [] { 3 };
            forceDecimalCulture.NumberFormat.NumberNegativePattern = 1;

            Thread.CurrentThread.CurrentCulture = forceDecimalCulture;

            // We're finished ruining the culture, let's run the app now.
            Startup.Run();
        }
    }
}

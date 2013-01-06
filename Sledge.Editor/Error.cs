using System.Windows.Forms;
using System.Diagnostics;

namespace Sledge.Editor
{
    public static class Error
    {
        /// <summary>
        /// A critical error displays a message box and then closes the application.
        /// </summary>
        public static void Critical(string message)
        {
            MessageBox.Show("A critical error has occurred:\n\n" + message + "\n\nThe application will now close.", "Critical Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Application.Exit();
        }

        /// <summary>
        /// A warning displays a message box.
        /// </summary>
        public static void Warning(string message)
        {
            MessageBox.Show("An error has occurred:\n\n" + message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /// <summary>
        /// A log writes the message to the debug output.
        /// </summary>
        public static void Log(string message)
        {
            Debug.WriteLine("Log output: " + message);
        }
    }
}
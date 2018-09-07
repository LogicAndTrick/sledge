using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Sledge.Translator
{
    /// <summary>
    /// Class that represent the main translator form.
    /// </summary>
    public partial class FormMain : Form
    {
        /// <summary>
        /// Defines if the data is dirty or not (modifications have been made).
        /// </summary>
        private bool DirtyData { get; set; }

        /// <summary>
        /// Construct the main translator form.
        /// </summary>
        public FormMain()
        {
            InitializeComponent();
            DirtyData = false;

            // Test data for testing purposes
            /*saveTranslationFileToolStripMenuItem.Enabled = true;
            dataGridView.Rows.Add("HELLO_WORLD", "Hello World!", "Bonjour !");
            dataGridView.Rows.Add("APP_NAME", "Sledge Editor", "Editeur Sledge");
            dataGridView.Rows.Add("NEW_FILE", "New file", "Nouveau fichier");
            dataGridView.Rows.Add("OPEN_FILE", "Open file", "Ouvrir un fichier");
            dataGridView.Rows.Add("SAVE_FILE", "Save file", "Enregistrer le fichier");
            dataGridView.Rows.Add("SAVE_AS_FILE", "Save file as", "Enregistrer le fichier sous");
            dataGridView.Rows.Add("CLOSE_FILE", "Close file", "Fermer le fichier");
            dataGridView.Rows.Add("EXPORT_FILE", "Export file", "Exporter le fichier");
            dataGridView.Rows.Add("QUIT_FILE", "Quit", "Quitter");*/
        }

        /// <summary>
        /// Save the translation file, remove the asterisk in the title and mark the data as no longer dirty.
        /// </summary>
        private void saveTranslationFile()
        {
            MessageBox.Show(this, "TODO - Save the file.", "TODO", MessageBoxButtons.OK, MessageBoxIcon.Information);

            int starIndex = Text.LastIndexOf('*');
            if (starIndex > -1)
                Text = Text.Remove(starIndex, 1);

            DirtyData = false;
        }

        #region Form's events
        /// <summary>
        /// Called whenever the "Open translation file" menu item has been clicked on.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openTranslationFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (DirtyData)
            {
                if (MessageBox.Show(
                        this,
                        "The translation has unsaved changes. Would you like to save them before opening a new translation file.",
                        "Unsaved changes",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question) == DialogResult.Yes)
                    saveTranslationFile();
            }

            MessageBox.Show(this, "TODO - Show the file open dialog and open the file if it's a success.", "TODO", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Called whenever the "Save translation file" menu item has been clicked on.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveTranslationFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveTranslationFile();
        }

        /// <summary>
        /// Called whenever the "Quit" menu item has been clicked on.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!DirtyData)
            {
                Application.Exit();
                return;
            }

            switch (MessageBox.Show(this, "The translation has unsaved changes.\n\nPress \"Yes\" to save the changes and close the application.\nPress \"No\" to discard the changes and close the application.\nPress \"Cancel\" to go back to the application.", "Unsaved changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
            {
                case DialogResult.Yes:
                    saveTranslationFile();
                    Application.Exit();
                    break;
                case DialogResult.No:
                    Application.Exit();
                    break;
            }
        }

        /// <summary>
        /// Called whenever the "Wiki page about translations" menu item has been clicked on.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void wikiPageAboutTranslationsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/LogicAndTrick/sledge/wiki/Supporting-translations");
        }

        /// <summary>
        /// Called whenever the "Report a bug" menu item has been clicked on.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void reportABugToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                    this,
                    "When making the report, please precise that the issue is about the translator and not Sledge itself by assigning the \"translator\" label.\n\n" +
                    "Thanks in advance for your cooperation!",
                    "When making the report...",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
            );

            Process.Start("https://github.com/LogicAndTrick/sledge/issues");
        }

        /// <summary>
        /// Called whenever the "About" menu item has been clicked on.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                    this,
                    "Sledge Translator v1.0.0.0.\n\n" +
                    "This application has been created by Joël \"Shepard62FR\" Troch and is maintained by Sledge's author Daniel \"LogicAndTrick\" Walder and it's community.\n\n" +
                    "For more information, consult Sledge's website at http://sledge-editor.com/",
                    "About this application",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
            );
        }

        /// <summary>
        /// Called whenever the current cell dirty state has been changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (!DirtyData)
            {
                Text = Text.Insert(Text.Length, "*");
                DirtyData = true;
            }
        }

        private void dataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView.GetCellCount(DataGridViewElementStates.Selected) > 0)
            {
                toolStripStatusLabel.Text = dataGridView.SelectedCells[0].Value.ToString();
            }
        }
        #endregion
    }
}

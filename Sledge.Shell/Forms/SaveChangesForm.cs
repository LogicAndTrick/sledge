using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sledge.Common.Shell.Documents;
using Sledge.Common.Translations;

namespace Sledge.Shell.Forms
{
    public partial class SaveChangesForm : Form
    {
        public SaveChangesForm(List<IDocument> unsaved)
        {
            InitializeComponent();
            DialogResult = DialogResult.Cancel;

            foreach (var document in unsaved)
            {
                DocumentList.Items.Add(document.Name + " *");
            }
            
            CreateHandle();
        }

        private void SaveAllClicked(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Yes;
            Close();
        }

        private void DiscardAllClicked(object sender, EventArgs e)
        {
            DialogResult = DialogResult.No;
            Close();
        }

        private void CancelClicked(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        public void Translate(ITranslationStringProvider translation)
        {
            this.InvokeLater(() => {
               SaveAllButton.Text = translation.GetString(typeof(SaveChangesForm).FullName + ".SaveAll");
               DiscardButton.Text = translation.GetString(typeof(SaveChangesForm).FullName + ".DiscardAll");
               CancelButton.Text = translation.GetString(typeof(SaveChangesForm).FullName + ".Cancel");
               UnsavedChangesLabel.Text = translation.GetString(typeof(SaveChangesForm).FullName + ".UnsavedChangesMessage");
               Text = translation.GetString(typeof(SaveChangesForm).FullName + ".Title");
            });
        }
    }
}

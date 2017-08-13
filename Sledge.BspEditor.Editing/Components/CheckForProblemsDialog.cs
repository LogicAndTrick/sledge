using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Editing.Problems;
using Sledge.Common.Translations;
using Sledge.Shell;

namespace Sledge.BspEditor.Editing.Components
{
    public partial class CheckForProblemsDialog : Form, IManualTranslate
    {
        private readonly MapDocument _document;

        public CheckForProblemsDialog(MapDocument document)
        {
            _document = document;
            InitializeComponent();
        }

        public void Translate(TranslationStringsCollection strings)
        {
            CreateHandle();
            var prefix = GetType().FullName;
            this.Invoke(() =>
            {
                Text = strings.GetString(prefix, "Title");
                grpDetails.Text = strings.GetString(prefix, "Details");
                btnGoToError.Text = strings.GetString(prefix, "GoToError");
                btnFix.Text = strings.GetString(prefix, "FixError");
                btnFixAllOfType.Text = strings.GetString(prefix, "FixAllOfType");
                btnFixAll.Text = strings.GetString(prefix, "FixAll");
                chkVisibleOnly.Text = strings.GetString(prefix, "VisibleObjectsOnly");
                btnClose.Text = strings.GetString(prefix, "CloseButton");
            });
        }

        protected override void OnLoad(EventArgs e)
        {
            DoCheck();
            base.OnLoad(e);
            btnClose.Select();
        }

        private List<Problem> _problems;

        private void DoCheck()
        {
            _problems = ProblemChecker.Check(_document, chkVisibleOnly.Checked).ToList();
            ProblemsList.BeginUpdate();
            ProblemsList.Items.Clear();
            ProblemsList.Items.AddRange(_problems.OfType<object>().ToArray());
            ProblemsList.EndUpdate();
        }

        private void UpdateSelectedProblem(object sender, EventArgs e)
        {
            var sel = ProblemsList.SelectedItem as Problem;
            DescriptionTextBox.Text = sel == null ? "" : sel.Description;
            btnGoToError.Enabled = false;
            btnFix.Enabled = sel != null && sel.Fix != null;
            btnFixAllOfType.Enabled = sel != null && sel.Fix != null;
            btnFixAll.Enabled = _problems.Any(x => x.Fix != null);

            // todo
            //if (sel != null)
            //{
            //    var objects = sel.Objects.Union(sel.Faces.Select(x => x.Parent)).Distinct().ToList();
            //    _document.PerformAction("Select problem", new ChangeSelection(objects, _document.Selection.GetSelectedObjects()));
            //    GoToButton.Enabled = objects.Any();
            //}
        }

        private void GoToError(object sender, EventArgs e)
        {
            //Mediator.Publish(HotkeysMediator.CenterAllViewsOnSelection);
        }

        private void FixErrors(params Problem[] problems)
        {
            var fixes = problems
                .Where(x => x != null && x.Fix != null)
                .Select(x => x.Fix(x))
                .Where(x => x != null)
                .ToArray();
            if (fixes.Any())
            {
                var name = "Fix " + fixes.Length + " problem" + (fixes.Length == 1 ? "" : "s");
                //_document.PerformAction(name, new ActionCollection(fixes));
                DoCheck();
            }
        }

        private void FixError(object sender, EventArgs e)
        {
            var sel = ProblemsList.SelectedItem as Problem;
            FixErrors(sel);
        }

        private void FixAllOfType(object sender, EventArgs e)
        {
            var sel = ProblemsList.SelectedItem as Problem;
            if (sel == null) return;
            FixErrors(_problems.Where(x => x.Type == sel.Type).ToArray());
        }

        private void FixAll(object sender, EventArgs e)
        {
            FixErrors(_problems.ToArray());
        }

        private void VisibleOnlyCheckboxChanged(object sender, EventArgs e)
        {
            DoCheck();
        }
    }
}

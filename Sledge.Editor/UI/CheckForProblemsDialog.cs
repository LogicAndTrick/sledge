using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Sledge.Common.Mediator;
using Sledge.Editor.Actions;
using Sledge.Editor.Actions.MapObjects.Selection;
using Sledge.Editor.Documents;
using Sledge.Editor.Problems;
using Sledge.Settings;

namespace Sledge.Editor.UI
{
    public partial class CheckForProblemsDialog : Form
    {
        private readonly Document _document;

        public CheckForProblemsDialog(Document document)
        {
            _document = document;
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            DoCheck();
            base.OnLoad(e);
            CloseButton.Select();
        }

        private List<Problem> _problems;

        private void DoCheck()
        {
            _problems = ProblemChecker.Check(_document.Map, VisibleOnlyCheckbox.Checked).ToList();
            ProblemsList.BeginUpdate();
            ProblemsList.Items.Clear();
            ProblemsList.Items.AddRange(_problems.OfType<object>().ToArray());
            ProblemsList.EndUpdate();
        }

        private void UpdateSelectedProblem(object sender, EventArgs e)
        {
            var sel = ProblemsList.SelectedItem as Problem;
            DescriptionTextBox.Text = sel == null ? "" : sel.Description;
            GoToButton.Enabled = false;
            FixButton.Enabled = sel != null && sel.Fix != null;
            FixAllTypeButton.Enabled = sel != null && sel.Fix != null;
            FixAllButton.Enabled = _problems.Any(x => x.Fix != null);

            if (sel != null)
            {
                var objects = sel.Objects.Union(sel.Faces.Select(x => x.Parent)).Distinct().ToList();
                _document.PerformAction("Select problem", new ChangeSelection(objects, _document.Selection.GetSelectedObjects()));
                GoToButton.Enabled = objects.Any();
            }
        }

        private void GoToError(object sender, EventArgs e)
        {
            Mediator.Publish(HotkeysMediator.CenterAllViewsOnSelection);
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
                _document.PerformAction(name, new ActionCollection(fixes));
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

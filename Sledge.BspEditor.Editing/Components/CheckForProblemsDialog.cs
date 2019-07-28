using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Editing.Problems;
using Sledge.BspEditor.Editing.Properties;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Modification.Operations.Selection;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common.Shell.Components;
using Sledge.Common.Shell.Context;
using Sledge.Common.Translations;
using Sledge.Shell;

namespace Sledge.BspEditor.Editing.Components
{
    [Export(typeof(IDialog))]
    [AutoTranslate]
    public partial class CheckForProblemsDialog : Form, IDialog, IManualTranslate
    {
        [Import("Shell", typeof(Form))] private Lazy<Form> _parent;
        [Import] private IContext _context;
        [ImportMany] private IEnumerable<Lazy<IProblemCheck>> _problemCheckers;

        private List<Subscription> _subscriptions;
        private List<ProblemWrapper> _problems;

        private bool _visibleOnly;
        private bool _selectedOnly;

        public CheckForProblemsDialog()
        {
            InitializeComponent();
            _problems = new List<ProblemWrapper>();
            _visibleOnly = true;
            _selectedOnly = false;

            using (var icon = new Bitmap(Resources.Menu_CheckForProblems))
            {
                var ptr = icon.GetHicon();
                var ico = Icon.FromHandle(ptr);
                Icon = ico;
                ico.Dispose();
            }
        }

        public void Translate(ITranslationStringProvider strings)
        {
            CreateHandle();
            var prefix = GetType().FullName;
            this.InvokeLater(() =>
            {
                Text = strings.GetString(prefix, "Title");
                grpDetails.Text = strings.GetString(prefix, "Details");
                btnGoToError.Text = strings.GetString(prefix, "GoToError");
                btnFix.Text = strings.GetString(prefix, "FixError");
                btnFixAllOfType.Text = strings.GetString(prefix, "FixAllOfType");
                btnFixAll.Text = strings.GetString(prefix, "FixAll");
                chkVisibleOnly.Text = strings.GetString(prefix, "VisibleObjectsOnly");
                chkSelectedOnly.Text = strings.GetString(prefix, "SelectedObjectsOnly");
                lnkExtraDetails.Text = strings.GetString(prefix, "ClickForAdditionalDetails");
                btnClose.Text = strings.GetString(prefix, "CloseButton");
            });
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            Oy.Publish("Context:Remove", new ContextInfo("BspEditor:CheckForProblems"));
        }

		protected override void OnMouseEnter(EventArgs e)
		{
            Focus();
            base.OnMouseEnter(e);
        }

        public bool IsInContext(IContext context)
        {
            return context.HasAny("BspEditor:CheckForProblems");
        }

        public void SetVisible(IContext context, bool visible)
        {
            this.InvokeLater(() =>
            {
                if (visible)
                {
                    if (!Visible) Show(_parent.Value);
                    Subscribe();
                    DoCheck();
                }
                else
                {
                    Hide();
                    Unsubscribe();
                }
            });
        }

        private void Subscribe()
        {
            if (_subscriptions != null) return;
            _subscriptions = new List<Subscription>
            {
                Oy.Subscribe<Change>("MapDocument:Changed", DocumentChanged),
                Oy.Subscribe<MapDocument>("Document:Activated", DocumentActivated)
            };
        }

        private void Unsubscribe()
        {
            if (_subscriptions == null) return;
            _subscriptions.ForEach(x => x.Dispose());
            _subscriptions = null;
        }

        private async Task DocumentActivated(MapDocument document)
        {
            await DoCheck(document);
        }

        private async Task DocumentChanged(Change change)
        {
            await DoCheck(change.Document);
        }

        private Task DoCheck()
        {
            return DoCheck(_context.Get<MapDocument>("ActiveDocument"));
        }

        private async Task DoCheck(MapDocument doc)
        {
            _problems = doc == null ? new List<ProblemWrapper>() : await Check(doc, GetFilter(_visibleOnly, _selectedOnly));
            this.InvokeLater(() =>
            {
                var si = ProblemsList.SelectedIndex;
                ProblemsList.BeginUpdate();
                ProblemsList.Items.Clear();
                ProblemsList.Items.AddRange(_problems.OfType<object>().ToArray());
                if (si < 0 || si >= ProblemsList.Items.Count) si = 0;
                if (si < ProblemsList.Items.Count) ProblemsList.SelectedIndex = si;
                ProblemsList.EndUpdate();
                UpdateSelectedProblem(null, EventArgs.Empty);
            });
        }

        private Predicate<IMapObject> GetFilter(bool visibleOnly, bool selectedOnly)
        {
            return x =>
            {
                if (selectedOnly && !x.IsSelected) return false;
                if (visibleOnly && x.Data.OfType<IObjectVisibility>().Any(v => v.IsHidden)) return false;
                return true;
            };
        }

        private async Task<List<ProblemWrapper>> Check(MapDocument map, Predicate<IMapObject> filter)
        {
            var list = new List<ProblemWrapper>();

            var index = 1;
            foreach (var checker in _problemCheckers)
            {
                var probs = await checker.Value.Check(map, filter);
                foreach (var p in probs)
                {
                    var w = new ProblemWrapper(index++, map, p, checker.Value);
                    list.Add(w);
                }
            }

            return list;
        }

        private void UpdateSelectedProblem(object sender, EventArgs e)
        {
            var sel = ProblemsList.SelectedItem as ProblemWrapper;
            DescriptionTextBox.Text = sel?.Details ?? "";
            btnGoToError.Enabled = sel?.Problem.Objects.Any() == true;
            btnFix.Enabled = btnFixAllOfType.Enabled = sel?.CanFix == true;
            lnkExtraDetails.Visible = sel?.Url != null;
            btnFixAll.Enabled = _problems.Any(x => x.CanFix);
        }

        private void VisibleOnlyCheckboxChanged(object sender, EventArgs e)
        {
            _selectedOnly = chkSelectedOnly.Checked;
            _visibleOnly = chkVisibleOnly.Checked;
            DoCheck();
        }

        private void OpenUrl(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var sel = ProblemsList.SelectedItem as ProblemWrapper;
            var uri = sel?.Url;
            if (uri != null)
            {
                Process.Start(uri.ToString());
            }
        }

        private async void GoToError(object sender, EventArgs e)
        {
            var doc = _context.Get<MapDocument>("ActiveDocument");
            if (doc == null) return;
            
            var sel = ProblemsList.SelectedItem as ProblemWrapper;
            if (sel == null || !sel.Problem.Objects.Any()) return;

            var op = new Transaction(
                new Deselect(doc.Selection.Except(sel.Problem.Objects)),
                new Select(sel.Problem.Objects)
            );
            await MapDocumentOperation.Perform(doc, op);

            var bb = doc.Selection.GetSelectionBoundingBox();
            await Oy.Publish("MapDocument:Viewport:Focus2D", bb);
            await Oy.Publish("MapDocument:Viewport:Focus3D", bb);
        }

        private async Task FixErrors(IEnumerable<ProblemWrapper> problems)
        {
            var doc = _context.Get<MapDocument>("ActiveDocument");
            if (doc == null) return;

            var fixes = problems.Where(x => x.CanFix && x.Document == doc).ToList();
            foreach (var pw in fixes)
            {
                await pw.Checker.Fix(doc, pw.Problem);
            }
        }

        private async void FixError(object sender, EventArgs e)
        {
            if (ProblemsList.SelectedItem is ProblemWrapper sel) await FixErrors(new[] {sel});
        }

        private async void FixAllOfType(object sender, EventArgs e)
        {
            var sel = ProblemsList.SelectedItem as ProblemWrapper;
            if (sel == null) return;
            await FixErrors(_problems.Where(x => x.Checker == sel.Checker));
        }

        private async void FixAll(object sender, EventArgs e)
        {
            await FixErrors(_problems);
        }

        private void CloseWindow(object sender, EventArgs e)
        {
            Close();
        }

        private class ProblemWrapper
        {
            public int Index { get; }
            public MapDocument Document { get; set; }
            public Problem Problem { get; }
            public IProblemCheck Checker { get; }

            public string Name => Checker.Name;
            public string Details => Checker.Details;
            public Uri Url => Checker.Url;
            public bool CanFix => Checker.CanFix;

            public ProblemWrapper(int index, MapDocument document, Problem problem, IProblemCheck checker)
            {
                Index = index;
                Document = document;
                Problem = problem;
                Checker = checker;
            }

            public override string ToString()
            {
                return Index + ": " +
                       (Name ?? Checker.GetType().Name) +
                       (String.IsNullOrWhiteSpace(Problem.Text) ? "" : " - " + Problem.Text);
            }
        }
    }
}

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Sledge.Common.Mediator;
using Sledge.DataStructures.MapObjects;
using Sledge.EditorNew.Documents;
using Sledge.EditorNew.Language;
using Sledge.EditorNew.UI.Viewports;
using Sledge.Gui;
using Sledge.Gui.Containers;
using Sledge.Gui.Controls;
using Sledge.Gui.Interfaces;
using Sledge.Gui.Interfaces.Dialogs;
using Sledge.Gui.Interfaces.Models;
using Sledge.Gui.Interfaces.Shell;
using Sledge.Gui.Models;
using Sledge.Gui.QuickForms;
using Sledge.Providers.GameData;
using Sledge.Providers.Map;
using Sledge.Settings;
using Sledge.Settings.Models;
using Path = System.IO.Path;

namespace Sledge.EditorNew.UI
{
    public class Shell : IMediatorListener
    {
        private static Shell _instance;
        private readonly IShell _shell;
        private TabStrip _tabs;
        private ResizableTable _table;

        public static void Bootstrap()
        {
            _instance = new Shell(UIManager.Manager.Shell);
        }

        private Shell(IShell shell)
        {
            _shell = shell;
            Build();

            ViewportManager.Init(_table);

            Mediator.Subscribe(EditorMediator.DocumentOpened, this);
            Mediator.Subscribe(EditorMediator.DocumentClosed, this);
            Mediator.Subscribe(EditorMediator.DocumentActivated, this);
            Mediator.Subscribe(EditorMediator.DocumentAllClosed, this);
            // 
            // Mediator.Subscribe(HotkeysMediator.FileOpen, this);
            Mediator.Subscribe(HotkeysMediator.FileNew, this);

            MapProvider.Register(new RmfProvider());
            GameDataProvider.Register(new FgdProvider());
            var file = @"D:\Github\sledge\_Resources\RMF\verc_18.rmf";
            DocumentManager.AddAndSwitch(new Document(null, new Map(), SettingsManager.Games[0]));
        }

        private void Build()
        {
            var vbox = new VerticalBox();

            
            _tabs = new TabStrip();
            _tabs.TabCloseRequested += TabCloseRequested;
            _tabs.TabSelected += TabSelected;
            vbox.Add(_tabs);

            _table = new ResizableTable{ControlPadding = 5};
            vbox.Add(_table, true);

            _shell.Container.Set(vbox);
            UpdateTitle();
        }

        private void TabSelected(object sender, ITab tab)
        {
            if (tab == null) DocumentManager.SwitchTo(null);
            else DocumentManager.SwitchTo((IDocument) tab.Value);
        }

        private void TabCloseRequested(object sender, ITab tab)
        {
            // todo are you sure etc
            DocumentManager.Remove((IDocument) tab.Value);
        }

        private void DocumentOpened(IDocument document)
        {
            var tab = new Tab { Closable = true, Dirty = false, Selected = false, Text = document.Text, Value = document };
            _tabs.Tabs.Add(tab);
        }

        private void DocumentClosed(IDocument document)
        {
            var tab = _tabs.Tabs.FirstOrDefault(x => x.Value == document);
            if (tab != null) _tabs.Tabs.Remove(tab);
        }

        private void DocumentActivated(IDocument document)
        {
            UpdateTitle();
            _tabs.SelectedTab = _tabs.Tabs.FirstOrDefault(x => x.Value == document);
        }

        private void DocumentAllClosed()
        {
            UpdateTitle();
        }

        private void FileNew()
        {
            var qf = new QuickForm("Test Form").Label("This is a label").TextBox("Text Box", "Blah").OkCancel();
            qf.Open();
        }

        private void FileOpen()
        {
            var d = UIManager.Manager.ConstructDialog<IFileOpenDialog>();
            if (d.Prompt())
            {
                var text = Path.GetFileName(d.File);
                DocumentManager.AddAndSwitch(new DummyDocument(text));
            }
        }

        private void UpdateTitle()
        {
            var str = Translate.Fetch("Shell/Window.Title");
            if (DocumentManager.CurrentDocument != null)
            {
                str += " - " + DocumentManager.CurrentDocument.Text;
            }
            _shell.Title = str;
        }

        public void Notify(string message, object data)
        {
            Mediator.ExecuteDefault(this, message, data);
        }
    }
}

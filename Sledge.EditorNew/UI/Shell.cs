using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Sledge.Common.Mediator;
using Sledge.EditorNew.Bootstrap;
using Sledge.EditorNew.Documents;
using Sledge.EditorNew.Language;
using Sledge.Gui;
using Sledge.Gui.Containers;
using Sledge.Gui.Controls;
using Sledge.Gui.Interfaces;
using Sledge.Gui.Interfaces.Dialogs;
using Sledge.Gui.Interfaces.Models;
using Sledge.Gui.Interfaces.Shell;
using Sledge.Gui.Models;
using Sledge.Gui.QuickForms;
using Sledge.Settings;
using Sledge.Settings.Models;

namespace Sledge.EditorNew.UI
{
    public class Shell : IMediatorListener
    {
        private static Shell _instance;
        private readonly IShell _shell;
        private TabStrip _tabs;
        private ResizableTable _table;

        private Shell(IShell shell)
        {
            _shell = shell;
            Build();

            // Mediator.Subscribe(EditorMediator.DocumentOpened, this);
            // Mediator.Subscribe(EditorMediator.DocumentClosed, this);
            // Mediator.Subscribe(EditorMediator.DocumentActivated, this);
            // Mediator.Subscribe(EditorMediator.DocumentAllClosed, this);
            // 
            // Mediator.Subscribe(HotkeysMediator.FileOpen, this);
            // Mediator.Subscribe(HotkeysMediator.FileNew, this);

            DocumentManager.Add(new DummyDocument("Test 1!"));
            DocumentManager.Add(new DummyDocument("Test 2!"));
            DocumentManager.AddAndSwitch(new DummyDocument("Test 3!"));
        }

        private void Build()
        {
            var vbox = new VerticalBox();

            /*
            _tabs = new TabStrip();
            _tabs.TabCloseRequested += TabCloseRequested;
            _tabs.TabSelected += TabSelected;

            _table = new ResizableTable();

            vbox.Add(_tabs);
            vbox.Add(_table, true);
            */

            var co = new Collapsible {Text = "Blah Blah"};
            vbox.Add(co);
            co.Set(new Label{TextKey = "TEST!"});

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

        public static void Bootstrap()
        {
            _instance = new Shell(UIManager.Manager.Shell);
        }

        public void Notify(string message, object data)
        {
            Mediator.ExecuteDefault(this, message, data);
        }
    }
}

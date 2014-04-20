using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualBasic.ApplicationServices;
using Sledge.Common.Mediator;

namespace Sledge.Editor
{
    public class SingleInstance : WindowsFormsApplicationBase
    {
        private readonly Type _formType;
        private static SingleInstance _instance;

        public static bool EnableSingleInstance
        {
            get { return _instance.IsSingleInstance; }
            set { _instance.IsSingleInstance = value; }
        }

        public static void Start(Type formType)
        {
            _instance = new SingleInstance(formType);
            _instance.Run(System.Environment.GetCommandLineArgs());
        }

        protected SingleInstance(Type formType)
        {
            _formType = formType;
            IsSingleInstance = true;
        }

        protected override void OnStartupNextInstance(StartupNextInstanceEventArgs e)
        {
            e.BringToForeground = true;
            base.OnStartupNextInstance(e);
            Editor.ProcessArguments(e.CommandLine.ToArray());
        }

        protected override void OnCreateMainForm()
        {
            MainForm = (Form)Activator.CreateInstance(_formType);
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sledge.Shell.UI
{
    public partial class Shell : Form
    {
        private CompositionContainer _container;

        public Shell()
        {
            InitializeComponent();

            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new DirectoryCatalog(Environment.CurrentDirectory));
            _container = new CompositionContainer(catalog);

        }
    }
}

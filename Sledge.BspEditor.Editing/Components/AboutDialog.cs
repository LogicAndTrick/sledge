using System.Diagnostics;
using System.Windows.Forms;
using Sledge.BspEditor.Documents;
using Sledge.Common.Mediator;

namespace Sledge.BspEditor.Editing.Components
{
    public partial class AboutDialog : Form
    {
        public AboutDialog()
        {
            InitializeComponent();

            VersionLabel.Text = FileVersionInfo.GetVersionInfo(typeof (MapDocument).Assembly.Location).FileVersion;

            // LTLink.Click += (s, e) => Mediator.Publish(EditorMediator.OpenWebsite, "http://logic-and-trick.com");
            // GithubLink.Click += (s, e) => Mediator.Publish(EditorMediator.OpenWebsite, "https://github.com/LogicAndTrick/sledge");
            // GPLLink.Click += (s, e) => Mediator.Publish(EditorMediator.OpenWebsite, "http://www.gnu.org/licenses/gpl-2.0.html");
            // AJLink.Click += (s, e) => Mediator.Publish(EditorMediator.OpenWebsite, "http://scrub-studios.com");
            // TWHLLink.Click += (s, e) => Mediator.Publish(EditorMediator.OpenWebsite, "http://twhl.info");
        }
    }
}

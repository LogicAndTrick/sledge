using System.Globalization;
using System.Linq;

namespace Sledge.Common.Shell.Documents
{
    /// <summary>
    /// Information about a file format and the extensions it might have.
    /// </summary>
    public class FileExtensionInfo
    {
        public string Description { get; set; }
        public string[] Extensions { get; set; }

        public FileExtensionInfo(string description, params string[] extensions)
        {
            Description = description;
            Extensions = extensions;
        }

        public bool Matches(string location)
        {
            return Extensions.Any(x => location.EndsWith(x, true, CultureInfo.InvariantCulture));
        }
    }
}
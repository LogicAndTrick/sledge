using System.ComponentModel;

namespace Sledge.Common.Documents
{
    public interface IDocument : INotifyPropertyChanged
    {
        /// <summary>
        /// Name of this document, will be shown in the UI tab
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Mountable control for this document
        /// </summary>
        object Control { get; }
    }
}
namespace Sledge.Editor.Settings
{
    public class FileType
    {
        public string Extension { get; set; }
        public string Description { get; set; }
        public bool IsPrimaryFormat { get; set; }

        public FileType(string extension, string description, bool isPrimaryFormat)
        {
            Extension = extension;
            Description = description;
            IsPrimaryFormat = isPrimaryFormat;
        }
    }
}
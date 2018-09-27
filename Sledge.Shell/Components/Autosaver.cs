using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using LogicAndTrick.Oy;
using Sledge.Common.Scheduling;
using Sledge.Common.Shell.Documents;
using Sledge.Common.Shell.Settings;
using Sledge.Shell.Registers;

namespace Sledge.Shell.Components
{
    [Export(typeof(ISettingsContainer))]
    public class Autosaver : ISettingsContainer
    {
        private readonly DocumentRegister _documentRegister;

        [Setting] private bool Enabled { get; set; } = true;
        [Setting] private int IntervalMinutes { get; set; } = 5;
        [Setting] private int RetainNumber { get; set; } = 5;
        [Setting] private bool SaveToAlternateDirectory { get; set; } = false;
        [Setting] private string AutosaveDirectory { get; set; } = "";
        [Setting] private bool OnlySaveIfChanged { get; set; } = true;
        [Setting] private bool SaveDocumentOnAutosave { get; set; } = true;

        [ImportingConstructor]
        public Autosaver(
            [Import] Lazy<DocumentRegister> documentRegister
        )
        {
            _documentRegister = documentRegister.Value;
        }

        public string Name => "Sledge.Shell.Autosaver";

        public IEnumerable<SettingKey> GetKeys()
        {
            yield return new SettingKey("Autosaving", "Enabled", typeof(bool));
            yield return new SettingKey("Autosaving", "IntervalMinutes", typeof(int)) { EditorHint = "1,60" };
            yield return new SettingKey("Autosaving", "RetainNumber", typeof(int)) { EditorHint = "0,100" };
            yield return new SettingKey("Autosaving", "SaveToAlternateDirectory", typeof(bool));
            yield return new SettingKey("Autosaving", "AutosaveDirectory", typeof(string)) { EditorHint = "Directory" };
            yield return new SettingKey("Autosaving", "OnlySaveIfChanged", typeof(bool));
            yield return new SettingKey("Autosaving", "SaveDocumentOnAutosave", typeof(bool));
        }

        public void LoadValues(ISettingsStore store)
        {
            store.LoadInstance(this);
            Reschedule();
        }

        public void StoreValues(ISettingsStore store)
        {
            store.StoreInstance(this);
        }

        private void Reschedule()
        {
            Scheduler.RemoveContext<Autosaver>(x => x == this);
            if (!Enabled) return;

            var at = Math.Max(1, IntervalMinutes);
            Scheduler.Schedule(this, Autosave, Schedule.Interval(TimeSpan.FromMinutes(at)));
        }

        private void Autosave()
        {
            foreach (var doc in _documentRegister.OpenDocuments)
            {
                if (OnlySaveIfChanged && !doc.HasUnsavedChanges) continue;
                try
                {
                    Autosave(doc);
                    if (SaveDocumentOnAutosave) Save(doc);
                }
                catch
                {
                    // 
                }
            }
        }

        private void Autosave(IDocument document)
        {
            var fs = GetAutosaveFormatString(document);
            var directory = GetAutosaveFolder(document);
            if (fs == null || directory == null) return;

            // Get the filename and ensure it doesn't exist
            var date = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd-hh-mm-ss");
            var filename = Path.Combine(directory, String.Format(fs, date));
            if (File.Exists(filename)) File.Delete(filename);

            // Delete excessive autosaves
            if (RetainNumber > 0)
            {
                var asFiles = GetExistingAutosaveFiles(directory, fs);
                foreach (var file in asFiles.OrderByDescending(x => x.Value).Skip(RetainNumber))
                {
                    if (File.Exists(file.Key)) File.Delete(file.Key);
                }
            }

            // Save the file
            _documentRegister.ExportDocument(document, filename);
        }

        private void Save(IDocument document)
        {
            var filename = document.FileName;
            if (filename == null || !Directory.Exists(Path.GetDirectoryName(filename))) return;

            _documentRegister.SaveDocument(document, filename);
        }

        private string GetAutosaveFormatString(IDocument document)
        {
            var path = document.FileName;
            if (path == null) return null;

            var we = Path.GetFileNameWithoutExtension(path);
            var ex = Path.GetExtension(path);
            return we + ".auto.{0}" + ex;
        }

        private string GetAutosaveFolder(IDocument document)
        {
            if (SaveToAlternateDirectory && Directory.Exists(AutosaveDirectory)) return AutosaveDirectory;
            var dir = Path.GetDirectoryName(document.FileName);
            return Directory.Exists(dir) ? dir : null;
        }

        public Dictionary<string, DateTime> GetExistingAutosaveFiles(string directory, string formatString)
        {
            var ret = new Dictionary<string, DateTime>();
            if (formatString == null || directory == null) return ret;

            // Search for matching files
            var files = Directory.GetFiles(directory, String.Format(formatString, "*"));
            foreach (var file in files)
            {
                // Match the date portion with a regex
                var re = Regex.Escape(formatString.Replace("{0}", ":")).Replace(":", "{0}");
                var regex = String.Format(re, "(\\d{4})-(\\d{2})-(\\d{2})-(\\d{2})-(\\d{2})-(\\d{2})");
                var match = Regex.Match(Path.GetFileName(file), regex, RegexOptions.IgnoreCase);
                if (!match.Success) continue;

                // Parse the date and add it if it is valid
                var result = DateTime.TryParse(
                    String.Format("{0}-{1}-{2}T{3}:{4}:{5}Z",
                        match.Groups[1].Value, match.Groups[2].Value,
                        match.Groups[3].Value, match.Groups[4].Value,
                        match.Groups[5].Value, match.Groups[6].Value
                    ),
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal,
                    out var date
                );
                if (result) ret.Add(file, date);
            }
            return ret;
        }
    }
}

<Query Kind="Statements" />

var baseFolder = @"D:\Github\sledge\Sledge.EditorNew";
var res = Path.Combine(baseFolder, "Properties", "Resources.resx");
var files = Directory.GetFiles(Path.Combine(baseFolder, "Resources")).Select (x => ".." + x.Substring(baseFolder.Length));

var doc = XDocument.Load(res);
foreach (var d in doc.Descendants("data")) {
	foreach (var v in d.Elements("value").Where (x => x.Value != null))
	{
		var spl = v.Value.Split(';');
		var fileName = files.FirstOrDefault (f => String.Equals(f, spl[0], StringComparison.InvariantCultureIgnoreCase)) ?? spl[0];
		spl[0] = fileName;
		v.Value = String.Join(";", spl);
	}
}
doc.Save(res+"_corrected");


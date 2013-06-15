using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sledge.DataStructures.GameData;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Documents;

namespace Sledge.Editor.UI
{
    public partial class MapInformationDialog : Form
    {
        public MapInformationDialog(Document document)
        {
            InitializeComponent();
            CalculateStats(document);
        }

        private void CalculateStats(Document document)
        {
            var all = document.Map.WorldSpawn.FindAll();
            var solids = all.OfType<Solid>().ToList();
            var numSolids = solids.Count;
            var numFaces = solids.Sum(x => x.Faces.Count);
            var entities = all.OfType<Entity>().ToList();
            var numPointEnts = entities.Count(x => !x.Children.Any());
            var numSolidEnts = entities.Count(x => x.Children.Any());
            var uniqueTextures = solids.SelectMany(x => x.Faces).Select(x => x.Texture.Name).Distinct().ToList();
            var numUniqueTextures = uniqueTextures.Count;
            // todo texture memory

            NumSolids.Text = numSolids.ToString(CultureInfo.InvariantCulture);
            NumFaces.Text = numFaces.ToString(CultureInfo.InvariantCulture);
            NumPointEntities.Text = numPointEnts.ToString(CultureInfo.InvariantCulture);
            NumSolidEntities.Text = numSolidEnts.ToString(CultureInfo.InvariantCulture);
            NumUniqueTextures.Text = numUniqueTextures.ToString(CultureInfo.InvariantCulture);
            // TextureMemory.Text = textureMemory.ToString(CultureInfo.InvariantCulture);
        }
    }
}

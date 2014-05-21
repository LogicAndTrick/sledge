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
using Sledge.Providers.Texture;

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
            var faces = solids.SelectMany(x => x.Faces).ToList();
            var entities = all.OfType<Entity>().ToList();
            var numSolids = solids.Count;
            var numFaces = faces.Count;
            var numPointEnts = entities.Count(x => !x.HasChildren);
            var numSolidEnts = entities.Count(x => x.HasChildren);
            var uniqueTextures = faces.Select(x => x.Texture.Name).Distinct().ToList();
            var numUniqueTextures = uniqueTextures.Count;
            var textureMemory = faces.Select(x => x.Texture.Texture)
                .Where(x => x != null)
                .Distinct()
                .Sum(x => x.Width * x.Height * 3); // 3 bytes per pixel
            var textureMemoryMb = textureMemory / (1024m * 1024m);
            // todo texture memory, texture packages

            NumSolids.Text = numSolids.ToString(CultureInfo.CurrentCulture);
            NumFaces.Text = numFaces.ToString(CultureInfo.CurrentCulture);
            NumPointEntities.Text = numPointEnts.ToString(CultureInfo.CurrentCulture);
            NumSolidEntities.Text = numSolidEnts.ToString(CultureInfo.CurrentCulture);
            NumUniqueTextures.Text = numUniqueTextures.ToString(CultureInfo.CurrentCulture);
            // TextureMemory.Text = textureMemory.ToString(CultureInfo.CurrentCulture);
            TextureMemory.Text = textureMemory.ToString("#,##0", CultureInfo.CurrentCulture)
                + " bytes (" + textureMemoryMb.ToString("0.00", CultureInfo.CurrentCulture) + " MB)";
            foreach (var tp in document.GetUsedTexturePackages())
            {
                TexturePackages.Items.Add(tp);
            }
        }
    }
}

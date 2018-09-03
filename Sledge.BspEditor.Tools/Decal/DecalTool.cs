using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Modification.Operations.Tree;
using Sledge.BspEditor.Primitives.MapData;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.BspEditor.Rendering.Viewport;
using Sledge.BspEditor.Tools.Properties;
using Sledge.Common;
using Sledge.Common.Shell.Components;
using Sledge.Common.Shell.Hotkeys;
using Sledge.DataStructures.Geometric;
using Sledge.Rendering.Cameras;

namespace Sledge.BspEditor.Tools.Decal
{
    /// <summary>
    /// The decal tool creates a decal on any face that is clicked in the 3D viewport.
    /// The decal will be created with the current texture in the texture toolbar.
    /// </summary>
    [Export(typeof(ITool))]
    [OrderHint("L")]
    [DefaultHotkey("Shift+D")]
    class DecalTool : BaseTool
    {
        public DecalTool()
        {
            Usage = ToolUsage.View3D;
        }

        public override Image GetIcon()
        {
            return Resources.Tool_Decal;
        }

        public override string GetName()
        {
            return "Decal Tool";
        }

        protected override void MouseDown(MapViewport viewport, PerspectiveCamera camera, ViewportEvent e)
        {
            var vp = viewport;
            if (vp == null) return;

            // Get the ray that is cast from the clicked point along the viewport frustrum
            var (rs, re) = camera.CastRayFromScreen(new Vector3(e.X, e.Y, 0));
            var ray = new Line(rs, re);

            // Grab all the elements that intersect with the ray
            var hit = Document.Map.Root.GetIntersectionsForVisibleObjects(ray)
                .FirstOrDefault();

            if (hit == null) return; // Nothing was clicked

            CreateDecal(hit.Intersection);
        }

        private async Task CreateDecal(Vector3 origin)
        {
            var gameData = await Document.Environment.GetGameData();
            if (gameData == null) return;
            
            var gd = gameData.Classes.FirstOrDefault(x => x.Name == "infodecal");
            if (gd == null) return;

            var texture = Document.Map.Data.GetOne<ActiveTexture>()?.Name;
            if (String.IsNullOrWhiteSpace(texture)) return;

            var tc = await Document.Environment.GetTextureCollection();
            if (tc == null) return;
            
            if (!tc.HasTexture(texture)) return;

            var decal = new Primitives.MapObjects.Entity(Document.Map.NumberGenerator.Next("MapObject"))
            {
                Data =
                {
                    new EntityData
                    {
                        Name = gd.Name,
                        Properties = {{"texture", texture}}
                    },
                    new ObjectColor(Colour.GetRandomBrushColour()),
                    new Origin(origin)
                }
            };

            await MapDocumentOperation.Perform(Document, new Attach(Document.Map.Root.ID, decal));
        }
    }
}

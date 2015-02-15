using System.Drawing;
using System.Linq;
using Sledge.Common;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Actions.MapObjects.Operations;
using Sledge.Editor.Extensions;
using Sledge.Editor.Properties;
using Sledge.Editor.Rendering;
using Sledge.Graphics.Helpers;
using Sledge.Rendering;
using Sledge.Settings;

namespace Sledge.Editor.Tools
{
    /// <summary>
    /// The decal tool creates a decal on any face that is clicked in the 3D viewport.
    /// The decal will be created with the current texture in the texture toolbar.
    /// </summary>
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

        public override HotkeyTool? GetHotkeyToolType()
        {
            return HotkeyTool.Decal;
        }

        public override string GetContextualHelp()
        {
            return "Select a decal texture from the textures sidebar.\n" +
                   "*Click* a face in the 3D view to place the decal onto that face.";
        }

        private Coordinate GetIntersectionPoint(MapObject obj, Line line)
        {
            if (obj == null) return null;

            var solid = obj as Solid;
            if (solid == null) return obj.GetIntersectionPoint(line);

            return solid.Faces.Where(x => x.Opacity > 0 && !x.IsHidden)
                .Select(x => x.GetIntersectionPoint(line))
                .Where(x => x != null)
                .OrderBy(x => (x - line.Start).VectorMagnitude())
                .FirstOrDefault();
        }

        public override void MouseDown(MapViewport viewport, ViewportEvent e)
        {
            var vp = viewport as MapViewport;
            if (vp == null) return;

            // Get the ray that is cast from the clicked point along the viewport frustrum
            var ray = vp.CastRayFromScreen(e.X, e.Y);

            // Grab all the elements that intersect with the ray
            var hits = Document.Map.WorldSpawn.GetAllNodesIntersectingWith(ray);

            // Sort the list of intersecting elements by distance from ray origin and grab the first hit
            var hit = hits
                .Select(x => new {Item = x, Intersection = GetIntersectionPoint(x, ray)})
                .Where(x => x.Intersection != null)
                .OrderBy(x => (x.Intersection - ray.Start).VectorMagnitude())
                .FirstOrDefault();

            if (hit == null) return; // Nothing was clicked

            CreateDecal(hit.Intersection);
        }

        private void CreateDecal(Coordinate origin)
        {
            var gd = Document.GameData.Classes.FirstOrDefault(x => x.Name == "infodecal");
            if (gd == null)
            {
                System.Windows.Forms.MessageBox.Show("`infodecal` was not found in the entity list.", "FGD Data Not Found",
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
                return;
            }
            var selected = Document.TextureCollection.SelectedTexture;
            var textureName = selected == null ? "{TARGET" : selected.Name;

            if (Document.TextureCollection.GetItem(textureName) == null)
            {
                return;
            }

            var decal = new Entity(Document.Map.IDGenerator.GetNextObjectID())
            {
                EntityData = new EntityData(gd),
                ClassName = gd.Name,
                Colour = Colour.GetRandomBrushColour(),
                Origin = origin
            };
            decal.SetDecal(TextureHelper.Get(textureName.ToLowerInvariant()));
            decal.EntityData.SetPropertyValue("texture", textureName);

            Document.PerformAction("Apply decal", new Create(Document.Map.WorldSpawn.ID, decal));
        }

        public override void MouseEnter(MapViewport viewport, ViewportEvent e)
        {
            // 
        }

        public override void MouseLeave(MapViewport viewport, ViewportEvent e)
        {
            // 
        }

        public override void MouseClick(MapViewport viewport, ViewportEvent e)
        {
            // Not used
        }

        public override void MouseDoubleClick(MapViewport viewport, ViewportEvent e)
        {
            // Not used
        }

        public override void MouseUp(MapViewport viewport, ViewportEvent e)
        {
            // 
        }

        public override void MouseWheel(MapViewport viewport, ViewportEvent e)
        {
            // 
        }

        public override void MouseMove(MapViewport viewport, ViewportEvent e)
        {
            // 
        }

        public override void KeyPress(MapViewport viewport, ViewportEvent e)
        {
            // 
        }

        public override void KeyDown(MapViewport viewport, ViewportEvent e)
        {
            // 
        }

        public override void KeyUp(MapViewport viewport, ViewportEvent e)
        {
            // 
        }

        public override void UpdateFrame(MapViewport viewport, Frame frame)
        {
            // 
        }

        public override void Render(MapViewport viewport)
        {
            // 
        }

        public override HotkeyInterceptResult InterceptHotkey(HotkeysMediator hotkeyMessage, object parameters)
        {
            return HotkeyInterceptResult.Continue;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenTK;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Documents;
using Sledge.Editor.Extensions;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Interfaces;
using Sledge.Rendering.Scenes.Elements;

namespace Sledge.Editor.Rendering.Converters
{
    public class EntityNameConverter : IMapObjectSceneConverter
    {
        public MapObjectSceneConverterPriority Priority => MapObjectSceneConverterPriority.DefaultLow;

        public bool ShouldStopProcessing(SceneMapObject smo, MapObject obj)
        {
            return false;
        }

        public bool Supports(MapObject obj)
        {
            return Sledge.Settings.View.DrawEntityNames && obj is Entity &&
                   !String.IsNullOrWhiteSpace(obj.GetEntityData()?.Name);
        }

        public async Task<bool> Convert(SceneMapObject smo, Document document, MapObject obj)
        {
            var entity = (Entity)obj;
            var el = new EntityTextElement(entity);
            smo.SceneObjects.Add(entity.EntityData, el);
            return true;
        }

        public async Task<bool> Update(SceneMapObject smo, Document document, MapObject obj)
        {
            var entity = (Entity)obj;
            if (smo.SceneObjects.ContainsKey(entity.EntityData))
            {
                var ete = smo.SceneObjects[entity.EntityData] as EntityTextElement;
                if (ete != null)
                {
                    ete.Update(entity);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// A custom text element for entity text. The entity text position varies per viewport based on the bounding box of the entity.
        /// </summary>
        private class EntityTextElement : TextElement
        {
            public override string ElementGroup => "Entity";
            public Box Box { get; set; }

            public EntityTextElement(Entity entity) : base(PositionType.World, entity.BoundingBox.Center.ToVector3(), entity.EntityData.Name, entity.Colour)
            {
                Box = entity.BoundingBox;
                ScreenOffset = new Vector3(0, -FontSize / 2 - 5, 0);
                CameraFlags = CameraFlags.Orthographic;
            }

            public void Update(Entity entity)
            {
                Location = entity.BoundingBox.Center.ToVector3();
                Text = entity.EntityData.Name;
                Color = entity.Colour;
                Box = entity.BoundingBox;
                ClearValue("Validated");
            }

            public override bool RequiresValidation(IViewport viewport, IRenderer renderer)
            {
                return !GetValue<bool>(viewport, "Validated") || GetValue(viewport, "Zoomed", viewport.Camera.Zoom >= 1) != viewport.Camera.Zoom >= 1;
            }

            public override void Validate(IViewport viewport, IRenderer renderer)
            {
                SetValue(viewport, "Zoomed", viewport.Camera.Zoom >= 1);
                SetValue(viewport, "Validated", true);
            }

            public override IEnumerable<FaceElement> GetFaces(IViewport viewport, IRenderer renderer)
            {
                // Don't render if we're zoomed out
                var ortho = viewport.Camera as OrthographicCamera;
                if (ortho != null && ortho.Zoom < 1) yield break;

                // Adjust the location to be above the object in the viewport
                // This is done by flattening the coordinate, extracting the Y value, and then expanding it back out to the world coordinate
                var loc = Location;
                var dim = viewport.Camera.Flatten((Box.Dimensions / 2).ToVector3());
                loc += viewport.Camera.Expand(new Vector3(0, dim.Y, 0));

                // Same code as in the base class, without the screen clipping or background colour stuff
                var el = renderer.StringTextureManager.GetElement(Text, Color, PositionType, loc, AnchorX, AnchorY, FontName, FontSize, FontStyle);
                foreach (var v in el.Vertices) v.Position.Offset += ScreenOffset;
                el.CameraFlags = CameraFlags.Orthographic;
                yield return el;
            }
        }
    }
}
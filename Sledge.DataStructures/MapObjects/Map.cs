using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sledge.Common;

namespace Sledge.DataStructures.MapObjects
{
    public class Map
    {
        public decimal Version { get; set; }
        public List<Visgroup> Visgroups { get; private set; }
        public List<Camera> Cameras { get; private set; }
        public Camera ActiveCamera { get; set; }
        public World WorldSpawn { get; set; }
        public IDGenerator IDGenerator { get; private set; }

        public Map()
        {
            Version = 1;
            Visgroups = new List<Visgroup>();
            Cameras = new List<Camera>();
            ActiveCamera = null;
            IDGenerator = new IDGenerator();
            WorldSpawn = new World(IDGenerator.GetNextObjectID());
        }

        public IEnumerable<string> GetAllTextures()
        {
            return GetAllTexturesRecursive(WorldSpawn).Distinct();
        }

        private static IEnumerable<string> GetAllTexturesRecursive(MapObject obj)
        {
            if (obj is Solid)
            {
                return ((Solid) obj).Faces.Select(f => f.Texture.Name);
            }
            else
            {
                return obj.Children.SelectMany(GetAllTexturesRecursive);
            }
        }

        /// <summary>
        /// Should be called when a map is loaded. Sets up visgroups, object ids, gamedata, and textures.
        /// </summary>
        public void PostLoadProcess(GameData.GameData gameData, Func<string, ITexture> textureAccessor)
        {
            // Set gamedata
            SetMapGameDataRecursive(WorldSpawn, gameData);

            // Set textures
            SetMapTexturesRecursive(WorldSpawn, textureAccessor);

            var all = WorldSpawn.FindAll();

            // Set maximum ids
            var maxObjectId = all.Max(x => x.ID);
            var maxFaceId = all.OfType<Solid>().SelectMany(x => x.Faces).Max(x => x.ID);
            IDGenerator.Reset(maxObjectId, maxFaceId);

            // todo visgroups
        }

        private static void SetMapGameDataRecursive(MapObject obj, GameData.GameData gd)
        {
            if (obj is Entity)
            {
                ((Entity)obj).GameData = gd.Classes.FirstOrDefault(x => x.Name == ((Entity)obj).EntityData.Name);
                obj.UpdateBoundingBox();
            }
            else
            {
                obj.Children.ForEach(x => SetMapGameDataRecursive(x, gd));
            }
        }

        private static void SetMapTexturesRecursive(MapObject obj, Func<string, ITexture> accessor)
        {
            if (obj is Solid)
            {
                ((Solid)obj).Faces.ForEach(f =>
                {
                    f.Texture.Texture = accessor(f.Texture.Name);
                    f.CalculateTextureCoordinates();
                });
            }
            else
            {
                obj.Children.ForEach(x => SetMapTexturesRecursive(x, accessor));
            }
        }
    }
}

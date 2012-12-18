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

        public Map()
        {
            Version = 1;
            Visgroups = new List<Visgroup>();
            Cameras = new List<Camera>();
            ActiveCamera = null;
            WorldSpawn = new World();
        }

        public IEnumerable<string> GetAllTextures()
        {
            return GetAllTexturesRecursive(WorldSpawn).Distinct();
        }

        public void SetMapTextures(Func<string, ITexture> accessor)
        {
            SetMapTexturesRecursive(WorldSpawn, accessor);
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

        private static void SetMapTexturesRecursive(MapObject obj, Func<string, ITexture> accessor)
        {
            if (obj is Solid)
            {
                ((Solid)obj).Faces.ForEach(f => {
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

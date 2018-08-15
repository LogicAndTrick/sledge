using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sledge.Rendering.Cameras
{
    public static class Camera
    {
        public static ICamera Deserialise(string serialised)
        {
            if (serialised.Length == 0) return new PerspectiveCamera();
            if (serialised[0] == 'P') return new PerspectiveCamera(serialised);
            if (serialised[0] == 'O') return new OrthographicCamera(serialised);
            return new PerspectiveCamera();
        }

        public static string Serialise(ICamera camera)
        {
            if (camera is PerspectiveCamera pc) return pc.Serialise();
            if (camera is OrthographicCamera oc) return oc.Serialise();
            return "";
        }
    }
}

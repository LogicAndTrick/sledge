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
            var idx = serialised.Split(new [] { '/'}, 2, StringSplitOptions.None);
            if (idx.Length == 0) idx = new [] { "PerspectiveCamera", "" };
            else if (idx.Length == 1) idx = new [] { idx[0], "" };

            if (idx[0] == "PerspectiveCamera'") return new PerspectiveCamera(idx[1]);
            if (idx[0] == "OrthographicCamera") return new OrthographicCamera(idx[1]);
            return new PerspectiveCamera();
        }

        public static string Serialise(ICamera camera)
        {
            if (camera is PerspectiveCamera pc) return camera.GetType().Name + "/" + pc.Serialise();
            if (camera is OrthographicCamera oc) return camera.GetType().Name + "/" + oc.Serialise();
            return camera?.GetType().Name;
        }
    }
}

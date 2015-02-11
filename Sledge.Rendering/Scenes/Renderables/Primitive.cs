using System.Collections.Generic;
using Sledge.Rendering.Materials;

namespace Sledge.Rendering.Scenes.Renderables
{
    public class Primitive : RenderableObject
    {
        private Material _material;

        public Material Material
        {
            get { return _material; }
            set
            {
                var uid = _material == null ? null : _material.UniqueIdentifier;
                _material = value;
                var nuid = _material == null ? null : _material.UniqueIdentifier;
                if (uid != nuid) OnPropertyChanged("RenderCritical");
                OnPropertyChanged("Material");
            }
        }
    }
}
using Sledge.DataStructures.Models;
using Sledge.Graphics.Helpers;

namespace Sledge.DataStructures.Rendering.Models
{
    public class DisplayListModelRenderable : ImmediateModelRenderable
    {
        private bool _initialised;
        private readonly string _name;

        public DisplayListModelRenderable(Model model) : base(model)
        {
            _initialised = false;
            _name = "Model/" + Model.Name;
        }

        public override void Dispose()
        {
            base.Dispose();
            DisplayList.Delete(_name);
        }

        public override void Render(object sender)
        {
            if (!_initialised)
            {
                using (DisplayList.Using(_name))
                {
                    base.Render(sender);
                }
                _initialised = true;
            }
            DisplayList.Call(_name);
        }
    }
}
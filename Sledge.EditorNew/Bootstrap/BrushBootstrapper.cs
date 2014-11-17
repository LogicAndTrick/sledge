using Sledge.EditorNew.Brushes;

namespace Sledge.EditorNew.Bootstrap
{
    public static class BrushBootstrapper
    {
        public static void Bootstrap()
        {
            BrushManager.Register(new BlockBrush());
            BrushManager.Register(new TetrahedronBrush());
            BrushManager.Register(new PyramidBrush());
            BrushManager.Register(new WedgeBrush());
            BrushManager.Register(new CylinderBrush());
            BrushManager.Register(new ConeBrush());
            BrushManager.Register(new PipeBrush());
            BrushManager.Register(new ArchBrush());
            BrushManager.Register(new SphereBrush());
            BrushManager.Register(new TorusBrush());
            BrushManager.Register(new TextBrush());
        }
    }
}
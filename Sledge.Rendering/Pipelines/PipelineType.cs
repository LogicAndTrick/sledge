namespace Sledge.Rendering.Pipelines
{
    public enum PipelineType
    {
        Wireframe,

        TexturedOpaque,
        BillboardOpaque,
        
        TexturedAlpha,
        TexturedAdditive,
        BillboardAlpha,

        WireframeModel,
        TexturedModel,

        Overlay,
    }
}
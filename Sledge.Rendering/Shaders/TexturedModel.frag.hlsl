struct FragmentIn
{
    float4 fPosition : SV_Position;
    float4 fNormal : NORMAL0;
    float2 fTexture : TEXCOORD0;
    uint1 fBone : POSITION1;
};

Texture2D Texture;
SamplerState Sampler;

float4 main(FragmentIn input) : SV_Target0
{
    float4 tex = Texture.Sample(Sampler, input.fTexture);
    if (tex.w < 0.5) discard;
    tex.w = 1;
    return tex;
}

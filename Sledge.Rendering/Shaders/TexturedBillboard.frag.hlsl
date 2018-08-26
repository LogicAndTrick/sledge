struct FragmentIn
{
    float4 fPosition : SV_Position;
    float4 fNormal : NORMAL0;
    float4 fColour : COLOR0;
    float2 fTexture : TEXCOORD0;
    float4 fTint : COLOR1;
};

Texture2D Texture;
SamplerState Sampler;
SamplerState PointSampler
{
    Filter = MIN_MAG_MIP_POINT;
    AddressU = Wrap;
    AddressV = Wrap;
    MinLOD = 0;
    MaxLOD = 0;
};

float4 main(FragmentIn input) : SV_Target0
{
    float4 tex = input.fColour * Texture.Sample(PointSampler, input.fTexture);
    if (tex.w < 0.05) discard;
    return tex * input.fTint;
}

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

float4 main(FragmentIn input) : SV_Target0
{
    return Texture.Sample(Sampler, input.fTexture);
}

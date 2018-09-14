struct FragmentIn
{
    float4 fPosition : SV_Position;
    float4 fNormal : NORMAL0;
    float4 fColour : COLOR0;
    float2 fTexture : TEXCOORD0;
    float4 fTint : COLOR1;
    uint1 fFlags : POSITION1;
};

static const float3 lightDirection = { 0.2672612f, 0.5345225f, 0.8017837f }; // = normalize({ 1, 2, 3 })
static const float lightIntensity = 0.5f;
static const float ambient = 0.8f;

Texture2D Texture;
SamplerState Sampler;

static const uint Flags_FlatColour = 1 << 1;

float4 main(FragmentIn input) : SV_Target0
{
    float4 tex = Texture.Sample(Sampler, input.fTexture) * input.fTint;
    tex = lerp(tex, input.fTint, (input.fFlags.x & Flags_FlatColour) / Flags_FlatColour);

    float incidence = dot(input.fNormal.xyz, lightDirection);
    float lighting = lightIntensity * incidence + ambient;

    float4 c = lighting * tex;
    c.w = tex.w;
    return c;
}

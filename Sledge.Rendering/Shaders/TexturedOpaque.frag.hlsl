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
static const uint Flags_AlphaTested = 1 << 2;

float4 main(FragmentIn input) : SV_Target0
{
    float4 tex = Texture.Sample(Sampler, input.fTexture);

    // flat colour flag: texture sample replaced with colour
    tex = lerp(tex, input.fColour, (input.fFlags.x & Flags_FlatColour) / Flags_FlatColour);

    // alpha tested flag: alpha < 0.5 is discarded
    // if not enabled the alpha value will be set to 1
    tex.w = lerp(1, tex.w, (input.fFlags.x & Flags_AlphaTested) / Flags_AlphaTested);

    if (tex.w < 0.5) discard;
    tex.w = 1; // This is the opaque shader

    float incidence = dot(input.fNormal.xyz, lightDirection);
    float lighting = lightIntensity * incidence + ambient;

    float4 c = lighting * tex;
    c.w = tex.w;
    return c * input.fTint;
}

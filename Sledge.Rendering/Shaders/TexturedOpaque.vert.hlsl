struct VertexIn
{
    float3 Position : POSITION0;
    float3 Normal : NORMAL0;
    float4 Colour : COLOR0;
    float2 Texture : TEXCOORD0;
    float4 Tint : COLOR1;
    uint1 Flags : POSITION1;
};

struct FragmentIn
{
    float4 fPosition : SV_Position;
    float4 fNormal : NORMAL0;
    float4 fColour : COLOR0;
    float2 fTexture : TEXCOORD0;
    float4 fTint : COLOR1;
    uint1 fFlags : POSITION1;
};

cbuffer Projection
{
    matrix Selective;
    matrix Model;
    matrix View;
    matrix Projection;
};

static const uint Flags_SelectiveTransformed = 1 << 0;

static const float4x4 Identity = { 1,0,0,0, 0,1,0,0, 0,0,1,0, 0,0,0,1 };

FragmentIn main(VertexIn input)
{
    matrix tModel = transpose(Model);
    matrix tView = transpose(View);
    matrix tProjection = transpose(Projection);

    FragmentIn output;

    float4 position = float4(input.Position, 1);
    float4 normal = float4(input.Normal, 1);

    position = mul(position, lerp(Identity, transpose(Selective), (input.Flags.x & Flags_SelectiveTransformed) / Flags_SelectiveTransformed));

    float4 modelPos = mul(position, tModel);
    float4 cameraPos = mul(modelPos, tView);
    float4 viewportPos = mul(cameraPos, tProjection);

    output.fPosition = viewportPos;
    output.fNormal = normal;
    output.fColour = input.Colour;
    output.fTexture = input.Texture;
    output.fTint = input.Tint;
    output.fFlags = input.Flags;

    return output;
}

struct VertexIn
{
    float3 Position : POSITION0;
    float3 Normal : NORMAL0;
    float4 Colour : COLOR0;
    float2 Texture : TEXCOORD0;
    float4 Tint : COLOR1;
    uint1 Flags : POSITION1;
};

struct GeometryIn
{
    float4 gPosition : SV_Position;
    float4 gNormal : NORMAL0;
    float4 gColour : COLOR0;
    float2 gTexture : TEXCOORD0;
    float4 gTint : COLOR1;
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

GeometryIn main(VertexIn input)
{
    GeometryIn output;

    float4 position = float4(input.Position, 1);
    float4 normal = float4(input.Normal, 1);

    position = mul(position, lerp(Identity, transpose(Selective), (input.Flags.x & Flags_SelectiveTransformed) / Flags_SelectiveTransformed));

    output.gPosition = position;
    output.gNormal = normal;
    output.gColour = input.Colour;
    output.gTexture = input.Texture;
    output.gTint = input.Tint;

    return output;
}

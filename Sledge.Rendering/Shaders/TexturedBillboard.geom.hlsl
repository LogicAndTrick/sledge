struct GeometryIn
{
    float4 gPosition : SV_Position;
    float4 gNormal : NORMAL0;
    float4 gColour : COLOR0;
    float2 gTexture : TEXCOORD0;
    float4 gTint : COLOR1;
};

struct FragmentIn
{
    float4 fPosition : SV_Position;
    float4 fNormal : NORMAL0;
    float4 fColour : COLOR0;
    float2 fTexture : TEXCOORD0;
    float4 fTint : COLOR1;
};

cbuffer Projection
{
    matrix Selective;
    matrix Model;
    matrix View;
    matrix Projection;
};

[maxvertexcount(4)]
void main(point GeometryIn input[1], inout TriangleStream<FragmentIn> output)
{
    matrix tModel = transpose(Model);
    matrix tView = transpose(View);
    matrix tProjection = transpose(Projection);

    float w = input[0].gNormal.x / 2;
    float h = input[0].gNormal.y / 2;

    float4 up = float4(0, h, 0, 0);
    float4 right = float4(w, 0, 0, 0);

    float4 verts[4];
    verts[0] = -right + up;
    verts[1] = +right + up;
    verts[2] = -right - up;
    verts[3] = +right - up;

    float2 texCoords[4];
    texCoords[0] = float2(0, 0);
    texCoords[1] = float2(1, 0);
    texCoords[2] = float2(0, 1);
    texCoords[3] = float2(1, 1);

    FragmentIn gOut;

    [unroll]
    for (int i = 0; i < 4; i++)
    {
        float4 modelPos = mul(verts[i], tModel) + input[0].gPosition;
        float4 cameraPos = mul(modelPos, tView);
        float4 viewportPos = mul(cameraPos, tProjection);

        gOut.fPosition = viewportPos;
        gOut.fNormal = float4(0,0,0, 1);
        gOut.fColour = input[0].gColour;
        gOut.fTexture = texCoords[i];
        gOut.fTint = input[0].gTint;

        output.Append(gOut);
    }
}
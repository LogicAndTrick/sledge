struct VertexIn
{
    float3 Position : POSITION0;
    float3 Normal : NORMAL0;
    float2 Texture : TEXCOORD0;
    uint1 Bone : POSITION1;
};

struct FragmentIn
{
    float4 fPosition : SV_Position;
    float4 fNormal : NORMAL0;
    float2 fTexture : TEXCOORD0;
    uint1 fBone : POSITION1;
};

cbuffer Projection
{
    matrix Selective;
    matrix Model;
    matrix View;
    matrix Projection;
};

cbuffer BoneTransforms
{
    matrix uTransforms[128];
};

static const float4x4 Identity = { 1,0,0,0, 0,1,0,0, 0,0,1,0, 0,0,0,1 };

FragmentIn main(VertexIn input)
{
    matrix tModel = transpose(Model);
    matrix tView = transpose(View);
    matrix tProjection = transpose(Projection);

    FragmentIn output;

    float4 position = float4(input.Position, 1);
    float4 normal = float4(input.Normal, 1);

    matrix bone = transpose(uTransforms[input.Bone.x]);
    position = mul(position, bone);
    normal = mul(normal, bone);

    float4 modelPos = mul(position, tModel);
    float4 cameraPos = mul(modelPos, tView);
    float4 viewportPos = mul(cameraPos, tProjection);

    output.fPosition = viewportPos;
    output.fNormal = normal;
    output.fTexture = input.Texture;
    output.fBone = input.Bone;

    output.fPosition.z = 0;

    return output;
}

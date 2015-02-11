#version 330

smooth in vec4 vertexPosition;
smooth in vec4 vertexNormal;
smooth in vec2 vertexTexture;
smooth in vec4 vertexMaterialColor;
smooth in vec4 vertexAccentColor;
smooth in vec4 vertexTintColor;
flat in uint vertexFlags;

uniform bool orthographic;
uniform bool wireframe;
uniform sampler2D currentTexture;

out vec4 fragmentColor;

uint FLAGS_INVISIBLE_PERSPECTIVE = 1u << 0;
uint FLAGS_INVISIBLE_ORTHOGRAPHIC = 1u << 1;
uint FLAGS_SELECTED = 1u << 2;

void main()
{
    if (orthographic && (vertexFlags & FLAGS_INVISIBLE_PERSPECTIVE) == FLAGS_INVISIBLE_PERSPECTIVE) discard;
    if (!orthographic && (vertexFlags & FLAGS_INVISIBLE_ORTHOGRAPHIC) == FLAGS_INVISIBLE_ORTHOGRAPHIC) discard;

    fragmentColor = orthographic || wireframe ? vertexAccentColor : texture(currentTexture, vertexTexture) * vertexMaterialColor;
	fragmentColor.rgb *= vertexTintColor.rgb * vertexTintColor.a;
	fragmentColor.a = vertexMaterialColor.a;
}

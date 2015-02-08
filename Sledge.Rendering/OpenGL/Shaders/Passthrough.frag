#version 330

smooth in vec4 vertexPosition;
smooth in vec4 vertexNormal;
smooth in vec2 vertexTexture;
smooth in vec4 vertexColor;
flat in uint vertexFlags;

uniform sampler2D currentTexture;

out vec4 fragmentColor;

uint FLAGS_INVISIBLE = 0x01u;

void main()
{
    if ((vertexFlags & FLAGS_INVISIBLE) == FLAGS_INVISIBLE) discard;
    fragmentColor = texture2D(currentTexture, vertexTexture);
	fragmentColor *= vertexColor;
}

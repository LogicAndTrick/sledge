#version 330

smooth in vec4 vertexPosition;
smooth in vec4 vertexNormal;
smooth in vec2 vertexTexture;
smooth in vec4 vertexColor;

uniform sampler2D currentTexture;

out vec4 fragmentColor;

void main()
{
    fragmentColor = vertexColor;
    fragmentColor = texture2D(currentTexture, vertexTexture);
	fragmentColor *= vertexColor;
}

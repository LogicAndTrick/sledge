#version 300

layout(location = 0) in vec3 position;
layout(location = 1) in vec4 color;

smooth out vec4 vertexPosition;
smooth out vec4 vertexColor;

uniform mat4 viewportMatrix;
uniform mat4 cameraMatrix;

void main()
{
	vec4 cameraPos = cameraMatrix * vec4(position, 1);
	vertexPosition = viewportMatrix * cameraPos;
	vertexColor = color;

	gl_Position = vertexPosition;
}

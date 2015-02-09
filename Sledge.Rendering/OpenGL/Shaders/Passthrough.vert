#version 330

layout(location = 0) in vec3 position;
layout(location = 1) in vec3 normal;
layout(location = 2) in vec2 texture;
layout(location = 3) in vec4 materialColor;
layout(location = 4) in vec4 accentColor;
layout(location = 5) in vec4 tintColor;
layout(location = 6) in uint flags;

smooth out vec4 vertexPosition;
smooth out vec4 vertexNormal;
smooth out vec2 vertexTexture;
smooth out vec4 vertexMaterialColor;
smooth out vec4 vertexAccentColor;
smooth out vec4 vertexTintColor;
flat out uint vertexFlags;

uniform mat4 viewportMatrix;
uniform mat4 cameraMatrix;

void main()
{
	vec4 cameraPos = cameraMatrix * vec4(position, 1);
    vec4 worldPos = viewportMatrix * cameraPos;

    vec4 worldNorm = vec4(normal, 1);

	vertexPosition = worldPos;
    vertexNormal = worldNorm;
    vertexTexture = texture;
	vertexMaterialColor = materialColor;
	vertexAccentColor = accentColor;
	vertexTintColor = tintColor;
    vertexFlags = flags;

	gl_Position = vertexPosition;
}

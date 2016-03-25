#version 330

layout(location = 0) in vec3 position;
layout(location = 1) in vec3 normal;
layout(location = 2) in vec2 texture;
layout(location = 3) in vec4 materialColor;
layout(location = 4) in vec4 accentColor;
layout(location = 5) in vec4 tintColor;
layout(location = 6) in uint flags;
layout(location = 7) in float zIndex;
layout(location = 8) in vec3 offset;

smooth out vec4 vertexPosition;
smooth out vec4 vertexNormal;
smooth out vec2 vertexTexture;
smooth out vec4 vertexMaterialColor;
smooth out vec4 vertexAccentColor;
smooth out vec4 vertexTintColor;
flat out uint vertexFlags;

uniform bool orthographic;
uniform mat4 selectionTransform;
uniform mat4 modelMatrix;
uniform mat4 viewportMatrix;
uniform mat4 cameraMatrix;
uniform vec2 viewportSize;
uniform float viewportZoom;

uint FLAGS_INVISIBLE_PERSPECTIVE = 1u << 0;
uint FLAGS_INVISIBLE_ORTHOGRAPHIC = 1u << 1;
uint FLAGS_SELECTED = 1u << 2;
uint FLAGS_NORMALISED = 1u << 3;

void main()
{
	vec4 worldPos = vec4(position, 1);
    vec4 worldNorm = vec4(normal, 1);

	if ((flags & FLAGS_NORMALISED) == FLAGS_NORMALISED) {
		worldPos *= vec4(viewportSize.x, viewportSize.y, 1, 1);
	}

	if ((flags & FLAGS_SELECTED) == FLAGS_SELECTED) {
		worldPos = selectionTransform * worldPos;
		worldNorm = transpose(inverse(selectionTransform)) * worldNorm;
	}

	float zoom = 1;
	if (viewportZoom != 0) zoom = viewportZoom;
	worldPos += vec4(offset, 0) / zoom;

	worldNorm = vec4(normalize(worldNorm.xyz), 1);

	vec4 modelPos = modelMatrix * worldPos;
	vec4 cameraPos = cameraMatrix * modelPos;
    vec4 viewportPos = viewportMatrix * cameraPos;

	
	vertexPosition = worldPos;
    vertexNormal = worldNorm;
    vertexTexture = texture;
	vertexMaterialColor = materialColor;
	vertexAccentColor = accentColor;
	vertexTintColor = tintColor;
    vertexFlags = flags;

	gl_Position = viewportPos;
	if (orthographic) gl_Position.z = zIndex / -100f;
}

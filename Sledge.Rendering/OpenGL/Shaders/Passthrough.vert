#version 120

// Be sure to keep these GLSL 330 parameters as a comment as they are needed for the preprocessor
/*
layout(location = 0) in vec3 position;
layout(location = 1) in vec3 normal;
layout(location = 2) in vec2 texture;
layout(location = 3) in vec4 materialColor;
layout(location = 4) in vec4 accentColor;
layout(location = 5) in vec4 pointColor;
layout(location = 6) in vec4 tintColor;
layout(location = 7) in uint flags;
layout(location = 8) in float zIndex;
layout(location = 9) in vec3 offset;
*/

attribute vec3 position;
attribute vec3 normal;
attribute vec2 texture;
attribute vec4 materialColor;
attribute vec4 accentColor;
attribute vec4 pointColor;
attribute vec4 tintColor;
attribute float fflags;
attribute float zIndex;
attribute vec3 offset;

varying vec4 vertexPosition;
varying vec4 vertexNormal;
varying vec2 vertexTexture;
varying vec4 vertexMaterialColor;
varying vec4 vertexAccentColor;
varying vec4 vertexPointColor;
varying vec4 vertexTintColor;
varying float vertexFlags;

uniform bool orthographic;
uniform mat4 selectionTransform;
uniform mat4 selectionTransformInverseTranspose;
uniform mat4 modelMatrix;
uniform mat4 viewportMatrix;
uniform mat4 cameraMatrix;
uniform vec2 viewportSize;
uniform float viewportZoom;

int FLAGS_INVISIBLE_PERSPECTIVE = 1;
int FLAGS_INVISIBLE_ORTHOGRAPHIC = 2;
int FLAGS_SELECTED = 4;
int FLAGS_NORMALISED = 8;

bool has_flag(int value, int flag)
{
    return mod(value, flag * 2) >= flag;
}

void main()
{
    int flags = int(floor(fflags));
	vec4 worldPos = vec4(position, 1);
    vec4 worldNorm = vec4(normal, 1);

	if (has_flag(flags, FLAGS_NORMALISED)) {
		worldPos *= vec4(viewportSize.x, viewportSize.y, 1, 1);
	}

    if (has_flag(flags, FLAGS_SELECTED)) {
		worldPos = selectionTransform * worldPos;
		worldNorm = selectionTransformInverseTranspose * worldNorm;
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
	vertexPointColor = pointColor;
	vertexTintColor = tintColor;
    vertexFlags = flags;

	gl_Position = viewportPos;
	if (orthographic) gl_Position.z = zIndex / -100;
}

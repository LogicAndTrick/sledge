#version 120

// Be sure to keep these GLSL 330 parameters as a comment as they are needed for the preprocessor
/*
layout(location = 0) in vec3 position;
layout(location = 1) in vec3 normal;
layout(location = 2) in vec2 texture;
layout(location = 3) in vec4 accentColor;
layout(location = 4) in vec4 tintColor;
layout(location = 5) in uint flags;

layout(location = 6) in int weight1;
layout(location = 7) in float weightValue1;
layout(location = 8) in int weight2;
layout(location = 9) in float weightValue2;
layout(location = 10) in int weight3;
layout(location = 11) in float weightValue3;
*/

attribute vec3 position;
attribute vec3 normal;
attribute vec2 texture;
attribute vec4 accentColor;
attribute vec4 tintColor;
attribute float fflags;

attribute float weight1;
attribute float weightValue1;
attribute float weight2;
attribute float weightValue2;
attribute float weight3;
attribute float weightValue3;

varying vec4 vertexPosition;
varying vec4 vertexNormal;
varying vec2 vertexTexture;
varying vec4 vertexAccentColor;
varying vec4 vertexTintColor;
varying float vertexFlags;

uniform mat4 selectionTransform;
uniform mat4 selectionTransformInverseTranspose;
uniform mat4 modelMatrix;
uniform mat4 viewportMatrix;
uniform mat4 cameraMatrix;

uniform mat4[128] animationTransforms;

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

    if (weightValue1 > 0) worldPos = animationTransforms[int(floor(weight1))] * worldPos;
    if (weightValue2 > 0) worldPos = animationTransforms[int(floor(weight2))] * worldPos;
    if (weightValue3 > 0) worldPos = animationTransforms[int(floor(weight3))] * worldPos;

    if (has_flag(flags, FLAGS_SELECTED)) {
		worldPos = selectionTransform * worldPos;
		worldNorm = selectionTransformInverseTranspose * worldNorm;
	}

	worldNorm = vec4(normalize(worldNorm.xyz), 1);

	vec4 modelPos = modelMatrix * worldPos;
	vec4 cameraPos = cameraMatrix * modelPos;
    vec4 viewportPos = viewportMatrix * cameraPos;
	
	vertexPosition = worldPos;
    vertexNormal = worldNorm;
    vertexTexture = texture;
	vertexAccentColor = accentColor;
	vertexTintColor = tintColor;
    vertexFlags = flags;

	gl_Position = viewportPos;
}

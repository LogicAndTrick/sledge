#version 120

layout(location = 0) in vec3 position;
layout(location = 1) in vec3 normal;
layout(location = 2) in vec2 texture;
layout(location = 3) in vec4 colour;
layout(location = 4) in float selected;

const vec3 light1direction = vec3(-1, -2, 3);
const vec3 light2direction = vec3(1, 2, 3);
const vec4 light1intensity = vec4(0.6, 0.6, 0.6, 1.0);
const vec4 light2intensity = vec4(0.3, 0.3, 0.3, 1.0);
const vec4 ambient = vec4(0.5, 0.5, 0.5, 1.0);

varying vec4 worldPosition;
varying vec4 worldNormal;
varying vec4 vertexLighting;
varying vec4 vertexColour;
varying vec2 texCoord;
varying float vertexSelected;

uniform mat4 modelViewMatrix;
uniform mat4 perspectiveMatrix;
uniform mat4 cameraMatrix;
uniform mat4 selectionTransform;
uniform mat4 inverseSelectionTransform;

void main()
{
    vec4 pos = vec4(position, 1);
    if (selected > 0.9) pos = selectionTransform * pos;
    vec4 modelPos = modelViewMatrix * pos;
    
	vec4 cameraPos = cameraMatrix * modelPos;
	gl_Position = perspectiveMatrix * cameraPos;

    vec4 npos = vec4(normal, 1);
    // http://www.arcsynthesis.org/gltut/Illumination/Tut09%20Normal%20Transformation.html
    if (selected > 0.9) npos = transpose(inverseSelectionTransform) * npos;
    vec3 normalPos = normalize(npos.xyz);
    npos = vec4(normalPos, 1);

    worldPosition = pos;
    worldNormal = npos;

    float incidence1 = dot(normalPos, light1direction);
    float incidence2 = dot(normalPos, light2direction);

    incidence1 = clamp(incidence1, 0, 1);
    incidence2 = clamp(incidence2, 0, 1);

	vertexColour = colour;
    vertexLighting = (vec4(1,1,1,1) * light1intensity * incidence1) * 0.5
                   + (vec4(1,1,1,1) * light2intensity * incidence2) * 0.5
                   + (vec4(1,1,1,1) * ambient);
    vertexLighting.w = 1.0; // Reset the alpha channel or transparency gets messed up later
    texCoord = texture;
    vertexSelected = selected;
}

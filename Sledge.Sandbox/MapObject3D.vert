#version 120

layout(location = 0) in vec3 position;
layout(location = 1) in vec3 normal;
layout(location = 2) in vec2 texture;
layout(location = 3) in vec4 colour;
layout(location = 4) in float selected;

const vec3 lightDirection = normalize(vec3(1, 2, 3));
const float lightIntensity = 0.5;
const float ambient = 0.5;

varying vec4 worldPosition;
varying vec4 worldNormal;
varying float vertexLighting;
varying vec4 vertexColour;
varying vec2 texCoord;
varying float vertexSelected;

uniform mat4 transformation;
uniform mat4 modelViewMatrix;
uniform mat4 perspectiveMatrix;
uniform mat4 cameraMatrix;
uniform mat4 selectionTransform;
uniform mat4 inverseSelectionTransform;

struct Light {
    vec3 position;
    vec3 ambient;
    //vec3 diffuse;
};
uniform Light lights[10];

void main()
{
    vec4 pos = vec4(position, 1);
    if (selected > 0.9) pos = selectionTransform * pos;
    vec4 modelPos = modelViewMatrix * pos * transformation;
    
	vec4 cameraPos = cameraMatrix * modelPos;
	gl_Position = perspectiveMatrix * cameraPos;

    vec4 npos = vec4(normal, 1);
    // http://www.arcsynthesis.org/gltut/Illumination/Tut09%20Normal%20Transformation.html
    if (selected > 0.9) npos = transpose(inverseSelectionTransform) * npos;
    vec3 normalPos = normalize(npos.xyz);
    npos = vec4(normalPos, 1);

    worldPosition = pos;
    worldNormal = npos;
    
	vertexColour = colour;
    float incidence = dot(normalPos, lightDirection);
    vertexLighting = lightIntensity * incidence + ambient;

    ///

    for (int i = 0; i < 10; i ++)
    {
        vec4 lPosition = modelViewMatrix * transformation * vec4(lights[i].position, 1.0);
        vec3 lVector = lPosition.xyz - pos.xyz;
    
        //float lDistance = 1.0;
        //if ( pointLightDistance[ i ] > 0.0 )
        //    lDistance = 1.0 - min( ( length( lVector ) / pointLightDistance[ i ] ), 1.0 );
        
        lVector = normalize( lVector );
        float dotProduct = dot( normalPos, lVector );
    
        vec3 pointLightWeighting = vec3( max( dotProduct, 0.0 ) );
    
        vertexLighting += lights[i].ambient * pointLightWeighting * 1.0;
    }

    vertexLighting = lightIntensity * incidence + ambient;

    ///
    
    texCoord = texture;
    vertexSelected = selected;
}

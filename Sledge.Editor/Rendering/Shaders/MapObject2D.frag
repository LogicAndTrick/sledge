#version 120

varying vec4 vertexColour;
varying float vertexSelected;

uniform bool drawSelectedOnly;
uniform bool drawUnselectedOnly;
uniform vec4 selectedColour;

void main()
{
    if (drawSelectedOnly && vertexSelected <= 0.9) discard;
    if (drawUnselectedOnly && vertexSelected > 0.9) discard;

	gl_FragColor = mix(vertexColour, selectedColour, vertexSelected);
}

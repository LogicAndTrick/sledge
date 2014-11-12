#version 120

varying vec4 worldPosition;
varying vec4 worldNormal;
varying vec4 vertexColour;
varying float vertexLighting;
varying vec2 texCoord;
varying float vertexSelected;

uniform bool isTextured;
uniform bool isLit;
uniform vec4 selectionColourMultiplier;
uniform bool showGrid;
uniform float gridSpacing;
uniform sampler2D currentTexture;

void main()
{
    vec4 outputColor;
	float lighting = vertexLighting;
	if (!isLit) lighting = 1;
    float alpha = vertexColour.w;

    if (isTextured) {
        vec4 texColour = texture2D(currentTexture, texCoord);
        outputColor = texColour * lighting;
        if (texColour.w < alpha) alpha = texColour.w;
    } else {
        outputColor = vertexColour * lighting;
    }
    if (vertexSelected > 0.9) {
        outputColor = outputColor * selectionColourMultiplier;
    }

    if (showGrid) {
        if (abs(worldNormal).x < 0.9999) outputColor = mix(outputColor, vec4(1, 0, 0, 1), step(mod(worldPosition.x, gridSpacing), 0.5));
        if (abs(worldNormal).y < 0.9999) outputColor = mix(outputColor, vec4(0, 1, 0, 1), step(mod(worldPosition.y, gridSpacing), 0.5));
        if (abs(worldNormal).z < 0.9999) outputColor = mix(outputColor, vec4(0, 0, 1, 1), step(mod(worldPosition.z, gridSpacing), 0.5));
    }
    outputColor.w = alpha;
    gl_FragColor = outputColor;
}

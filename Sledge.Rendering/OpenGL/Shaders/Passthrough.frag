#version 330

smooth in vec4 vertexPosition;
smooth in vec4 vertexNormal;
smooth in vec2 vertexTexture;
smooth in vec4 vertexMaterialColor;
smooth in vec4 vertexAccentColor;
smooth in vec4 vertexTintColor;
flat in uint vertexFlags;

uniform bool orthographic;
uniform bool useAccentColor;
uniform bool showGrid;
uniform float gridSpacing;
uniform sampler2D currentTexture;

out vec4 fragmentColor;

uint FLAGS_INVISIBLE_PERSPECTIVE = 1u << 0;
uint FLAGS_INVISIBLE_ORTHOGRAPHIC = 1u << 1;
uint FLAGS_SELECTED = 1u << 2;

void main()
{
    if (orthographic && (vertexFlags & FLAGS_INVISIBLE_PERSPECTIVE) == FLAGS_INVISIBLE_PERSPECTIVE) discard;
    if (!orthographic && (vertexFlags & FLAGS_INVISIBLE_ORTHOGRAPHIC) == FLAGS_INVISIBLE_ORTHOGRAPHIC) discard;

    vec3 tint = vec3(1, 1, 1) - ((vec3(1, 1, 1) - vertexTintColor.rgb) * vertexTintColor.a);

    fragmentColor = useAccentColor ? vertexAccentColor : texture(currentTexture, vertexTexture) * vertexMaterialColor;
	fragmentColor.rgb *= tint; // vertexTintColor.rgb * vertexTintColor.a;
	fragmentColor.a *= vertexMaterialColor.a;

    if (fragmentColor.a < 0.05) discard;
	
    if (showGrid) {
        if (abs(vertexNormal).x < 0.9999) fragmentColor = mix(fragmentColor, vec4(1, 0, 0, 1), step(mod(vertexPosition.x, gridSpacing), 0.1f));
        if (abs(vertexNormal).y < 0.9999) fragmentColor = mix(fragmentColor, vec4(0, 1, 0, 1), step(mod(vertexPosition.y, gridSpacing), 0.1f));
        if (abs(vertexNormal).z < 0.9999) fragmentColor = mix(fragmentColor, vec4(0, 0, 1, 1), step(mod(vertexPosition.z, gridSpacing), 0.1f));
    }
}

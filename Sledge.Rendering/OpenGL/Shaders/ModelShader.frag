#version 120

varying vec4 vertexPosition;
varying vec4 vertexNormal;
varying vec2 vertexTexture;
varying vec4 vertexAccentColor;
varying vec4 vertexTintColor;
varying float vertexFlags;

uniform bool orthographic;
uniform bool useAccentColor;
uniform sampler2D currentTexture;

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
    int flags = int(floor(vertexFlags + 0.5f));
    
    if (orthographic && has_flag(flags, FLAGS_INVISIBLE_PERSPECTIVE)) discard;
    if (!orthographic && has_flag(flags, FLAGS_INVISIBLE_ORTHOGRAPHIC)) discard;

    vec4 fragmentColor = orthographic || useAccentColor ? vertexAccentColor : texture2D(currentTexture, vertexTexture);
	fragmentColor.rgb *= vertexTintColor.rgb * vertexTintColor.a;
    
    gl_FragColor = fragmentColor;
}

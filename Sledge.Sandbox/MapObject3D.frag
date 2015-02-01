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


struct Light {
    vec3 position;
    vec3 ambient;
    //vec3 diffuse;
};
uniform Light lights[10];

void main()
{
    vec4 outputColor;
	vec4 lighting = vec4(0,0,0,0);
	if (!isLit) lighting = vec4(1,1,1,1);
    float alpha = vertexColour.w;

    ///

    for (int i = 0; i < 10; i ++)
    {
        vec4 lPosition = vec4(lights[i].position, 1.0);
        vec3 lVector = lPosition.xyz - worldPosition.xyz;
    
        //float lDistance = 1.0;
        //if ( pointLightDistance[ i ] > 0.0 )
        //    lDistance = 1.0 - min( ( length( lVector ) / pointLightDistance[ i ] ), 1.0 );
        
        lVector = normalize( lVector );
        float dotProduct = dot( worldNormal.xyz, lVector );
    
        vec4 pointLightWeighting = vec4( max( dotProduct, 0.0 ) );
    
        lighting += vec4(lights[i].ambient, 1) * pointLightWeighting;
    }

    ////




    if (isTextured) {
        vec4 texColour = texture2D(currentTexture, texCoord);
        outputColor = texColour * lighting;
        if (texColour.w < alpha) alpha = texColour.w;
    } else {
        outputColor = vertexColour + lighting;
    }
    if (vertexSelected > 0.9) {
        outputColor = outputColor * selectionColourMultiplier;
    } else {
        // outputColor = vec4(1,1,1,1);
    }

    if (showGrid) {
        if (abs(worldNormal).x < 0.9999) outputColor = mix(outputColor, vec4(1, 0, 0, 1), step(mod(worldPosition.x, gridSpacing), 0.5));
        if (abs(worldNormal).y < 0.9999) outputColor = mix(outputColor, vec4(0, 1, 0, 1), step(mod(worldPosition.y, gridSpacing), 0.5));
        if (abs(worldNormal).z < 0.9999) outputColor = mix(outputColor, vec4(0, 0, 1, 1), step(mod(worldPosition.z, gridSpacing), 0.5));
    }
    outputColor.w = alpha;
    gl_FragColor = outputColor;
}

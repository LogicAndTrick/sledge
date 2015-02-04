#version 300

smooth in vec4 vertexPosition;
smooth in vec4 vertexColor;

out vec4 fragmentColor;

void main()
{
    fragmentColor = vertexColor;
}

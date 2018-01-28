#version 330 core

uniform mat4 Projection;
uniform mat4 ModelView;

layout (location = 0) in vec3 in_Position;
layout (location = 1) in vec3 in_Normal;
layout (location = 2) in vec2 in_TexCoord;
layout (location = 3) in vec4 in_Color;

out vec4 vertexColor;
out vec2 texCoord;

void main()
{
	gl_Position = Projection * ModelView * vec4(in_Position, 1.0);
	vertexColor = in_Color;
	texCoord = in_TexCoord;
}
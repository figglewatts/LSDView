#version 330 core

in vec4 vertexColor;
in vec2 texCoord;

out vec4 out_FragColor;

uniform sampler2D uTexture;

void main()
{
	vec4 texColor = texture(uTexture, texCoord);
	vec4 vertColor = vertexColor;

	if (texColor.a < 0.1)
		discard;
	
	out_FragColor = vertexColor * texColor;
}
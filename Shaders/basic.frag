#version 330 core

in vec4 vertexColor;
in vec2 texCoord;

out vec4 out_FragColor;

uniform sampler2D uTexture;
uniform vec4 ColorTint = vec4(1.0, 1.0, 1.0, 1.0);

void main()
{
	vec4 texColor = texture(uTexture, texCoord);
	vec4 vertColor = vertexColor;

	if (texColor.a < 0.1)
		discard;
	
	out_FragColor = vertexColor * texColor * ColorTint;
}
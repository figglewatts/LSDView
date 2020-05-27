#version 330 core

out vec4 out_FragColor;

in vec2 texCoord;

uniform sampler2D uTexture;

void main()
{
	out_FragColor = texture(uTexture, texCoord);
}
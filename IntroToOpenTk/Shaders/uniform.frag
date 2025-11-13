#version 330 core

in vec3 vertexColor;

uniform float uTime;
uniform vec3 uTint;

out vec4 FragColor;

void main()
{
	vec3 color = vertexColor;
	color.r = color.r * (sin(uTime) * 0.5 + 0.5);
	color.g = color.g * ((sin(uTime) + 2.0) * 0.5 + 0.5);
	color.b = color.b * (sin(uTime + 4.0) * 0.5 + 0.5);

	color = color * uTint;

	FragColor = vec4(color, 1.0);
}

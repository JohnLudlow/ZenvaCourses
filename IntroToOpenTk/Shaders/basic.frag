#version 330 core

in vec3 vertexColor;
out vec4 FragColor;

void main()
{
    // Set the final fragment color by combining RGB vertex color with full opacity
    // vec4 constructor: (red, green, blue, alpha)
    // - vertexColor.rgb: Use the interpolated RGB color from vertex shader
    // - 1.0: Set alpha (transparency) to 1.0 (fully opaque)
    FragColor = vec4(vertexColor, 1.0);
}

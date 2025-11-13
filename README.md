# IntroToOpenTk

A small teaching project that demonstrates using OpenTK (OpenGL) with .NET 8 to render a simple triangle and manage GLSL shaders. The sample shows window creation, shader compilation/linking, vertex buffers/arrays, and basic input handling.

## Features
- Creates an OpenGL window using OpenTK 4.9.4
- Compiles and links vertex and fragment shaders
- Renders a colored triangle with two shader variants (basic and uniform)
- Keyboard controls:
  - `Space` — toggle between basic and uniform shader
  - `Esc` — close the window
- Shader source files shipped in `Shaders/` and copied to output

## Project layout
- `Program.cs` — application entry point
- `Engine/` — game loop, input handling, shader and triangle setup
- `Graphics/` — shader helper classes, buffer/VAO wrappers
- `Shaders/` — GLSL source files (`basic.vert`, `basic.frag`, `uniform.frag`)

## Prerequisites
- .NET 8 SDK
- A system with an OpenGL-compatible GPU and drivers

## Build and run
1. Restore and build:
   - `dotnet build`
2. Run:
   - `dotnet run --project IntroToOpenTk`

Shaders are included in the output directory (see project file), so you can edit `Shaders/*.vert` and `Shaders/*.frag` and re-run to test shader changes.

## Debugging tips
- If shader compilation fails, inspect the console output for the GLSL compiler log.
- Ensure shader source text (not file paths) is passed to the compiler — the project includes a helper to load shader files correctly.
- Trim BOM and whitespace from shader files if unexpected syntax errors appear.

## Contributing
Contributions and bug reports are welcome. Open a PR or issue describing the change and steps to reproduce.

## License
This project is provided as-is for learning purposes. Use and modify freely; add a license file if you intend to redistribute.
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace JohnLudlow.ZenvaCourses.IntroToOpenTk.Graphics;

public class ShaderCompilationException : Exception
{
  public ShaderCompilationException(string message) : base(message)
  {
  }

  public ShaderCompilationException()
  {
  }

  public ShaderCompilationException(string message, Exception innerException) : base(message, innerException)
  {
  }
}

internal class Shader : IDisposable
{
  public int Handle { get; private set; }

  private readonly Dictionary<string, int> _uniformLocations = [];

  private bool _disposed;

  /// <summary>
  /// Initializes a new instance of the Shader class by compiling and linking the specified vertex and fragment shader
  /// source code.
  /// </summary>
  /// <remarks>This constructor compiles the provided shader sources and links them into a shader program.
  /// If compilation or linking fails, an exception may be thrown. Uniform locations are cached for efficient access.
  /// The created shader program is ready for use in rendering operations.</remarks>
  /// <param name="vertexSource">The source code for the vertex shader. Must be valid GLSL code; cannot be null or empty.</param>
  /// <param name="fragmentSource">The source code for the fragment shader. Must be valid GLSL code; cannot be null or empty.</param>
  public Shader(string vertexSource, string fragmentSource)
  {
    var vertexShader = GL.CreateShader(ShaderType.VertexShader);
    GL.ShaderSource(vertexShader, vertexSource);
    GL.CompileShader(vertexShader);
    CheckShaderCompilation(vertexShader, "vertex");

    var fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
    GL.ShaderSource(fragmentShader, fragmentSource);
    GL.CompileShader(fragmentShader);
    CheckShaderCompilation(fragmentShader, "fragment");

    Handle = GL.CreateProgram();
    GL.AttachShader(Handle, vertexShader);
    GL.AttachShader(Handle, fragmentShader);
    GL.LinkProgram(Handle);

    CheckProgramLinking(Handle);

    GL.DetachShader(Handle, vertexShader);
    GL.DetachShader(Handle, fragmentShader);
    GL.DeleteShader(vertexShader);
    GL.DeleteShader(fragmentShader);

    GL.GetProgram(Handle, GetProgramParameterName.ActiveUniforms, out var numberOfUniforms);

    for (var i = 0; i < numberOfUniforms; i++)
    {
      var name = GL.GetActiveUniform(Handle, i, out _, out _);
      var location = GL.GetUniformLocation(Handle, name);
      _uniformLocations.Add(name, location);
    }
  }

  /// <summary>
  /// Sets the current OpenGL rendering state to use this shader program for subsequent draw operations.
  /// </summary>
  /// <remarks>Call this method before issuing draw calls to ensure that the correct shader program is
  /// active. This method affects all rendering until another shader program is set. This method is not thread-safe;
  /// ensure that OpenGL context usage is properly synchronized in multithreaded scenarios.</remarks>
  public void Use()
  {
    Console.WriteLine($"Shader Program :: {Handle} :: {nameof(Use)}");
    GL.UseProgram(Handle);
  }

  /// <summary>
  /// Retrieves the location of a uniform variable within the shader program by its name.
  /// </summary>
  /// <remarks>If the uniform variable is not found or not active, a warning is written to the console and
  /// -1 is returned. The method caches uniform locations for subsequent calls to improve performance.</remarks>
  /// <param name="name">The name of the uniform variable whose location is to be retrieved. Cannot be null.</param>
  /// <returns>The location of the specified uniform variable, or -1 if the uniform is not found or not active in the shader
  /// program.</returns>
  public int GetUniformLocation(string name)
  {
    if (_uniformLocations.TryGetValue(name, out var location))
    {
      return location;
    }

    location = GL.GetUniformLocation(Handle, name);

    if (location == -1)
    {
      Console.WriteLine($"Warning: uniform '{name}' not found in shader or not active.");
    }
    else
    {
      _uniformLocations.Add(name, location);
    }

    return location;
  }

  /// <summary>
  /// Sets the value of a boolean uniform variable in the current shader program.
  /// </summary>
  /// <remarks>This method updates the specified uniform variable in the currently active shader program. If the
  /// uniform does not exist, no action is taken. Ensure that the shader program is in use before calling this
  /// method.</remarks>
  /// <param name="name">The name of the uniform variable to set. Cannot be null or empty.</param>
  /// <param name="value">The boolean value to assign to the uniform variable.</param>
  public void SetBool(string name, bool value)
  {
    Use();
    Console.WriteLine($"Shader Program :: {Handle} :: {nameof(SetBool)} ({name}, {value})");
    GL.Uniform1(GetUniformLocation(name), value ? 1 : 0);
  }

  /// <summary>
  /// Sets the value of an integer uniform variable in the current shader program.
  /// </summary>
  /// <remarks>This method updates the specified uniform variable for the currently active shader program. If
  /// the uniform name does not exist in the program, the operation has no effect.</remarks>
  /// <param name="name">The name of the uniform variable to set. Cannot be null or empty.</param>
  /// <param name="value">The integer value to assign to the uniform variable.</param>
  public void SetInt(string name, int value)
  {
    Use();
    Console.WriteLine($"Shader Program :: {Handle} :: {nameof(SetInt)} ({name}, {value})");
    GL.Uniform1(GetUniformLocation(name), value);
  }

  /// <summary>
  /// Sets the value of a float uniform variable in the current shader program.
  /// </summary>
  /// <remarks>This method activates the shader program before updating the uniform value. If the specified
  /// uniform name does not exist in the shader, the operation has no effect.</remarks>
  /// <param name="name">The name of the uniform variable to set. Cannot be null or empty.</param>
  /// <param name="value">The float value to assign to the specified uniform variable.</param>
  public void SetFloat(string name, float value)
  {
    Use();
    Console.WriteLine($"Shader Program :: {Handle} :: {nameof(SetFloat)} ({name}, {value})");
    GL.Uniform1(GetUniformLocation(name), value);
  }

  /// <summary>
  /// Sets the value of a 2-component vector uniform variable in the shader program.
  /// </summary>
  /// <remarks>This method activates the shader program before updating the uniform variable. If the specified
  /// uniform name does not exist in the shader, the operation has no effect.</remarks>
  /// <param name="name">The name of the uniform variable to set. Cannot be null or empty.</param>
  /// <param name="value">The vector value to assign to the uniform variable.</param>
  public void SetVector2(string name, Vector2 value)
  {
    Use();
    Console.WriteLine($"Shader Program :: {Handle} :: {nameof(SetVector2)} ({name}, {value})");
    GL.Uniform2(GetUniformLocation(name), value);
  }

  /// <summary>
  /// Sets the value of a 3-component vector uniform variable in the shader program.
  /// </summary>
  /// <remarks>This method must be called after the shader program is in use. If the specified uniform name does
  /// not exist in the shader, no action is taken.</remarks>
  /// <param name="name">The name of the uniform variable to set. Cannot be null or empty.</param>
  /// <param name="value">The <see cref="Vector3"/> value to assign to the uniform variable.</param>
  public void SetVector3(string name, Vector3 value)
  {
    Use();
    Console.WriteLine($"Shader Program :: {Handle} :: {nameof(SetVector3)} ({name}, {value})");
    GL.Uniform3(GetUniformLocation(name), value);
  }

  /// <summary>
  /// Sets the value of a 4-component vector uniform variable in the shader program.
  /// </summary>
  /// <remarks>This method activates the shader program before updating the uniform value. If the specified
  /// uniform name does not exist in the shader, the operation has no effect.</remarks>
  /// <param name="name">The name of the uniform variable to set. Cannot be null or empty.</param>
  /// <param name="value">The <see cref="Vector4"/> value to assign to the uniform variable.</param>
  public void SetVector4(string name, Vector4 value)
  {
    Use();
    Console.WriteLine($"Shader Program :: {Handle} :: {nameof(SetVector4)} ({name}, {value})");
    GL.Uniform4(GetUniformLocation(name), value);
  }

  /// <summary>
  /// Sets the value of a 4x4 matrix uniform variable in the shader program.
  /// </summary>
  /// <remarks>This method activates the shader program before updating the uniform variable. If the specified
  /// uniform name does not exist in the shader, the operation has no effect.</remarks>
  /// <param name="name">The name of the uniform variable to set. Cannot be null or empty.</param>
  /// <param name="value">The 4x4 matrix value to assign to the uniform variable.</param>
  public void SetMatrix4(string name, Matrix4 value)
  {
    Use();
    Console.WriteLine($"Shader Program :: {Handle} :: {nameof(SetMatrix4)} ({name}, {value})");
    GL.UniformMatrix4(GetUniformLocation(name), false, ref value);
  }

  /// <summary>
  /// Loads a shader by reading the source code from the specified vertex and fragment shader files.
  /// </summary>
  /// <remarks>Both files must exist and contain valid shader source code. This method reads the entire
  /// contents of each file and passes them to the Shader constructor. If either file does not exist or cannot be
  /// read, an exception will be thrown.</remarks>
  /// <param name="vertexPath">The file path to the vertex shader source code. Cannot be null or empty.</param>
  /// <param name="fragmentPath">The file path to the fragment shader source code. Cannot be null or empty.</param>
  /// <returns>A Shader instance created from the contents of the specified vertex and fragment shader files.</returns>
  public static Shader LoadShaderFromFiles(string vertexPath, string fragmentPath)
  {
    var vertexSource = File.ReadAllText(vertexPath);
    var fragmentSource = File.ReadAllText(fragmentPath);
    return new Shader(vertexSource, fragmentSource);
  }

  private static void CheckShaderCompilation(int shader, string type)
  {
    GL.GetShader(shader, ShaderParameter.CompileStatus, out var success);
    if (success == 0)
    {
      var infoLog = GL.GetShaderInfoLog(shader);
      Console.WriteLine($"Error compiling {type} shader: {infoLog}");
      throw new ShaderCompilationException(infoLog);
    }
  }

  private static void CheckProgramLinking(int program)
  {
    GL.GetProgram(program, GetProgramParameterName.LinkStatus, out var success);
    if (success == 0)
    {
      var infoLog = GL.GetProgramInfoLog(program);
      Console.WriteLine($"Error linking shader program: {infoLog}");
      throw new ShaderCompilationException(infoLog);
    }
  }

  /// <summary>
  /// Releases unmanaged OpenGL resources held by this shader instance.
  /// </summary>
  /// <remarks>
  /// Deletes the OpenGL program identified by <see cref="Handle"/> and marks the instance as disposed.
  /// This method is idempotent (safe to call multiple times) and does not throw when called repeatedly.
  /// It must be invoked on a thread that has a current OpenGL context; calling it without a valid context
  /// may produce undefined behavior. After disposal the shader must not be used for rendering or for retrieving
  /// uniform locations.
  /// </remarks>
  public void Dispose()
  {
    if (!_disposed)
    {
      GL.DeleteProgram(Handle);
      _disposed = true;
      GC.SuppressFinalize(this);
    }
  }

  /// <summary>
  /// Finalizes the Shader instance and releases unmanaged resources before the object is reclaimed by garbage
  /// collection.
  /// </summary>
  /// <remarks>This destructor ensures that unmanaged resources are properly released if Dispose was not called
  /// explicitly. It is recommended to call Dispose manually to free resources deterministically.</remarks>
  ~Shader()
  {
    if (!_disposed)
    {
      Dispose();
    }
  }
}

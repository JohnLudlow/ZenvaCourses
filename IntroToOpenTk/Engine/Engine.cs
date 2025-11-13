using JohnLudlow.ZenvaCourses.IntroToOpenTk.Graphics;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace JohnLudlow.ZenvaCourses.IntroToOpenTk.Engine;

public class Engine : IDisposable
{
  private readonly GameWindow _window;
  private VertexArray _vertexArray;
  private VertexBuffer _vertexBuffer;
  private bool _disposed = false;

  private Shader _basicShader;
  private Shader _uniformShader;
  private bool _useBasicShader = true;
  private float _time = 0f;

  public Engine(GameWindow window)
  {
    _window = window;
    _window.Load += OnLoad;
    _window.Resize += OnResize;
    _window.UpdateFrame += OnUpdateFrame;
    _window.RenderFrame += OnRenderFrame;
    _window.KeyDown += OnKeyDown;
  }

  private void OnKeyDown(KeyboardKeyEventArgs args)
  {
    if (args.Key == Keys.Space)
    {
      _useBasicShader = !_useBasicShader;
      Console.WriteLine($"Switched to {(_useBasicShader ? "Basic Shader" : "Uniform Shader")}");
    }

    if (args.Key == Keys.Escape)
    {
      _window.Close();
    }
  }

  private void OnLoad()
  {
    Console.WriteLine($"OpenGL Version: {GL.GetString(StringName.Version)}");
    GL.ClearColor(0.2f, 0.3f, 0.3f, 1f);
    CreateShaders();
    CreateTriangle();

    Console.WriteLine("\tPress SPACE to switch shaders.");
    Console.WriteLine("\tPress ESC to exit.");
  }

  private void CreateTriangle()
  {
    _vertexArray = new VertexArray();
    _vertexArray.Bind();

    var vertices = new float[]
    {
      // Positions        // Colors      
      -0.5f,  0.5f,       1f, 0f, 0f,  // Top
      -0.5f, -0.5f,       0f, 1f, 0f,  // Bottom Left
       0.5f, -0.5f,       0f, 0f, 1f,  // Bottom Right
    };

    _vertexBuffer = new VertexBuffer();
    _vertexBuffer.Bind();
    _vertexBuffer.SetData(vertices);

    GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
    GL.EnableVertexAttribArray(0);

    GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 2 * sizeof(float));
    GL.EnableVertexAttribArray(1);

    _vertexArray.Unbind();
  }

  private void CreateShaders()
  {
    Directory.CreateDirectory("Shaders");
    
    var vertexShaderPath = "Shaders/basic.vert";
    var fragmentShaderPath = "Shaders/basic.frag";
    var uniformFragmentShaderPath = "Shaders/uniform.frag";

    if (!File.Exists(vertexShaderPath) || !File.Exists(fragmentShaderPath) || !File.Exists(uniformFragmentShaderPath))
    {
      throw new FileNotFoundException("Shader files not found in the Shaders directory.");
    }

    try 
    {
      _basicShader = Shader.LoadShaderFromFiles(vertexShaderPath, fragmentShaderPath);
      _uniformShader = Shader.LoadShaderFromFiles(vertexShaderPath, uniformFragmentShaderPath);
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Error creating shaders: {ex.Message}");
      throw;
    }
  }

  private void OnRenderFrame(FrameEventArgs args)
  {
    GL.Clear(ClearBufferMask.ColorBufferBit);

    DrawTriangle();

    _window.SwapBuffers();
  }

  private void DrawTriangle()
  {
    if (_useBasicShader)
    {
      _basicShader.Use();
    }
    else
    {
      _uniformShader.Use();
      _uniformShader.SetFloat("uTime", _time);
      _uniformShader.SetVector3("uTint", new Vector3(1.0f, 1.0f, 1.0f));
    }

    _vertexArray.Bind();
    GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
   
    _vertexArray.Unbind();
  }

  private void OnUpdateFrame(FrameEventArgs args)
  {
    _time += (float)args.Time;
  }

  private void OnResize(ResizeEventArgs args)
  {
    GL.Viewport(0, 0, args.Width, args.Height);
  }

  protected virtual void Dispose(bool disposing)
  {
    if (!_disposed)
    {
      if (disposing)
      {
        _basicShader?.Dispose();
        _uniformShader?.Dispose();          
        _vertexBuffer?.Dispose();
        _vertexArray?.Dispose();
      }
      _disposed = true;
    }
  }

  public void Dispose()
  {
    Dispose(true);
    GC.SuppressFinalize(this);
  }

  public void Run()
  {
    _window.Run();
  }
}

using OpenTK.Graphics.OpenGL4;

namespace JohnLudlow.ZenvaCourses.IntroToOpenTk.Graphics;

internal class VertexBuffer : IDisposable
{
  private readonly int _handle;
  private bool _disposed = false;

  public int Handle => _handle;

  public VertexBuffer()
  {
    _handle = GL.GenBuffer();
  }

  public void Bind() => GL.BindBuffer(BufferTarget.ArrayBuffer, _handle);

  public static void Unbind() => GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

  public void SetData(float[] data, BufferUsageHint usageHint = BufferUsageHint.StaticDraw)
  {
    Bind();
    GL.BufferData(BufferTarget.ArrayBuffer, data.Length * sizeof(float), data, usageHint);
  }

  public void EnableAttribute(int index)
  {
    Bind();
    GL.EnableVertexAttribArray(index);
  }

  public void DisableAttribute(int index)
  {
    Bind();
    GL.DisableVertexAttribArray(index);
  }

  public void Dispose()
  {
    if (!_disposed)
    {
      GL.DeleteBuffer(_handle);
      _disposed = true;
      GC.SuppressFinalize(this);
    }
  }

  ~VertexBuffer()
  {
    if (!_disposed)
    {
      Console.WriteLine($"WARNING: {nameof(VertexBuffer)} was not disposed properly.");
    }
  }
}

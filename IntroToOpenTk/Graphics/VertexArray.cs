using OpenTK.Graphics.OpenGL4;

namespace JohnLudlow.ZenvaCourses.IntroToOpenTk.Graphics;

  internal class VertexArray : IDisposable
  {
      private readonly int _handle;
      private bool _disposed = false;

      public int Handle => _handle;

      public VertexArray()
      {
          _handle = GL.GenVertexArray();
      }

      public void Bind()
      {
          GL.BindVertexArray(_handle);
      }

      public void Unbind()
      {
          GL.BindVertexArray(0);
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
              GL.DeleteVertexArray(_handle);
              _disposed = true;
              GC.SuppressFinalize(this);
          }
      }

      ~VertexArray()
      {
          if (!_disposed)
          {
              Console.WriteLine($"WARNING: {nameof(VertexArray)} was not disposed properly.");
          }
      }
  }

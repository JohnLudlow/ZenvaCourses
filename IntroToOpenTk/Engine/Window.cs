using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace JohnLudlow.ZenvaCourses.IntroToOpenTk.Engine;

  public static class Window
  {
      public static GameWindow Create(int width, int height, string title)
      {
          ArgumentOutOfRangeException.ThrowIfNegativeOrZero(width);
          ArgumentOutOfRangeException.ThrowIfNegativeOrZero(height);
          ArgumentException.ThrowIfNullOrWhiteSpace(title);

          var gameWindowSettings = GameWindowSettings.Default;
          var nativeWindowSettings = new NativeWindowSettings()
          {
              ClientSize = new OpenTK.Mathematics.Vector2i(width, height),
              Title = title,
              Flags = ContextFlags.ForwardCompatible
          };

          var window = new GameWindow(gameWindowSettings, nativeWindowSettings);
          window.CenterWindow();
          return window;
      }
  }

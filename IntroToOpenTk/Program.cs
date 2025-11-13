using JohnLudlow.ZenvaCourses.IntroToOpenTk.Engine;

namespace JohnLudlow.ZenvaCourses.IntroToOpenTk;

internal class Program
{
    private static void Main(string[] args)
    {
        var window = Window.Create(800, 600, "Intro to OpenTK");

        using var engine = new Engine.Engine(window);
        engine.Run();
    }
}
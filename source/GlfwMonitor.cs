using TkGlfw = OpenTK.Windowing.GraphicsLibraryFramework;

namespace ChaosFramework.Platform.Glfw
{
    unsafe class GlfwMonitor(TkGlfw.Monitor* monitor) : Monitor
    {
        readonly TkGlfw.VideoMode vm = *TkGlfw.GLFW.GetVideoMode(monitor);

        uint Monitor.width => (uint)vm.Width;
        uint Monitor.height => (uint)vm.Height;
        Math.Vectors.Vector2i Monitor.position => 0; // only primary monitor supported right now
        string Monitor.deviceName => null;
    }
}

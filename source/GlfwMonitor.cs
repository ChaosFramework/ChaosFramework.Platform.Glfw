using TkGlfw = OpenTK.Windowing.GraphicsLibraryFramework;

namespace ChaosFramework.Platform.Glfw
{
    public unsafe class GlfwMonitor : Monitor
    {
        readonly TkGlfw.VideoMode vm;

        public uint width => (uint)vm.Width;
        public uint height => (uint)vm.Height;
        public Math.Vectors.Vector2i position => 0; // only primary monitor supported right now
        public string deviceName { get; }

        internal GlfwMonitor(TkGlfw.Monitor* monitor)
        {
            vm = *TkGlfw.GLFW.GetVideoMode(monitor);
            deviceName =  TkGlfw.GLFW.GetMonitorName(monitor);
        }
    }
}

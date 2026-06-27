using TkGlfw = OpenTK.Windowing.GraphicsLibraryFramework;

namespace ChaosFramework.Platform.Glfw
{
    public unsafe class GlfwMonitor : Monitor
    {
        internal readonly TkGlfw.Monitor* monitor;
        readonly TkGlfw.VideoMode vm;

        public uint width => (uint)vm.Width;
        public uint height => (uint)vm.Height;
        public Math.Vectors.Vector2i position
        {
            get
            {
                TkGlfw.GLFW.GetMonitorPos(monitor, out int x, out int y);
                return new (x, y);
            }
        }

        public string deviceName { get; }

        internal GlfwMonitor(TkGlfw.Monitor* monitor)
        {
            this.monitor = monitor;
            vm = *TkGlfw.GLFW.GetVideoMode(monitor);
            deviceName =  TkGlfw.GLFW.GetMonitorName(monitor);
        }
    }
}

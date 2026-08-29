using System;
using TkGlfw = OpenTK.Windowing.GraphicsLibraryFramework;

namespace ChaosFramework.Platform.Glfw
{
    public unsafe class GlfwMonitor : Monitor
    {
        /// <summary>
        ///     Persistent monitor handle until the monitor is disconnected.
        ///     When disconnecting and reconnecting the same physical monitor
        ///     it may not receive the same handle again.
        /// </summary>
        /// <seealso href="https://www.glfw.org/docs/latest/monitor_guide.html#monitor_object"/>
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

        public override bool Equals(object other)
            => Equals(other as GlfwMonitor);

        public bool Equals(GlfwMonitor other)
            => other != null && monitor == other.monitor;

        public override int GetHashCode()
            => ((IntPtr)monitor).GetHashCode();
    }
}

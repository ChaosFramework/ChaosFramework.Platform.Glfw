using System;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using TkGlfw = OpenTK.Windowing.GraphicsLibraryFramework;

namespace ChaosFramework.Platform.Glfw
{
    public unsafe class GlfwFullscreen : Fullscreen
    {
        public readonly NativeWindow window;

        public Monitor monitor { get; }
        public string title { get; set; }

        public uint width => monitor.width;
        public uint height => monitor.height;
        public Math.Vectors.Vector2i position => monitor.position;

        internal GlfwFullscreen(string title, GlfwMonitor monitor)
        {
            this.monitor = monitor ?? throw new ArgumentException(nameof(Monitor), $"Monitor must be a {nameof(GlfwMonitor)}.");
            window = new NativeWindow(new NativeWindowSettings()
            {
                CurrentMonitor = new MonitorHandle(new IntPtr(monitor.monitor)),
                Title = this.title = title,
                WindowState = WindowState.Fullscreen,
                StartVisible = true,
                APIVersion = new Version(3, 3)
            });
            TkGlfw.GLFW.MakeContextCurrent(window.WindowPtr);
            TkGlfw.GLFW.ShowWindow(window.WindowPtr);
            TkGlfw.GLFW.SetInputMode(window.WindowPtr, TkGlfw.CursorStateAttribute.Cursor, TkGlfw.CursorModeValue.CursorDisabled);
        }

        void PresentationContext.Present()
        {
            TkGlfw.GLFW.MakeContextCurrent(window.WindowPtr);
            TkGlfw.GLFW.SwapBuffers(window.WindowPtr);
        }
    }
}

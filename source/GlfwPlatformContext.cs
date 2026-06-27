using System.Linq;
using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using TkGlfw = OpenTK.Windowing.GraphicsLibraryFramework;

namespace ChaosFramework.Platform.Glfw
{
    public unsafe class GlfwPlatformContext
        : PlatformContext
        , GlContext
    {
        public event Action Terminate;

        bool terminated = false;
        GlfwFullscreen fullscreen = null;

        Overhead PlatformContext.messageQueue => PerformOverhead;
        GlContext PlatformContext.glContext => this;

        public GlfwMonitor PrimaryMonitor
        {
            get
            {
                TkGlfw.Monitor* primary = TkGlfw.GLFW.GetPrimaryMonitor();
                return primary == null
                    ? new GlfwMonitor(primary)
                    : EnumerateMonitors().FirstOrDefault();
            }
        }

        Monitor PlatformContext.PrimaryMonitor => PrimaryMonitor;

        public GlfwPlatformContext()
        {
            TkGlfw.GLFW.Init();
        }

        public GlfwFullscreen CreateFullscreen(string title, GlfwMonitor monitor)
            => fullscreen == null
                ? fullscreen =  new GlfwFullscreen(title, monitor)
                : throw new NotSupportedException("Only one monitor supported right now")
                ;

        Fullscreen PlatformContext.CreateFullscreen(string title, Monitor monitor)
            => CreateFullscreen(title, monitor as GlfwMonitor);

        Window PlatformContext.CreateWindow(string title)
            => throw new NotImplementedException();

        void GlContext.Init()
            => GL.LoadBindings(new TkGlfw.GLFWBindingsContext());

        void PerformOverhead()
        {
            TkGlfw.GLFW.PollEvents();
            if (fullscreen != null)
                if (TkGlfw.GLFW.WindowShouldClose(fullscreen.window.WindowPtr) && !terminated)
                {
                    terminated = true;
                    Terminate?.Invoke();
                }
        }

        public IEnumerable<GlfwMonitor> EnumerateMonitors()
        {
            TkGlfw.Monitor** tkMonitors = TkGlfw.GLFW.GetMonitorsRaw(out int numMonitors);
            GlfwMonitor[] monitors = new GlfwMonitor[numMonitors];
            for (int i = 0; i < numMonitors; ++i)
                monitors[i] = new GlfwMonitor(tkMonitors[i]);

            return monitors;
        }

        IEnumerable<Monitor> PlatformContext.EnumerateMonitors()
            => EnumerateMonitors();
    }
}

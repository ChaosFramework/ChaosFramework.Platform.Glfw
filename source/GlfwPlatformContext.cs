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

        readonly HashSet<GlfwMonitor> knownMonitors = [];

        // TODO: remove when closed
        readonly Dictionary<GlfwMonitor, GlfwFullscreen> fullscreen = [];

        bool terminated = false;

        Overhead PlatformContext.messageQueue => PerformOverhead;
        GlContext PlatformContext.glContext => this;

        public GlfwMonitor PrimaryMonitor
        {
            get
            {
                TkGlfw.Monitor* primary = TkGlfw.GLFW.GetPrimaryMonitor();
                return primary == null
                    ? GetOrCreateMonitor(primary)
                    : EnumerateMonitors().FirstOrDefault();
            }
        }

        Monitor PlatformContext.PrimaryMonitor => PrimaryMonitor;

        public GlfwPlatformContext()
        {
            TkGlfw.GLFW.Init();
        }

        public GlfwFullscreen CreateFullscreen(string title, GlfwMonitor monitor)
            => fullscreen.ContainsKey(monitor)
                ? throw new InvalidOperationException("More than one fullscreen window per monitor doesn't make sense.")
                : fullscreen[monitor] = new GlfwFullscreen(title, monitor)
                ;

        Fullscreen PlatformContext.CreateFullscreen(string title, Monitor monitor)
            => CreateFullscreen(title, monitor as GlfwMonitor);

        Window PlatformContext.CreateWindow(string title)
            => throw new NotImplementedException();

        void GlContext.Init()
            => GL.LoadBindings(new TkGlfw.GLFWBindingsContext());

        void GlContext.MakeCurrent(PresentationContext context)
        {
            if (context is GlfwFullscreen fs)
                TkGlfw.GLFW.MakeContextCurrent(fs.window.WindowPtr);
            else
                throw new ArgumentException(nameof(context));
        }

        void PerformOverhead()
        {
            TkGlfw.GLFW.PollEvents();
            foreach(GlfwFullscreen fs in fullscreen.Values)
                if (TkGlfw.GLFW.WindowShouldClose(fs.window.WindowPtr) && !terminated)
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
                monitors[i] = GetOrCreateMonitor(tkMonitors[i]);

            return monitors;
        }

        IEnumerable<Monitor> PlatformContext.EnumerateMonitors()
            => EnumerateMonitors();

        GlfwMonitor GetOrCreateMonitor(TkGlfw.Monitor* monitor)
        {
            GlfwMonitor newMonitor = new(monitor);
            if (knownMonitors.TryGetValue(newMonitor, out GlfwMonitor knownMonitor))
                return knownMonitor;

            knownMonitors.Add(newMonitor);
            return newMonitor;
        }
    }
}

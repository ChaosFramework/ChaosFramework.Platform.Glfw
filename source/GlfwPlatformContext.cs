using System;
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

        public GlfwPlatformContext()
        {
            TkGlfw.GLFW.Init();
        }

        public GlfwFullscreen CreateFullscreen(string title)
            => fullscreen == null
                ? fullscreen =  new GlfwFullscreen(title)
                : throw new NotSupportedException("Only one monitor supported right now")
                ;

        Fullscreen PlatformContext.CreateFullscreen(string title)
            => CreateFullscreen(title);

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
    }
}

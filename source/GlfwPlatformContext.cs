using ChaosFramework.Collections;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.Common;
using OpenTK.Mathematics;
using System;
using TkGlfw = OpenTK.Windowing.GraphicsLibraryFramework;

namespace ChaosFramework.Platform.Glfw
{
    public unsafe class GlfwPlatformContext
        : PlatformContext
        , GlContext
    {
        public class GlfwWindow : Window
        {
            public readonly NativeWindow window;

            int w, h;
            int Window.width => w;
            int Window.height => h;

            public GlfwWindow()
            {
                TkGlfw.Monitor* monitor = TkGlfw.GLFW.GetPrimaryMonitor();
                TkGlfw.VideoMode* vm = TkGlfw.GLFW.GetVideoMode(monitor);
                window = new NativeWindow(new NativeWindowSettings()
                {
                    Size = new Vector2i(w = vm->Width, h = vm->Height),
                    Title = "Glfw window",
                    WindowState = WindowState.Fullscreen,
                    StartVisible = true,
                    APIVersion = new Version(3, 3)
                });
                TkGlfw.GLFW.MakeContextCurrent(window.WindowPtr);
                TkGlfw.GLFW.ShowWindow(window.WindowPtr);
                TkGlfw.GLFW.SetInputMode(window.WindowPtr, TkGlfw.CursorStateAttribute.Cursor, TkGlfw.CursorModeValue.CursorDisabled);
            }

            void Window.Present()
            {
                TkGlfw.GLFW.MakeContextCurrent(window.WindowPtr);
                TkGlfw.GLFW.SwapBuffers(window.WindowPtr);
            }
        }

        Overhead PlatformContext.messageQueue => PerformOverhead;

        public GlfwPlatformContext()
        {
            TkGlfw.GLFW.Init();
        }

        public event Action Terminate;
        bool terminated = false;
        AdvancedLinkedList<GlfwWindow> windows = new AdvancedLinkedList<GlfwWindow>();

        GlContext PlatformContext.glContext => this;

        public GlfwWindow CreateWindow()
        {
            GlfwWindow window = new GlfwWindow();
            windows.Add(window);
            return window;
        }

        Window PlatformContext.CreateWindow()
            => CreateWindow();

        void GlContext.Init()
        {
            GL.LoadBindings(new TkGlfw.GLFWBindingsContext());
        }

        void PerformOverhead()
        {
            TkGlfw.GLFW.PollEvents();

            foreach (GlfwWindow window in windows)
                if (TkGlfw.GLFW.WindowShouldClose(window.window.WindowPtr))
                    windows.RemoveCurrent();

            if (windows.empty && !terminated)
            {
                terminated = true;
                Terminate?.Invoke();
            }
        }
    }
}

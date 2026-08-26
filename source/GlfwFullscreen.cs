using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ChaosFramework.Graphics.Imaging;
using ChaosFramework.Math.Vectors;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using TkGlfw = OpenTK.Windowing.GraphicsLibraryFramework;

namespace ChaosFramework.Platform.Glfw
{
    public unsafe class GlfwFullscreen : Fullscreen
    {
        static float SelectX(KeyValuePair<Vector2i, Rgba8Image> v) => v.Key.x;
        static float SelectY(KeyValuePair<Vector2i, Rgba8Image> v) => v.Key.y;
        static TkGlfw.Image CreateImage((Vector2i, RawDataHandle) layer)
            => new TkGlfw.Image(layer.Item1.x, layer.Item1.y, (byte*)layer.Item2.firstElementAddress);

        public readonly NativeWindow window;

        public Monitor monitor { get; }
        public string title { get; set; }

        public uint width => monitor.width;
        public uint height => monitor.height;
        public Vector2i position => monitor.position;

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

        void PresentationContext.SetIcon(ApplicationIcon icon)
        {
            switch(icon.format)
            {
                case ApplicationIcon.IconFormat.ico:
                    using (Stream str = icon.getStream())
                        SetIcon(Icon.FromStream(str));
                    break;

                default:
                    throw new ArgumentException("Unknown icon format", nameof(icon));
            }
        }

        public void SetIcon(Icon icon)
        {
            (Vector2i, RawDataHandle)[] handles = new (Vector2i, RawDataHandle)[icon.imgs.Count];
            try
            {
                int i = 0;
                foreach(KeyValuePair<Vector2i, Rgba8Image> layer in icon.imgs.OrderBy(SelectY).OrderBy(SelectX))
                    handles[i++] = (layer.Value.Size(), layer.Value.GetRawData());

                TkGlfw.GLFW.SetWindowIcon(
                    window.WindowPtr,
                    handles.Select(CreateImage).ToArray()
                    );
            }
            finally
            {
                foreach((Vector2i, RawDataHandle) handle in handles)
                    handle.Item2?.Dispose();
            }
        }

        public void Close()
            => window.Close();
    }
}

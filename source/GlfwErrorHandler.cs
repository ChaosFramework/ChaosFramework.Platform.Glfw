using System;
using ChaosFramework.Collections;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace ChaosFramework.Platform.Glfw
{
    public class GlfwErrorHandler
    {
        public delegate bool HandleGlfwError(ErrorCode errorCode, string message);

        static bool DefaultHandleIconNotSupported(ErrorCode errorCode, string message)
        {
            // here's hoping that this error message never gets localized
            if (errorCode == ErrorCode.FeatureUnavailable && message.Contains("The platform does not support setting the window icon"))
            {
                Console.WriteLine("Couldn't set icon for Glfw presentation context.");
                Console.WriteLine(message);
                return true;
            }

            return false;
        }

        public HandleGlfwError settingIconNotSupportedHandler = DefaultHandleIconNotSupported;

        readonly LinkedList<HandleGlfwError> handlers = [];

        internal GlfwErrorHandler()
        {
            GLFWProvider.SetErrorCallback(GlobalHandler);
            AddHandler(HandleIconNotSupported);
        }

        public void AddHandler(HandleGlfwError handler)
            => handlers.Add(handler);

        public void RemoveHandler(HandleGlfwError handler)
            => handlers.Remove(handler);

        void GlobalHandler(ErrorCode errorCode, string message)
        {
            foreach(HandleGlfwError handler in handlers)
                if (handler(errorCode, message))
                    return;

            throw new GLFWException(message, errorCode);
        }

        bool HandleIconNotSupported(ErrorCode errorCode, string message)
            => settingIconNotSupportedHandler?.Invoke(errorCode, message) ?? false;
    }
}

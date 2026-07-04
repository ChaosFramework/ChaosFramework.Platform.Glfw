using ChaosFramework.Collections;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace ChaosFramework.Platform.Glfw
{
    public class GlfwErrorHandler
    {
        public delegate bool HandleGlfwError(ErrorCode errorCode, string message);

        readonly LinkedList<HandleGlfwError> handlers = [];

        internal GlfwErrorHandler()
        {
            GLFWProvider.SetErrorCallback(GlobalHandler);
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
    }
}

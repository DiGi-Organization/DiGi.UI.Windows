using System;
using System.Runtime.InteropServices;

namespace DiGi.UI.Windows
{
    public static partial class Imports
    {
        /// <summary>
        /// Retrieves a handle to the foreground window.
        /// </summary>
        /// <returns>A handle to the foreground window.</returns>
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        /// <summary>
        /// Retrieves the identifier of the thread that created the specified window and the identifier of the process.
        /// </summary>
        /// <param name="hWnd">A handle to the window.</param>
        /// <param name="lpdwProcessId">A pointer to a variable that receives the process identifier of the process that created the window.</param>
        /// <returns>The identifier of the thread that created the specified window.</returns>
        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        /// <summary>
        /// Destroys an icon and frees any memory the icon occupied; required for a handle returned by <c>Bitmap.GetHicon</c>, which <c>Icon.FromHandle</c> does not take ownership of.
        /// </summary>
        /// <param name="hIcon">A handle to the icon to be destroyed.</param>
        /// <returns><see langword="true"/> when the icon was destroyed.</returns>
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DestroyIcon(IntPtr hIcon);
    }
}
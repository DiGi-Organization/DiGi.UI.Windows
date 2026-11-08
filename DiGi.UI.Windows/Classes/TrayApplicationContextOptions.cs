using System;

namespace DiGi.UI.Windows.Classes
{
    /// <summary>
    /// Behaviour switches for <see cref="TrayApplicationContext{TWindow}"/>, chosen once per application when the context is created.
    /// <para>Each behaviour of the tray context that an application may want differently is a property here, so a project opts in or out without the shared class changing under it. The defaults are the behaviour a new tray application gets.</para>
    /// </summary>
    public class TrayApplicationContextOptions
    {
        /// <summary>
        /// Gets a value indicating whether the window is shown and activated as soon as the context is created. Default <see langword="true"/>; <see langword="false"/> starts the application in the tray only.
        /// </summary>
        public bool ShowWindowOnStartup { get; init; } = true;

        /// <summary>
        /// Gets a value indicating whether the Minimise button hides the window to the tray. Default <see langword="true"/>; <see langword="false"/> leaves the window minimised in the taskbar.
        /// </summary>
        public bool HideOnMinimize { get; init; } = true;

        /// <summary>
        /// Gets a value indicating whether the window is hidden to the tray when it loses focus to another process (desktop, taskbar or another application). Default <see langword="false"/>.
        /// </summary>
        public bool HideOnDeactivate { get; init; } = false;

        /// <summary>
        /// Gets how often the hover popup text is refreshed while the popup is open. Default 250 ms.
        /// </summary>
        public TimeSpan HoverRefreshInterval { get; init; } = TimeSpan.FromMilliseconds(250);
    }
}

using System;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;

namespace DiGi.UI.Windows.Classes
{
    /// <summary>
    /// Provides a base application context for applications that reside in the system tray and manage a specific window type.
    /// </summary>
    /// <typeparam name="TWindow">The type of the Window managed by this context, which must inherit from <see cref="Window"/>.</typeparam>
    public abstract class TrayApplicationContext<TWindow> : ApplicationContext where TWindow : Window
    {
        /// <summary>
        /// The system tray icon used for application notifications and interaction.
        /// </summary>
        protected readonly NotifyIcon? NotifyIcon;
        private TWindow? window;
        private bool isExiting = false;
        private WindowState windowState_Restore = WindowState.Normal;

        /// <summary>
        /// Initializes a new instance of the <see cref="TrayApplicationContext{TWindow}"/> class.
        /// <para>The window is hidden to the tray only by its Minimise or Close button; losing focus leaves it visible.</para>
        /// </summary>
        /// <param name="text">The text to display as a tooltip when hovering over the tray icon.</param>
        /// <param name="showWindowOnStartup">Whether the window is shown and activated as soon as the context is created; <see langword="false"/> starts the application in the tray only.</param>
        public TrayApplicationContext(string text, bool showWindowOnStartup = true)
        {
            // 1. Setup Context Menu
            ContextMenuStrip contextMenuStrip = new ();
            contextMenuStrip.Items.Add("Open", null, (object? s, EventArgs e) => ShowWindow());
            contextMenuStrip.Items.Add("-");
            contextMenuStrip.Items.Add("Exit", null, (object? s, EventArgs e) => ExitApplication());

            // 2. Setup NotifyIcon
            NotifyIcon = new NotifyIcon()
            {
                Icon = Icon.ExtractAssociatedIcon(System.Windows.Forms.Application.ExecutablePath),
                ContextMenuStrip = contextMenuStrip,
                Visible = true,
                Text = text
            };

            NotifyIcon.DoubleClick += (object? s, EventArgs e) => ShowWindow();

            window = GetWindow();
            if(window is not null)
            {
                windowState_Restore = window.WindowState == WindowState.Minimized ? WindowState.Normal : window.WindowState;

                window.Closing += OnWindowClosing;
                window.StateChanged += OnWindowStateChanged;

                if (showWindowOnStartup)
                {
                    ShowWindow();
                }
            }
        }

        /// <summary>
        /// When overridden in a derived class, returns the window instance associated with this application context.
        /// <para>Called from the base constructor, before the derived constructor body runs: it must not depend on state that body assigns.</para>
        /// </summary>
        /// <returns>The <typeparamref name="TWindow"/> instance.</returns>
        protected abstract TWindow GetWindow();

        private void ShowWindow()
        {
            if (window == null)
            {
                return;
            }

            // Restore before showing so a window hidden by Minimise does not come back minimised.
            if (window.WindowState == WindowState.Minimized)
            {
                window.WindowState = windowState_Restore;
            }

            if (!window.IsVisible)
            {
                window.Show();
            }

            window.Activate();
        }

        private void OnWindowStateChanged(object? sender, EventArgs e)
        {
            if (window == null)
            {
                return;
            }

            if (window.WindowState == WindowState.Minimized)
            {
                window.Hide();
            }
            else
            {
                windowState_Restore = window.WindowState;
            }
        }

        private void OnWindowClosing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!isExiting)
            {
                e.Cancel = true;
                window?.Hide();
            }
        }

        private void ExitApplication()
        {
            isExiting = true;

            if (NotifyIcon != null)
            {
                NotifyIcon.Visible = false;
                NotifyIcon.Dispose();
            }

            if (window != null)
            {
                window.Closing -= OnWindowClosing;
                window.Close();
            }

            System.Windows.Application.Current.Shutdown();
        }
    }
}
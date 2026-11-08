using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;

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
        private readonly DispatcherTimer dispatcherTimer_Animation;
        private Icon? icon_Static;
        private List<Icon>? icons_Animation;
        private int animationIndex = 0;

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

            icon_Static = NotifyIcon.Icon;

            // 3. Setup icon animation (created on the UI thread, so its ticks run there too)
            dispatcherTimer_Animation = new DispatcherTimer(DispatcherPriority.Normal) { Interval = TimeSpan.FromMilliseconds(250) };
            dispatcherTimer_Animation.Tick += OnAnimationTick;

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

        /// <summary>
        /// Gets the window managed by this context, or <see langword="null"/> when <see cref="GetWindow"/> returned none.
        /// </summary>
        protected TWindow? Window => window;

        /// <summary>
        /// Gets a value indicating whether the tray icon is currently cycling through the animation icons.
        /// </summary>
        protected bool IsAnimated => dispatcherTimer_Animation.IsEnabled;

        /// <summary>
        /// Sets the icon shown in the tray while no animation runs.
        /// <para>Replaces the default, which is the icon associated with the executable (a single 32 px image Windows scales down for the tray); an icon loaded at <see cref="SystemInformation.SmallIconSize"/> renders sharper.</para>
        /// </summary>
        /// <param name="icon">The static icon; <see langword="null"/> is ignored.</param>
        protected void SetIcon(Icon? icon)
        {
            if (icon is null)
            {
                return;
            }

            Invoke(() =>
            {
                icon_Static = icon;
                if (!dispatcherTimer_Animation.IsEnabled && NotifyIcon is not null)
                {
                    NotifyIcon.Icon = icon;
                }
            });
        }

        /// <summary>
        /// Sets the frames the tray icon cycles through while animated, and the time each frame is shown.
        /// </summary>
        /// <param name="icons">The animation frames in display order; <see langword="null"/> or empty disables the animation.</param>
        /// <param name="interval">The time each frame is shown.</param>
        protected void SetAnimationIcons(IEnumerable<Icon>? icons, TimeSpan interval)
        {
            List<Icon>? icons_Temp = icons?.Where(x => x is not null).ToList();

            Invoke(() =>
            {
                icons_Animation = icons_Temp is null || icons_Temp.Count == 0 ? null : icons_Temp;
                animationIndex = 0;
                dispatcherTimer_Animation.Interval = interval;

                if (icons_Animation is null)
                {
                    StopAnimation();
                }
            });
        }

        /// <summary>
        /// Starts or stops the tray icon animation; stopping restores the static icon.
        /// <para>Safe to call from any thread and idempotent, so it can be driven straight from state-change notifications. Starting does nothing until <see cref="SetAnimationIcons"/> has supplied frames.</para>
        /// </summary>
        /// <param name="animated"><see langword="true"/> to cycle the animation icons; <see langword="false"/> to show the static icon.</param>
        protected void SetAnimated(bool animated)
        {
            Invoke(() =>
            {
                if (!animated)
                {
                    StopAnimation();
                    return;
                }

                if (isExiting || icons_Animation is null || dispatcherTimer_Animation.IsEnabled)
                {
                    return;
                }

                animationIndex = 0;
                OnAnimationTick(null, EventArgs.Empty);
                dispatcherTimer_Animation.Start();
            });
        }

        private void OnAnimationTick(object? sender, EventArgs e)
        {
            if (NotifyIcon is null || icons_Animation is null || icons_Animation.Count == 0)
            {
                return;
            }

            NotifyIcon.Icon = icons_Animation[animationIndex % icons_Animation.Count];
            animationIndex = (animationIndex + 1) % icons_Animation.Count;
        }

        private void StopAnimation()
        {
            dispatcherTimer_Animation.Stop();
            if (NotifyIcon is not null && icon_Static is not null && !isExiting)
            {
                NotifyIcon.Icon = icon_Static;
            }
        }

        private void Invoke(Action action)
        {
            if (dispatcherTimer_Animation.Dispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                dispatcherTimer_Animation.Dispatcher.BeginInvoke(action);
            }
        }

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
            dispatcherTimer_Animation.Stop();

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
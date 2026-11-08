using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;

namespace DiGi.UI.Windows.Classes
{
    /// <summary>
    /// Provides a base application context for applications that reside in the system tray and manage a specific window type.
    /// <para>Window behaviour (show on startup, hide on Minimise, hide on focus loss, hover refresh) is chosen per application through <see cref="TrayApplicationContextOptions"/>. The tray icon can be replaced (<see cref="SetIcon"/>), animated (<see cref="SetAnimationIcons"/>, <see cref="SetAnimated"/>) and marked with a blinking indicator dot (<see cref="SetIndicator"/>, <see cref="SetIndicatorVisible"/>), and the hover tooltip can be replaced by live text (<see cref="GetHoverText"/>); all four are opt-in and leave the default tray icon and tooltip untouched when unused.</para>
    /// </summary>
    /// <typeparam name="TWindow">The type of the Window managed by this context, which must inherit from <see cref="Window"/>.</typeparam>
    public abstract class TrayApplicationContext<TWindow> : ApplicationContext where TWindow : Window
    {
        /// <summary>
        /// The longest text Windows accepts as a tray tooltip; <see cref="NotifyIcon"/> throws above it.
        /// </summary>
        private const int TooltipTextMaxLength = 127;

        /// <summary>
        /// The system tray icon used for application notifications and interaction.
        /// <para>Set its tooltip through <see cref="TooltipText"/>, not <c>NotifyIcon.Text</c>: the hover popup clears and restores the tooltip and would overwrite a direct assignment.</para>
        /// </summary>
        protected readonly NotifyIcon? NotifyIcon;
        private readonly TrayApplicationContextOptions trayApplicationContextOptions;
        private readonly TWindow? window;
        private bool isExiting;
        private WindowState windowState_Restore = WindowState.Normal;
        private string tooltipText;

        private readonly DispatcherTimer dispatcherTimer_Animation;
        private Icon? icon_Static;
        private List<Icon>? icons_Animation;
        private int animationIndex;

        private readonly DispatcherTimer dispatcherTimer_Indicator;
        private System.Drawing.Color? color_Indicator;
        private bool indicatorVisible;
        private bool indicatorBlinks = true;
        private bool indicatorShown = true;
        private Icon? icon_Static_Indicator;
        private List<Icon>? icons_Animation_Indicator;

        private readonly System.Windows.Controls.Primitives.Popup popup_Hover;
        private readonly System.Windows.Controls.TextBlock textBlock_Hover;
        private readonly DispatcherTimer dispatcherTimer_Hover;
        private System.Drawing.Point point_Hover;

        /// <summary>
        /// Initializes a new instance of the <see cref="TrayApplicationContext{TWindow}"/> class.
        /// <para>Must be created on the UI thread: the icon animation and hover timers are bound to its dispatcher.</para>
        /// </summary>
        /// <param name="text">The tooltip shown when hovering over the tray icon; trimmed to 127 characters, the Windows limit.</param>
        /// <param name="trayApplicationContextOptions">The behaviour switches; <see langword="null"/> uses the defaults of <see cref="TrayApplicationContextOptions"/>.</param>
        public TrayApplicationContext(string text, TrayApplicationContextOptions? trayApplicationContextOptions = null)
        {
            this.trayApplicationContextOptions = trayApplicationContextOptions ?? new TrayApplicationContextOptions();
            tooltipText = TrimTooltipText(text);

            // 1. Setup Context Menu
            ContextMenuStrip contextMenuStrip = new ();
            contextMenuStrip.Items.Add("Open", null, (object? s, EventArgs e) => ShowWindow());
            contextMenuStrip.Items.Add("-");
            contextMenuStrip.Items.Add("Exit", null, (object? s, EventArgs e) => ExitApplication());
            contextMenuStrip.Opening += (object? s, System.ComponentModel.CancelEventArgs e) => CloseHover();

            // 2. Setup NotifyIcon
            NotifyIcon = new NotifyIcon()
            {
                Icon = Icon.ExtractAssociatedIcon(System.Windows.Forms.Application.ExecutablePath),
                ContextMenuStrip = contextMenuStrip,
                Visible = true,
                Text = tooltipText
            };

            NotifyIcon.DoubleClick += (object? s, EventArgs e) => ShowWindow();
            NotifyIcon.MouseMove += OnNotifyIconMouseMove;

            icon_Static = NotifyIcon.Icon;

            // 3. Setup icon animation (created on the UI thread, so its ticks run there too)
            dispatcherTimer_Animation = new DispatcherTimer(DispatcherPriority.Normal) { Interval = TimeSpan.FromMilliseconds(250) };
            dispatcherTimer_Animation.Tick += OnAnimationTick;

            dispatcherTimer_Indicator = new DispatcherTimer(DispatcherPriority.Normal) { Interval = TimeSpan.FromSeconds(1) };
            dispatcherTimer_Indicator.Tick += OnIndicatorTick;

            // 4. Setup hover popup (replaces the native tooltip while GetHoverText returns text)
            textBlock_Hover = new System.Windows.Controls.TextBlock()
            {
                FontSize = 12,
                Foreground = System.Windows.SystemColors.InfoTextBrush,
            };

            // MousePoint lets WPF read the cursor and convert it for the monitor's DPI; it flips the popup above the
            // cursor when there is no room below, which is the case for a taskbar at the bottom of the screen.
            popup_Hover = new System.Windows.Controls.Primitives.Popup()
            {
                Child = new System.Windows.Controls.Border()
                {
                    Background = System.Windows.SystemColors.InfoBrush,
                    BorderBrush = System.Windows.SystemColors.ActiveBorderBrush,
                    BorderThickness = new Thickness(1),
                    Padding = new Thickness(8, 6, 8, 6),
                    Child = textBlock_Hover
                },
                Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint,
                StaysOpen = true,
                Focusable = false,
            };

            dispatcherTimer_Hover = new DispatcherTimer(DispatcherPriority.Normal) { Interval = this.trayApplicationContextOptions.HoverRefreshInterval };
            dispatcherTimer_Hover.Tick += OnHoverTick;

            // 5. Setup window
            window = GetWindow();
            if (window is not null)
            {
                windowState_Restore = window.WindowState == WindowState.Minimized ? WindowState.Normal : window.WindowState;

                window.Closing += OnWindowClosing;
                window.StateChanged += OnWindowStateChanged;
                window.Deactivated += OnWindowDeactivated;

                if (this.trayApplicationContextOptions.ShowWindowOnStartup)
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
        /// Gets the behaviour switches this context was created with.
        /// </summary>
        protected TrayApplicationContextOptions TrayApplicationContextOptions => trayApplicationContextOptions;

        /// <summary>
        /// Gets a value indicating whether the tray icon is currently cycling through the animation icons.
        /// </summary>
        protected bool IsAnimated => dispatcherTimer_Animation.IsEnabled;

        /// <summary>
        /// Gets or sets the native tooltip shown when hovering over the tray icon while <see cref="GetHoverText"/> returns <see langword="null"/>.
        /// <para>Trimmed to 127 characters, the Windows limit. Must be set on the UI thread.</para>
        /// </summary>
        protected string TooltipText
        {
            get
            {
                return tooltipText;
            }

            set
            {
                tooltipText = TrimTooltipText(value);
                if (NotifyIcon is not null && !isExiting && !popup_Hover.IsOpen)
                {
                    NotifyIcon.Text = tooltipText;
                }
            }
        }

        /// <summary>
        /// Sets the icon shown in the tray while no animation runs.
        /// <para>Replaces the default, which is the icon associated with the executable (a single 32 px image Windows scales down for the tray); an icon loaded at <see cref="SystemInformation.SmallIconSize"/> renders sharper. The context does not take ownership: the caller keeps the icon alive and disposes it.</para>
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
                UpdateIndicatorIcons();
            });
        }

        /// <summary>
        /// Sets the frames the tray icon cycles through while animated, and the time each frame is shown.
        /// <para>The context does not take ownership of the icons.</para>
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
                    dispatcherTimer_Animation.Stop();
                }

                UpdateIndicatorIcons();
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
                dispatcherTimer_Animation.Start();
                UpdateNotifyIcon();
            });
        }

        /// <summary>
        /// Sets the colour and blink rate of the indicator dot drawn in the top-right corner of the tray icon.
        /// <para>The dot is drawn over whichever icon is showing, static or animation frame, so it combines with <see cref="SetAnimated"/>. It is shown only while <see cref="SetIndicatorVisible"/> is <see langword="true"/>. Safe to call from any thread.</para>
        /// </summary>
        /// <param name="color">The dot colour; <see langword="null"/> removes the indicator.</param>
        /// <param name="blinkInterval">How long the dot stays on, then off; <see cref="TimeSpan.Zero"/> or less shows it steadily.</param>
        protected void SetIndicator(System.Drawing.Color? color, TimeSpan blinkInterval)
        {
            Invoke(() =>
            {
                color_Indicator = color;
                indicatorBlinks = blinkInterval > TimeSpan.Zero;
                if (indicatorBlinks)
                {
                    dispatcherTimer_Indicator.Interval = blinkInterval;
                }

                UpdateIndicatorIcons();

                // Restart so a new interval or colour takes effect at once.
                dispatcherTimer_Indicator.Stop();
                ApplyIndicator(indicatorVisible);
            });
        }

        /// <summary>
        /// Shows or hides the indicator dot configured by <see cref="SetIndicator"/>.
        /// <para>Safe to call from any thread and idempotent, so it can be driven straight from state-change notifications. Does nothing visible until <see cref="SetIndicator"/> has supplied a colour.</para>
        /// </summary>
        /// <param name="visible"><see langword="true"/> to show (and blink) the dot; <see langword="false"/> to remove it.</param>
        protected void SetIndicatorVisible(bool visible)
        {
            Invoke(() => ApplyIndicator(visible));
        }

        private void ApplyIndicator(bool visible)
        {
            indicatorVisible = visible;

            if (!visible || isExiting || color_Indicator is null || !indicatorBlinks)
            {
                dispatcherTimer_Indicator.Stop();
                indicatorShown = true;
            }
            else if (!dispatcherTimer_Indicator.IsEnabled)
            {
                // Each run starts with the dot on, so it appears the moment a task starts.
                indicatorShown = true;
                dispatcherTimer_Indicator.Start();
            }

            UpdateNotifyIcon();
        }

        private void OnIndicatorTick(object? sender, EventArgs e)
        {
            indicatorShown = !indicatorShown;
            UpdateNotifyIcon();
        }

        /// <summary>
        /// Shows the icon matching the current state: the animation frame or the static icon, with or without the indicator dot.
        /// </summary>
        private void UpdateNotifyIcon()
        {
            if (NotifyIcon is null || isExiting)
            {
                return;
            }

            bool indicator = indicatorVisible && indicatorShown && color_Indicator is not null;

            Icon? icon;
            if (dispatcherTimer_Animation.IsEnabled && icons_Animation is not null && icons_Animation.Count != 0)
            {
                int index = animationIndex % icons_Animation.Count;
                icon = indicator && icons_Animation_Indicator is not null && index < icons_Animation_Indicator.Count ? icons_Animation_Indicator[index] : icons_Animation[index];
            }
            else
            {
                icon = indicator && icon_Static_Indicator is not null ? icon_Static_Indicator : icon_Static;
            }

            if (icon is not null && !ReferenceEquals(NotifyIcon.Icon, icon))
            {
                NotifyIcon.Icon = icon;
            }
        }

        /// <summary>
        /// Rebuilds the indicator variants of the static and animation icons, then disposes the previous ones once they are no longer shown.
        /// <para>These variants are created and owned by this context, unlike the icons passed in.</para>
        /// </summary>
        private void UpdateIndicatorIcons()
        {
            Icon? icon_Static_Indicator_Old = icon_Static_Indicator;
            List<Icon>? icons_Animation_Indicator_Old = icons_Animation_Indicator;

            icon_Static_Indicator = null;
            icons_Animation_Indicator = null;

            if (color_Indicator is System.Drawing.Color color)
            {
                icon_Static_Indicator = IndicatorIcon(icon_Static, color);
                icons_Animation_Indicator = icons_Animation?.ConvertAll(x => IndicatorIcon(x, color) ?? x);
            }

            UpdateNotifyIcon();

            DisposeIndicatorIcons(icon_Static_Indicator_Old, icons_Animation_Indicator_Old);
        }

        private void DisposeIndicatorIcons(Icon? icon, List<Icon>? icons)
        {
            icon?.Dispose();
            if (icons is null)
            {
                return;
            }

            foreach (Icon icon_Temp in icons)
            {
                // An animation frame the indicator could not be drawn on is the caller's icon itself, not a copy.
                if (icons_Animation is null || !icons_Animation.Contains(icon_Temp))
                {
                    icon_Temp.Dispose();
                }
            }
        }

        private static Icon? IndicatorIcon(Icon? icon, System.Drawing.Color color)
        {
            if (icon is null)
            {
                return null;
            }

            using Bitmap bitmap = icon.ToBitmap();
            int size = Math.Min(bitmap.Width, bitmap.Height);
            int diameter = Math.Max(5, (int)Math.Round(size * 0.44));
            int border = Math.Max(1, size / 16);

            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using SolidBrush solidBrush_Border = new(System.Drawing.Color.White);
                using SolidBrush solidBrush = new(color);
                graphics.FillEllipse(solidBrush_Border, bitmap.Width - diameter, 0, diameter, diameter);
                graphics.FillEllipse(solidBrush, bitmap.Width - diameter + border, border, diameter - (2 * border), diameter - (2 * border));
            }

            IntPtr handle = bitmap.GetHicon();
            try
            {
                using Icon icon_Handle = Icon.FromHandle(handle);
                return (Icon)icon_Handle.Clone();
            }
            finally
            {
                Imports.DestroyIcon(handle);
            }
        }

        /// <summary>
        /// When overridden in a derived class, returns the text shown in a popup while the pointer rests on the tray icon.
        /// <para>Called on the UI thread when the pointer enters the icon and every <see cref="TrayApplicationContextOptions.HoverRefreshInterval"/> while the popup is open, so the text follows live state. Return <see langword="null"/> to show the native tooltip (<see cref="TooltipText"/>) instead; the native tooltip is limited to 127 characters, the popup is not. An exception thrown here closes the popup and is otherwise ignored, so a failing override cannot raise an error every refresh. The default returns <see langword="null"/>.</para>
        /// </summary>
        /// <returns>The popup text, or <see langword="null"/> for the native tooltip.</returns>
        protected virtual string? GetHoverText()
        {
            return null;
        }

        private static string TrimTooltipText(string? text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            return text.Length <= TooltipTextMaxLength ? text : text.Substring(0, TooltipTextMaxLength);
        }

        private string? HoverText()
        {
            try
            {
                return GetHoverText();
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
                return null;
            }
        }

        private void OnNotifyIconMouseMove(object? sender, MouseEventArgs e)
        {
            if (NotifyIcon is null || isExiting || NotifyIcon.ContextMenuStrip?.Visible == true)
            {
                return;
            }

            point_Hover = Control.MousePosition;

            string? text_Hover = HoverText();
            if (text_Hover is null)
            {
                CloseHover();
                return;
            }

            // An empty Text suppresses the native tooltip, which would otherwise show on top of the popup.
            NotifyIcon.Text = string.Empty;
            textBlock_Hover.Text = text_Hover;

            if (!popup_Hover.IsOpen)
            {
                popup_Hover.IsOpen = true;
                dispatcherTimer_Hover.Start();
            }
        }

        private void OnHoverTick(object? sender, EventArgs e)
        {
            // NotifyIcon raises no mouse-leave; the pointer has left once it drifts further than an icon from the last MouseMove.
            System.Drawing.Point point = Control.MousePosition;
            int distance = Math.Max(Math.Abs(point.X - point_Hover.X), Math.Abs(point.Y - point_Hover.Y));
            if (distance > SystemInformation.SmallIconSize.Width)
            {
                CloseHover();
                return;
            }

            string? text_Hover = HoverText();
            if (text_Hover is null)
            {
                CloseHover();
                return;
            }

            textBlock_Hover.Text = text_Hover;
        }

        private void CloseHover()
        {
            dispatcherTimer_Hover.Stop();
            popup_Hover.IsOpen = false;

            if (NotifyIcon is not null && !isExiting)
            {
                NotifyIcon.Text = tooltipText;
            }
        }

        private void OnAnimationTick(object? sender, EventArgs e)
        {
            if (icons_Animation is null || icons_Animation.Count == 0)
            {
                return;
            }

            animationIndex = (animationIndex + 1) % icons_Animation.Count;
            UpdateNotifyIcon();
        }

        private void StopAnimation()
        {
            dispatcherTimer_Animation.Stop();
            UpdateNotifyIcon();
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

            // Restore before showing so a window hidden while minimised does not come back minimised.
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

            if (window.WindowState != WindowState.Minimized)
            {
                windowState_Restore = window.WindowState;
                return;
            }

            if (trayApplicationContextOptions.HideOnMinimize)
            {
                window.Hide();
            }
        }

        private void OnWindowDeactivated(object? sender, EventArgs e)
        {
            if (!trayApplicationContextOptions.HideOnDeactivate || window == null || !window.IsVisible)
            {
                return;
            }

            // Using Dispatcher to wait for the OS to update the Foreground Window
            window.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (window == null || !window.IsVisible)
                {
                    return;
                }

                IntPtr foregroundHandle = Imports.GetForegroundWindow();
                if (foregroundHandle == IntPtr.Zero)
                {
                    return;
                }

                _ = Imports.GetWindowThreadProcessId(foregroundHandle, out uint foregroundProcessId);
                uint currentProcessId = (uint)Environment.ProcessId;

                // Only hide if the new active window belongs to a different process
                // (e.g. user clicked on Desktop, Taskbar or another App)
                if (foregroundProcessId != currentProcessId)
                {
                    window.Hide();
                }
            }), DispatcherPriority.Background);
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
            dispatcherTimer_Indicator.Stop();
            dispatcherTimer_Hover.Stop();
            popup_Hover.IsOpen = false;

            if (NotifyIcon != null)
            {
                NotifyIcon.Visible = false;
                NotifyIcon.Dispose();
            }

            DisposeIndicatorIcons(icon_Static_Indicator, icons_Animation_Indicator);
            icon_Static_Indicator = null;
            icons_Animation_Indicator = null;

            if (window != null)
            {
                window.Closing -= OnWindowClosing;
                window.Close();
            }

            System.Windows.Application.Current.Shutdown();
        }
    }
}

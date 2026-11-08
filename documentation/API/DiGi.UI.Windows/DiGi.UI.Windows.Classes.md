#### [DiGi\.UI\.Windows](DiGi.UI.Windows.Overview.md 'DiGi\.UI\.Windows\.Overview')

## DiGi\.UI\.Windows\.Classes Namespace
### Classes

<a name='DiGi.UI.Windows.Classes.TrayApplicationContext_TWindow_'></a>

## TrayApplicationContext\<TWindow\> Class

Provides a base application context for applications that reside in the system tray and manage a specific window type\.

Window behaviour (show on startup, hide on Minimise, hide on focus loss, hover refresh) is chosen per application through [TrayApplicationContextOptions](DiGi.UI.Windows.Classes.md#DiGi.UI.Windows.Classes.TrayApplicationContext_TWindow_.TrayApplicationContextOptions 'DiGi\.UI\.Windows\.Classes\.TrayApplicationContext\<TWindow\>\.TrayApplicationContextOptions'). The tray icon can be replaced ([SetIcon\(Icon\)](DiGi.UI.Windows.Classes.md#DiGi.UI.Windows.Classes.TrayApplicationContext_TWindow_.SetIcon(System.Drawing.Icon) 'DiGi\.UI\.Windows\.Classes\.TrayApplicationContext\<TWindow\>\.SetIcon\(System\.Drawing\.Icon\)')) and animated ([SetAnimationIcons\(IEnumerable&lt;Icon&gt;, TimeSpan\)](DiGi.UI.Windows.Classes.md#DiGi.UI.Windows.Classes.TrayApplicationContext_TWindow_.SetAnimationIcons(System.Collections.Generic.IEnumerable_System.Drawing.Icon_,System.TimeSpan) 'DiGi\.UI\.Windows\.Classes\.TrayApplicationContext\<TWindow\>\.SetAnimationIcons\(System\.Collections\.Generic\.IEnumerable\<System\.Drawing\.Icon\>, System\.TimeSpan\)'), [SetAnimated\(bool\)](DiGi.UI.Windows.Classes.md#DiGi.UI.Windows.Classes.TrayApplicationContext_TWindow_.SetAnimated(bool) 'DiGi\.UI\.Windows\.Classes\.TrayApplicationContext\<TWindow\>\.SetAnimated\(bool\)')), and the hover tooltip can be replaced by live text ([GetHoverText\(\)](DiGi.UI.Windows.Classes.md#DiGi.UI.Windows.Classes.TrayApplicationContext_TWindow_.GetHoverText() 'DiGi\.UI\.Windows\.Classes\.TrayApplicationContext\<TWindow\>\.GetHoverText\(\)')); all three are opt-in and leave the default tray icon and tooltip untouched when unused.

```csharp
public abstract class TrayApplicationContext<TWindow> : System.Windows.Forms.ApplicationContext
    where TWindow : System.Windows.Window
```
#### Type parameters

<a name='DiGi.UI.Windows.Classes.TrayApplicationContext_TWindow_.TWindow'></a>

`TWindow`

The type of the Window managed by this context, which must inherit from [Window](DiGi.UI.Windows.Classes.md#DiGi.UI.Windows.Classes.TrayApplicationContext_TWindow_.Window 'DiGi\.UI\.Windows\.Classes\.TrayApplicationContext\<TWindow\>\.Window')\.

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → [System\.Windows\.Forms\.ApplicationContext](https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.applicationcontext 'System\.Windows\.Forms\.ApplicationContext') → TrayApplicationContext\<TWindow\>
### Constructors

<a name='DiGi.UI.Windows.Classes.TrayApplicationContext_TWindow_.TrayApplicationContext(string,DiGi.UI.Windows.Classes.TrayApplicationContextOptions)'></a>

## TrayApplicationContext\(string, TrayApplicationContextOptions\) Constructor

Initializes a new instance of the [TrayApplicationContext&lt;TWindow&gt;](DiGi.UI.Windows.Classes.md#DiGi.UI.Windows.Classes.TrayApplicationContext_TWindow_ 'DiGi\.UI\.Windows\.Classes\.TrayApplicationContext\<TWindow\>') class\.

Must be created on the UI thread: the icon animation and hover timers are bound to its dispatcher.

```csharp
public TrayApplicationContext(string text, DiGi.UI.Windows.Classes.TrayApplicationContextOptions? trayApplicationContextOptions=null);
```
#### Parameters

<a name='DiGi.UI.Windows.Classes.TrayApplicationContext_TWindow_.TrayApplicationContext(string,DiGi.UI.Windows.Classes.TrayApplicationContextOptions).text'></a>

`text` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The tooltip shown when hovering over the tray icon; trimmed to 127 characters, the Windows limit\.

<a name='DiGi.UI.Windows.Classes.TrayApplicationContext_TWindow_.TrayApplicationContext(string,DiGi.UI.Windows.Classes.TrayApplicationContextOptions).trayApplicationContextOptions'></a>

`trayApplicationContextOptions` [TrayApplicationContextOptions](DiGi.UI.Windows.Classes.md#DiGi.UI.Windows.Classes.TrayApplicationContextOptions 'DiGi\.UI\.Windows\.Classes\.TrayApplicationContextOptions')

The behaviour switches; [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null') uses the defaults of [TrayApplicationContextOptions](DiGi.UI.Windows.Classes.md#DiGi.UI.Windows.Classes.TrayApplicationContext_TWindow_.TrayApplicationContextOptions 'DiGi\.UI\.Windows\.Classes\.TrayApplicationContext\<TWindow\>\.TrayApplicationContextOptions')\.
### Fields

<a name='DiGi.UI.Windows.Classes.TrayApplicationContext_TWindow_.NotifyIcon'></a>

## TrayApplicationContext\<TWindow\>\.NotifyIcon Field

The system tray icon used for application notifications and interaction\.

Set its tooltip through [TooltipText](DiGi.UI.Windows.Classes.md#DiGi.UI.Windows.Classes.TrayApplicationContext_TWindow_.TooltipText 'DiGi\.UI\.Windows\.Classes\.TrayApplicationContext\<TWindow\>\.TooltipText'), not `NotifyIcon.Text`: the hover popup clears and restores the tooltip and would overwrite a direct assignment.

```csharp
protected readonly NotifyIcon? NotifyIcon;
```

#### Field Value
[System\.Windows\.Forms\.NotifyIcon](https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.notifyicon 'System\.Windows\.Forms\.NotifyIcon')

<a name='DiGi.UI.Windows.Classes.TrayApplicationContext_TWindow_.TooltipTextMaxLength'></a>

## TrayApplicationContext\<TWindow\>\.TooltipTextMaxLength Field

The longest text Windows accepts as a tray tooltip; [NotifyIcon](DiGi.UI.Windows.Classes.md#DiGi.UI.Windows.Classes.TrayApplicationContext_TWindow_.NotifyIcon 'DiGi\.UI\.Windows\.Classes\.TrayApplicationContext\<TWindow\>\.NotifyIcon') throws above it\.

```csharp
private const int TooltipTextMaxLength = 127;
```

#### Field Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')
### Properties

<a name='DiGi.UI.Windows.Classes.TrayApplicationContext_TWindow_.IsAnimated'></a>

## TrayApplicationContext\<TWindow\>\.IsAnimated Property

Gets a value indicating whether the tray icon is currently cycling through the animation icons\.

```csharp
protected bool IsAnimated { protected get; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

<a name='DiGi.UI.Windows.Classes.TrayApplicationContext_TWindow_.TooltipText'></a>

## TrayApplicationContext\<TWindow\>\.TooltipText Property

Gets or sets the native tooltip shown when hovering over the tray icon while [GetHoverText\(\)](DiGi.UI.Windows.Classes.md#DiGi.UI.Windows.Classes.TrayApplicationContext_TWindow_.GetHoverText() 'DiGi\.UI\.Windows\.Classes\.TrayApplicationContext\<TWindow\>\.GetHoverText\(\)') returns [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null')\.

Trimmed to 127 characters, the Windows limit. Must be set on the UI thread.

```csharp
protected string TooltipText { protected get; protected set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.UI.Windows.Classes.TrayApplicationContext_TWindow_.TrayApplicationContextOptions'></a>

## TrayApplicationContext\<TWindow\>\.TrayApplicationContextOptions Property

Gets the behaviour switches this context was created with\.

```csharp
protected DiGi.UI.Windows.Classes.TrayApplicationContextOptions TrayApplicationContextOptions { protected get; }
```

#### Property Value
[TrayApplicationContextOptions](DiGi.UI.Windows.Classes.md#DiGi.UI.Windows.Classes.TrayApplicationContextOptions 'DiGi\.UI\.Windows\.Classes\.TrayApplicationContextOptions')

<a name='DiGi.UI.Windows.Classes.TrayApplicationContext_TWindow_.Window'></a>

## TrayApplicationContext\<TWindow\>\.Window Property

Gets the window managed by this context, or [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null') when [GetWindow\(\)](DiGi.UI.Windows.Classes.md#DiGi.UI.Windows.Classes.TrayApplicationContext_TWindow_.GetWindow() 'DiGi\.UI\.Windows\.Classes\.TrayApplicationContext\<TWindow\>\.GetWindow\(\)') returned none\.

```csharp
protected TWindow? Window { protected get; }
```

#### Property Value
[TWindow](DiGi.UI.Windows.Classes.md#DiGi.UI.Windows.Classes.TrayApplicationContext_TWindow_.TWindow 'DiGi\.UI\.Windows\.Classes\.TrayApplicationContext\<TWindow\>\.TWindow')
### Methods

<a name='DiGi.UI.Windows.Classes.TrayApplicationContext_TWindow_.GetHoverText()'></a>

## TrayApplicationContext\<TWindow\>\.GetHoverText\(\) Method

When overridden in a derived class, returns the text shown in a popup while the pointer rests on the tray icon\.

Called on the UI thread when the pointer enters the icon and every [HoverRefreshInterval](DiGi.UI.Windows.Classes.md#DiGi.UI.Windows.Classes.TrayApplicationContextOptions.HoverRefreshInterval 'DiGi\.UI\.Windows\.Classes\.TrayApplicationContextOptions\.HoverRefreshInterval') while the popup is open, so the text follows live state. Return [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null') to show the native tooltip ([TooltipText](DiGi.UI.Windows.Classes.md#DiGi.UI.Windows.Classes.TrayApplicationContext_TWindow_.TooltipText 'DiGi\.UI\.Windows\.Classes\.TrayApplicationContext\<TWindow\>\.TooltipText')) instead; the native tooltip is limited to 127 characters, the popup is not. An exception thrown here closes the popup and is otherwise ignored, so a failing override cannot raise an error every refresh. The default returns [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null').

```csharp
protected virtual string? GetHoverText();
```

#### Returns
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
The popup text, or [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null') for the native tooltip\.

<a name='DiGi.UI.Windows.Classes.TrayApplicationContext_TWindow_.GetWindow()'></a>

## TrayApplicationContext\<TWindow\>\.GetWindow\(\) Method

When overridden in a derived class, returns the window instance associated with this application context\.

Called from the base constructor, before the derived constructor body runs: it must not depend on state that body assigns.

```csharp
protected abstract TWindow GetWindow();
```

#### Returns
[TWindow](DiGi.UI.Windows.Classes.md#DiGi.UI.Windows.Classes.TrayApplicationContext_TWindow_.TWindow 'DiGi\.UI\.Windows\.Classes\.TrayApplicationContext\<TWindow\>\.TWindow')  
The [TWindow](DiGi.UI.Windows.Classes.md#DiGi.UI.Windows.Classes.TrayApplicationContext_TWindow_.TWindow 'DiGi\.UI\.Windows\.Classes\.TrayApplicationContext\<TWindow\>\.TWindow') instance\.

<a name='DiGi.UI.Windows.Classes.TrayApplicationContext_TWindow_.SetAnimated(bool)'></a>

## TrayApplicationContext\<TWindow\>\.SetAnimated\(bool\) Method

Starts or stops the tray icon animation; stopping restores the static icon\.

Safe to call from any thread and idempotent, so it can be driven straight from state-change notifications. Starting does nothing until [SetAnimationIcons\(IEnumerable&lt;Icon&gt;, TimeSpan\)](DiGi.UI.Windows.Classes.md#DiGi.UI.Windows.Classes.TrayApplicationContext_TWindow_.SetAnimationIcons(System.Collections.Generic.IEnumerable_System.Drawing.Icon_,System.TimeSpan) 'DiGi\.UI\.Windows\.Classes\.TrayApplicationContext\<TWindow\>\.SetAnimationIcons\(System\.Collections\.Generic\.IEnumerable\<System\.Drawing\.Icon\>, System\.TimeSpan\)') has supplied frames.

```csharp
protected void SetAnimated(bool animated);
```
#### Parameters

<a name='DiGi.UI.Windows.Classes.TrayApplicationContext_TWindow_.SetAnimated(bool).animated'></a>

`animated` [System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

[true](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/bool 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/builtin\-types/bool') to cycle the animation icons; [false](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/bool 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/builtin\-types/bool') to show the static icon\.

<a name='DiGi.UI.Windows.Classes.TrayApplicationContext_TWindow_.SetAnimationIcons(System.Collections.Generic.IEnumerable_System.Drawing.Icon_,System.TimeSpan)'></a>

## TrayApplicationContext\<TWindow\>\.SetAnimationIcons\(IEnumerable\<Icon\>, TimeSpan\) Method

Sets the frames the tray icon cycles through while animated, and the time each frame is shown\.

The context does not take ownership of the icons.

```csharp
protected void SetAnimationIcons(System.Collections.Generic.IEnumerable<System.Drawing.Icon>? icons, System.TimeSpan interval);
```
#### Parameters

<a name='DiGi.UI.Windows.Classes.TrayApplicationContext_TWindow_.SetAnimationIcons(System.Collections.Generic.IEnumerable_System.Drawing.Icon_,System.TimeSpan).icons'></a>

`icons` [System\.Collections\.Generic\.IEnumerable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1 'System\.Collections\.Generic\.IEnumerable\`1')[System\.Drawing\.Icon](https://learn.microsoft.com/en-us/dotnet/api/system.drawing.icon 'System\.Drawing\.Icon')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1 'System\.Collections\.Generic\.IEnumerable\`1')

The animation frames in display order; [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null') or empty disables the animation\.

<a name='DiGi.UI.Windows.Classes.TrayApplicationContext_TWindow_.SetAnimationIcons(System.Collections.Generic.IEnumerable_System.Drawing.Icon_,System.TimeSpan).interval'></a>

`interval` [System\.TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan 'System\.TimeSpan')

The time each frame is shown\.

<a name='DiGi.UI.Windows.Classes.TrayApplicationContext_TWindow_.SetIcon(System.Drawing.Icon)'></a>

## TrayApplicationContext\<TWindow\>\.SetIcon\(Icon\) Method

Sets the icon shown in the tray while no animation runs\.

Replaces the default, which is the icon associated with the executable (a single 32 px image Windows scales down for the tray); an icon loaded at [System\.Windows\.Forms\.SystemInformation\.SmallIconSize](https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.systeminformation.smalliconsize 'System\.Windows\.Forms\.SystemInformation\.SmallIconSize') renders sharper. The context does not take ownership: the caller keeps the icon alive and disposes it.

```csharp
protected void SetIcon(System.Drawing.Icon? icon);
```
#### Parameters

<a name='DiGi.UI.Windows.Classes.TrayApplicationContext_TWindow_.SetIcon(System.Drawing.Icon).icon'></a>

`icon` [System\.Drawing\.Icon](https://learn.microsoft.com/en-us/dotnet/api/system.drawing.icon 'System\.Drawing\.Icon')

The static icon; [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null') is ignored\.

<a name='DiGi.UI.Windows.Classes.TrayApplicationContextOptions'></a>

## TrayApplicationContextOptions Class

Behaviour switches for [TrayApplicationContext&lt;TWindow&gt;](DiGi.UI.Windows.Classes.md#DiGi.UI.Windows.Classes.TrayApplicationContext_TWindow_ 'DiGi\.UI\.Windows\.Classes\.TrayApplicationContext\<TWindow\>'), chosen once per application when the context is created\.

Each behaviour of the tray context that an application may want differently is a property here, so a project opts in or out without the shared class changing under it. The defaults are the behaviour a new tray application gets.

```csharp
public class TrayApplicationContextOptions
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → TrayApplicationContextOptions
### Properties

<a name='DiGi.UI.Windows.Classes.TrayApplicationContextOptions.HideOnDeactivate'></a>

## TrayApplicationContextOptions\.HideOnDeactivate Property

Gets a value indicating whether the window is hidden to the tray when it loses focus to another process \(desktop, taskbar or another application\)\. Default [false](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/bool 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/builtin\-types/bool')\.

```csharp
public bool HideOnDeactivate { get; init; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

<a name='DiGi.UI.Windows.Classes.TrayApplicationContextOptions.HideOnMinimize'></a>

## TrayApplicationContextOptions\.HideOnMinimize Property

Gets a value indicating whether the Minimise button hides the window to the tray\. Default [true](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/bool 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/builtin\-types/bool'); [false](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/bool 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/builtin\-types/bool') leaves the window minimised in the taskbar\.

```csharp
public bool HideOnMinimize { get; init; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

<a name='DiGi.UI.Windows.Classes.TrayApplicationContextOptions.HoverRefreshInterval'></a>

## TrayApplicationContextOptions\.HoverRefreshInterval Property

Gets how often the hover popup text is refreshed while the popup is open\. Default 250 ms\.

```csharp
public System.TimeSpan HoverRefreshInterval { get; init; }
```

#### Property Value
[System\.TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan 'System\.TimeSpan')

<a name='DiGi.UI.Windows.Classes.TrayApplicationContextOptions.ShowWindowOnStartup'></a>

## TrayApplicationContextOptions\.ShowWindowOnStartup Property

Gets a value indicating whether the window is shown and activated as soon as the context is created\. Default [true](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/bool 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/builtin\-types/bool'); [false](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/bool 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/builtin\-types/bool') starts the application in the tray only\.

```csharp
public bool ShowWindowOnStartup { get; init; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')
#### [DiGi\.UI\.Windows](DiGi.UI.Windows.Overview.md 'DiGi\.UI\.Windows\.Overview')

## DiGi\.UI\.Windows\.Classes Namespace
### Classes

<a name='DiGi.UI.Windows.Classes.TrayApplicationContext_TWindow_'></a>

## TrayApplicationContext\<TWindow\> Class

Provides a base application context for applications that reside in the system tray and manage a specific window type\.

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

<a name='DiGi.UI.Windows.Classes.TrayApplicationContext_TWindow_.TrayApplicationContext(string,bool)'></a>

## TrayApplicationContext\(string, bool\) Constructor

Initializes a new instance of the [TrayApplicationContext&lt;TWindow&gt;](DiGi.UI.Windows.Classes.md#DiGi.UI.Windows.Classes.TrayApplicationContext_TWindow_ 'DiGi\.UI\.Windows\.Classes\.TrayApplicationContext\<TWindow\>') class\.

The window is hidden to the tray only by its Minimise or Close button; losing focus leaves it visible.

```csharp
public TrayApplicationContext(string text, bool showWindowOnStartup=true);
```
#### Parameters

<a name='DiGi.UI.Windows.Classes.TrayApplicationContext_TWindow_.TrayApplicationContext(string,bool).text'></a>

`text` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The text to display as a tooltip when hovering over the tray icon\.

<a name='DiGi.UI.Windows.Classes.TrayApplicationContext_TWindow_.TrayApplicationContext(string,bool).showWindowOnStartup'></a>

`showWindowOnStartup` [System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

Whether the window is shown and activated as soon as the context is created; [false](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/bool 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/builtin\-types/bool') starts the application in the tray only\.
### Fields

<a name='DiGi.UI.Windows.Classes.TrayApplicationContext_TWindow_.NotifyIcon'></a>

## TrayApplicationContext\<TWindow\>\.NotifyIcon Field

The system tray icon used for application notifications and interaction\.

```csharp
protected readonly NotifyIcon? NotifyIcon;
```

#### Field Value
[System\.Windows\.Forms\.NotifyIcon](https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.notifyicon 'System\.Windows\.Forms\.NotifyIcon')
### Properties

<a name='DiGi.UI.Windows.Classes.TrayApplicationContext_TWindow_.IsAnimated'></a>

## TrayApplicationContext\<TWindow\>\.IsAnimated Property

Gets a value indicating whether the tray icon is currently cycling through the animation icons\.

```csharp
protected bool IsAnimated { protected get; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

<a name='DiGi.UI.Windows.Classes.TrayApplicationContext_TWindow_.Window'></a>

## TrayApplicationContext\<TWindow\>\.Window Property

Gets the window managed by this context, or [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null') when [GetWindow\(\)](DiGi.UI.Windows.Classes.md#DiGi.UI.Windows.Classes.TrayApplicationContext_TWindow_.GetWindow() 'DiGi\.UI\.Windows\.Classes\.TrayApplicationContext\<TWindow\>\.GetWindow\(\)') returned none\.

```csharp
protected TWindow? Window { protected get; }
```

#### Property Value
[TWindow](DiGi.UI.Windows.Classes.md#DiGi.UI.Windows.Classes.TrayApplicationContext_TWindow_.TWindow 'DiGi\.UI\.Windows\.Classes\.TrayApplicationContext\<TWindow\>\.TWindow')
### Methods

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

Replaces the default, which is the icon associated with the executable (a single 32 px image Windows scales down for the tray); an icon loaded at [System\.Windows\.Forms\.SystemInformation\.SmallIconSize](https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.systeminformation.smalliconsize 'System\.Windows\.Forms\.SystemInformation\.SmallIconSize') renders sharper.

```csharp
protected void SetIcon(System.Drawing.Icon? icon);
```
#### Parameters

<a name='DiGi.UI.Windows.Classes.TrayApplicationContext_TWindow_.SetIcon(System.Drawing.Icon).icon'></a>

`icon` [System\.Drawing\.Icon](https://learn.microsoft.com/en-us/dotnet/api/system.drawing.icon 'System\.Drawing\.Icon')

The static icon; [null](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/null 'https://docs\.microsoft\.com/en\-us/dotnet/csharp/language\-reference/keywords/null') is ignored\.
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

The type of the Window managed by this context, which must inherit from [System\.Windows\.Window](https://learn.microsoft.com/en-us/dotnet/api/system.windows.window 'System\.Windows\.Window')\.

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
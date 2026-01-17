# ModelingEvolution.FloatingWindow

A draggable, resizable floating window component for Blazor applications using MudBlazor. Supports both declarative and service-based patterns (similar to MudBlazor's dialog system).

## Installation

```bash
dotnet add package ModelingEvolution.FloatingWindow
```

## Setup

1. Add the CSS to your `App.razor` or `_Host.cshtml`:

```html
<link href="_content/ModelingEvolution.FloatingWindow/floatingWindow.css" rel="stylesheet" />
```

2. Add the namespace to your `_Imports.razor`:

```razor
@using ModelingEvolution.FloatingWindow
```

3. Register the service in `Program.cs`:

```csharp
builder.Services.AddFloatingWindow();
```

4. Add the provider to your `MainLayout.razor`:

```razor
<MudThemeProvider />
<MudPopoverProvider />
<MudDialogProvider />
<MudSnackbarProvider />
<FloatingWindowProvider />  @* Add this line *@
```

## Usage

### Service-Based Pattern (Recommended)

Open windows programmatically from anywhere in your application:

```razor
@page "/"
@inject IFloatingWindowService FloatingWindows

<MudButton OnClick="OpenWindow">Open Window</MudButton>
<MudButton OnClick="OpenSettings">Open Settings</MudButton>

@code {
    void OpenWindow()
    {
        FloatingWindows.Show<MyContent>("Window Title", new FloatingWindowOptions
        {
            Width = 500,
            Height = 400,
            CenterOnScreen = true
        });
    }

    async Task OpenSettings()
    {
        var windowRef = FloatingWindows.Show<SettingsPanel>("Settings");
        var result = await windowRef.Result;

        if (!result.Cancelled && result.TryGetData<string>(out var data))
        {
            Console.WriteLine($"Settings saved: {data}");
        }
    }
}
```

#### Content Component with Window Access

```razor
@* SettingsPanel.razor *@
<MudTextField @bind-Value="_name" Label="Name" />
<MudButton OnClick="Save">Save</MudButton>
<MudButton OnClick="Cancel">Cancel</MudButton>

@code {
    [CascadingParameter]
    IFloatingWindowInstance? Window { get; set; }

    string _name = "";

    async Task Save()
    {
        await Window!.CloseAsync(_name);
    }

    async Task Cancel()
    {
        await Window!.CloseAsync();
    }
}
```

### Declarative Pattern

For simpler use cases, use the component directly:

```razor
@page "/"

<MudButton OnClick="() => isWindowVisible = true">Open Window</MudButton>

<FloatingWindow @bind-IsVisible="isWindowVisible"
                Title="My Window"
                Left="100"
                Top="100"
                Width="400"
                Height="300">
    <p>Window content goes here</p>
</FloatingWindow>

@code {
    private bool isWindowVisible = false;
}
```

## API Reference

### IFloatingWindowService

| Method | Description |
|--------|-------------|
| `Show<T>(title, options?)` | Open window with component type T |
| `Show<T>(title, parameters, options?)` | Open with component and parameters |
| `Show(title, RenderFragment, options?)` | Open with render fragment |
| `CloseAll()` | Close all open windows |
| `BringToFront(windowId)` | Bring window to front |

### IFloatingWindowReference

Returned when opening a window:

| Member | Description |
|--------|-------------|
| `Id` | Unique window identifier |
| `Title` | Window title (get/set) |
| `Result` | Task that completes when window closes |
| `State` | Current state (Normal, Minimized, Maximized) |
| `Close()` | Close without result |
| `Close<T>(data)` | Close with result data |
| `BringToFront()` | Bring to front |
| `Minimize()` / `Maximize()` / `Restore()` | Window state control |

### IFloatingWindowInstance

Cascaded to content components:

| Member | Description |
|--------|-------------|
| `Id` | Window identifier |
| `Title` | Get/set title dynamically |
| `CloseAsync()` | Close (cancelled) |
| `CloseAsync<T>(data)` | Close with data |
| `BringToFront()` / `Minimize()` / `Maximize()` / `Restore()` | Window control |

### FloatingWindowOptions

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Left` | `double` | `100` | Initial X position |
| `Top` | `double` | `100` | Initial Y position |
| `Width` | `double` | `400` | Window width |
| `Height` | `double` | `300` | Window height |
| `MinWidth` | `double` | `200` | Minimum width |
| `MinHeight` | `double` | `150` | Minimum height |
| `CanClose` | `bool` | `true` | Show close button |
| `CanMaximize` | `bool` | `true` | Show maximize button |
| `CanResize` | `bool` | `true` | Enable resize handle |
| `CenterOnScreen` | `bool` | `false` | Center when opened |
| `CloseOnOutsideClick` | `bool` | `false` | Close on outside click |
| `CloseOnEscape` | `bool` | `true` | Close on Escape key |
| `CssClass` | `string?` | `null` | Additional CSS class |

Static factory methods:
- `FloatingWindowOptions.Default` - Default options
- `FloatingWindowOptions.Centered(width, height)` - Centered window
- `FloatingWindowOptions.Modal(width, height)` - Modal-like behavior

## Features

- **Multi-Window Support**: Open multiple windows simultaneously
- **Draggable**: Click and drag the header to move
- **Resizable**: Drag the bottom-right corner to resize
- **Minimize/Maximize/Restore**: Standard window controls
- **Result Awaiting**: Await window close and get result data
- **Cascading Access**: Content components can control their window
- **Z-Index Management**: Click to bring to front
- **MudBlazor Integration**: Uses MudBlazor theming
- **Dark Theme Support**: Automatically adapts to theme

## Requirements

- .NET 10.0 or later
- MudBlazor 8.0.0 or later

## License

MIT

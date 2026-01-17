# ModelingEvolution.FloatingWindow

A draggable, resizable floating window component for Blazor applications using MudBlazor.

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

## Usage

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

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Title` | `string` | `"Window"` | Window title displayed in the header |
| `IsVisible` | `bool` | `false` | Controls window visibility (supports two-way binding) |
| `Left` | `double` | `100` | Initial X position in pixels |
| `Top` | `double` | `100` | Initial Y position in pixels |
| `Width` | `double` | `400` | Initial width in pixels |
| `Height` | `double` | `300` | Initial height in pixels |
| `MinWidth` | `double` | `200` | Minimum width when resizing |
| `MinHeight` | `double` | `150` | Minimum height when resizing |
| `CanClose` | `bool` | `true` | Show/hide close button |
| `CanMaximize` | `bool` | `true` | Show/hide maximize button |
| `CanResize` | `bool` | `true` | Enable/disable resize handle |
| `ChildContent` | `RenderFragment` | - | Window content |

## Features

- **Draggable**: Click and drag the header to move the window
- **Resizable**: Drag the bottom-right corner to resize
- **Minimize**: Collapse to show only the header
- **Maximize**: Expand to fill the viewport
- **Close**: Hide the window (controlled via `IsVisible`)
- **MudBlazor Integration**: Uses MudBlazor theming and components
- **Dark Theme Support**: Automatically adapts to MudBlazor dark theme

## Requirements

- .NET 10.0 or later
- MudBlazor 8.0.0 or later

## License

MIT

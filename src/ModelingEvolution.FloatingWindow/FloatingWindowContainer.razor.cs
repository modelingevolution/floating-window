using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace ModelingEvolution.FloatingWindow;

/// <summary>
/// Internal container component that renders a single floating window.
/// </summary>
public partial class FloatingWindowContainer : ComponentBase, IAsyncDisposable
{
    [Inject] private IJSRuntime JS { get; set; } = default!;

    [Parameter, EditorRequired]
    public FloatingWindowReference WindowReference { get; set; } = default!;

    private ElementReference _windowElement;
    private DotNetObjectReference<FloatingWindowContainer>? _dotNetRef;
    private IJSObjectReference? _jsModule;

    private bool _isDragging;
    private bool _isResizing;
    private double _dragStartX;
    private double _dragStartY;
    private bool _initialized;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _dotNetRef = DotNetObjectReference.Create(this);
            _jsModule = await JS.InvokeAsync<IJSObjectReference>(
                "import", "./_content/ModelingEvolution.FloatingWindow/floatingWindow.js");
            await _jsModule.InvokeVoidAsync("initialize", WindowReference.Id.ToString(), _dotNetRef);
            _initialized = true;

            // Center on screen if requested
            if (WindowReference.Options.CenterOnScreen && !_initialized)
            {
                var viewport = await _jsModule.InvokeAsync<ViewportSize>("getViewportSize");
                WindowReference.CurrentLeft = (viewport.Width - WindowReference.CurrentWidth) / 2;
                WindowReference.CurrentTop = (viewport.Height - WindowReference.CurrentHeight) / 2;
                StateHasChanged();
            }
        }
    }

    private RenderFragment RenderContent()
    {
        if (WindowReference.Content != null)
        {
            return WindowReference.Content;
        }

        if (WindowReference.ComponentType != null)
        {
            return builder =>
            {
                builder.OpenComponent(0, WindowReference.ComponentType);
                if (WindowReference.Parameters != null)
                {
                    builder.AddMultipleAttributes(1, WindowReference.Parameters.ToDictionary()
                        .Where(p => p.Value != null)!);
                }
                builder.CloseComponent();
            };
        }

        return _ => { };
    }

    private string GetWindowStyle()
    {
        var zIndex = WindowReference.ZIndex;

        if (WindowReference.State == FloatingWindowState.Maximized)
        {
            return $"left: 0; top: 0; width: 100vw; height: 100vh; z-index: {zIndex};";
        }

        return $"left: {WindowReference.CurrentLeft}px; top: {WindowReference.CurrentTop}px; " +
               $"width: {WindowReference.CurrentWidth}px; height: {WindowReference.CurrentHeight}px; z-index: {zIndex};";
    }

    private void OnWindowClick()
    {
        WindowReference.BringToFront();
    }

    private async Task OnHeaderMouseDown(MouseEventArgs e)
    {
        if (WindowReference.State == FloatingWindowState.Maximized) return;

        _isDragging = true;
        _dragStartX = e.ClientX - WindowReference.CurrentLeft;
        _dragStartY = e.ClientY - WindowReference.CurrentTop;

        if (_jsModule is not null)
        {
            await _jsModule.InvokeVoidAsync("startDrag", WindowReference.Id.ToString());
        }
    }

    private async Task OnResizeMouseDown(MouseEventArgs e)
    {
        if (WindowReference.State == FloatingWindowState.Maximized) return;

        _isResizing = true;
        _dragStartX = e.ClientX;
        _dragStartY = e.ClientY;

        if (_jsModule is not null)
        {
            await _jsModule.InvokeVoidAsync("startResize", WindowReference.Id.ToString());
        }
    }

    [JSInvokable]
    public void OnMouseMove(double clientX, double clientY)
    {
        if (_isDragging)
        {
            WindowReference.CurrentLeft = Math.Max(0, clientX - _dragStartX);
            WindowReference.CurrentTop = Math.Max(0, clientY - _dragStartY);
            StateHasChanged();
        }
        else if (_isResizing)
        {
            var deltaX = clientX - _dragStartX;
            var deltaY = clientY - _dragStartY;

            WindowReference.CurrentWidth = Math.Max(WindowReference.Options.MinWidth, WindowReference.CurrentWidth + deltaX);
            WindowReference.CurrentHeight = Math.Max(WindowReference.Options.MinHeight, WindowReference.CurrentHeight + deltaY);

            _dragStartX = clientX;
            _dragStartY = clientY;
            StateHasChanged();
        }
    }

    [JSInvokable]
    public void OnMouseUp()
    {
        _isDragging = false;
        _isResizing = false;
    }

    private void ToggleMinimize()
    {
        if (WindowReference.State == FloatingWindowState.Minimized)
        {
            WindowReference.Restore();
        }
        else
        {
            WindowReference.Minimize();
        }
        StateHasChanged();
    }

    private void ToggleMaximize()
    {
        if (WindowReference.State == FloatingWindowState.Maximized)
        {
            WindowReference.Restore();
        }
        else
        {
            WindowReference.Maximize();
        }
        StateHasChanged();
    }

    private void Close()
    {
        WindowReference.Close();
    }

    public async ValueTask DisposeAsync()
    {
        if (_jsModule is not null)
        {
            await _jsModule.InvokeVoidAsync("dispose", WindowReference.Id.ToString());
            await _jsModule.DisposeAsync();
        }
        _dotNetRef?.Dispose();
    }

    private record ViewportSize(double Width, double Height);
}

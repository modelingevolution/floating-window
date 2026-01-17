using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace ModelingEvolution.FloatingWindow;

/// <summary>
/// A floating, draggable window component for Blazor using MudBlazor.
/// </summary>
public partial class FloatingWindow : ComponentBase, IAsyncDisposable
{
    [Inject] private IJSRuntime JS { get; set; } = default!;

    /// <summary>
    /// The title displayed in the window header.
    /// </summary>
    [Parameter] public string Title { get; set; } = "Window";

    /// <summary>
    /// The content to display inside the window.
    /// </summary>
    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Initial X position of the window in pixels.
    /// </summary>
    [Parameter] public double Left { get; set; } = 100;

    /// <summary>
    /// Initial Y position of the window in pixels.
    /// </summary>
    [Parameter] public double Top { get; set; } = 100;

    /// <summary>
    /// Width of the window in pixels.
    /// </summary>
    [Parameter] public double Width { get; set; } = 400;

    /// <summary>
    /// Height of the window in pixels.
    /// </summary>
    [Parameter] public double Height { get; set; } = 300;

    /// <summary>
    /// Minimum width of the window in pixels.
    /// </summary>
    [Parameter] public double MinWidth { get; set; } = 200;

    /// <summary>
    /// Minimum height of the window in pixels.
    /// </summary>
    [Parameter] public double MinHeight { get; set; } = 150;

    /// <summary>
    /// Whether the window can be closed.
    /// </summary>
    [Parameter] public bool CanClose { get; set; } = true;

    /// <summary>
    /// Whether the window can be maximized.
    /// </summary>
    [Parameter] public bool CanMaximize { get; set; } = true;

    /// <summary>
    /// Whether the window can be resized.
    /// </summary>
    [Parameter] public bool CanResize { get; set; } = true;

    /// <summary>
    /// Whether the window is visible.
    /// </summary>
    [Parameter] public bool IsVisible { get; set; } = true;

    /// <summary>
    /// Callback when the window visibility changes.
    /// </summary>
    [Parameter] public EventCallback<bool> IsVisibleChanged { get; set; }

    /// <summary>
    /// Callback when the window is closed.
    /// </summary>
    [Parameter] public EventCallback OnClosed { get; set; }

    /// <summary>
    /// Whether the window is currently minimized.
    /// </summary>
    public bool IsMinimized { get; private set; }

    /// <summary>
    /// Whether the window is currently maximized.
    /// </summary>
    public bool IsMaximized { get; private set; }

    private ElementReference _windowElement;
    private DotNetObjectReference<FloatingWindow>? _dotNetRef;
    private IJSObjectReference? _jsModule;

    private double _currentLeft;
    private double _currentTop;
    private double _currentWidth;
    private double _currentHeight;

    private double _preMaxLeft;
    private double _preMaxTop;
    private double _preMaxWidth;
    private double _preMaxHeight;

    private bool _isDragging;
    private bool _isResizing;
    private double _dragStartX;
    private double _dragStartY;

    protected override void OnInitialized()
    {
        _currentLeft = Left;
        _currentTop = Top;
        _currentWidth = Width;
        _currentHeight = Height;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _dotNetRef = DotNetObjectReference.Create(this);
            _jsModule = await JS.InvokeAsync<IJSObjectReference>(
                "import", "./_content/ModelingEvolution.FloatingWindow/floatingWindow.js");
            await _jsModule.InvokeVoidAsync("initialize", _dotNetRef);
        }
    }

    private string GetWindowStyle()
    {
        if (!IsVisible) return "display: none;";
        if (IsMaximized) return "left: 0; top: 0; width: 100vw; height: 100vh;";

        return $"left: {_currentLeft}px; top: {_currentTop}px; width: {_currentWidth}px; height: {_currentHeight}px;";
    }

    private async Task OnHeaderMouseDown(MouseEventArgs e)
    {
        if (IsMaximized) return;

        _isDragging = true;
        _dragStartX = e.ClientX - _currentLeft;
        _dragStartY = e.ClientY - _currentTop;

        if (_jsModule is not null)
        {
            await _jsModule.InvokeVoidAsync("startDrag");
        }
    }

    private async Task OnResizeMouseDown(MouseEventArgs e)
    {
        if (IsMaximized) return;

        _isResizing = true;
        _dragStartX = e.ClientX;
        _dragStartY = e.ClientY;

        if (_jsModule is not null)
        {
            await _jsModule.InvokeVoidAsync("startResize");
        }
    }

    [JSInvokable]
    public void OnMouseMove(double clientX, double clientY)
    {
        if (_isDragging)
        {
            _currentLeft = Math.Max(0, clientX - _dragStartX);
            _currentTop = Math.Max(0, clientY - _dragStartY);
            StateHasChanged();
        }
        else if (_isResizing)
        {
            var deltaX = clientX - _dragStartX;
            var deltaY = clientY - _dragStartY;

            _currentWidth = Math.Max(MinWidth, _currentWidth + deltaX);
            _currentHeight = Math.Max(MinHeight, _currentHeight + deltaY);

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
        IsMinimized = !IsMinimized;
    }

    private void ToggleMaximize()
    {
        if (IsMaximized)
        {
            _currentLeft = _preMaxLeft;
            _currentTop = _preMaxTop;
            _currentWidth = _preMaxWidth;
            _currentHeight = _preMaxHeight;
        }
        else
        {
            _preMaxLeft = _currentLeft;
            _preMaxTop = _currentTop;
            _preMaxWidth = _currentWidth;
            _preMaxHeight = _currentHeight;
        }

        IsMaximized = !IsMaximized;
        IsMinimized = false;
    }

    private async Task Close()
    {
        IsVisible = false;
        await IsVisibleChanged.InvokeAsync(false);
        await OnClosed.InvokeAsync();
    }

    public async ValueTask DisposeAsync()
    {
        if (_jsModule is not null)
        {
            await _jsModule.InvokeVoidAsync("dispose");
            await _jsModule.DisposeAsync();
        }
        _dotNetRef?.Dispose();
    }
}

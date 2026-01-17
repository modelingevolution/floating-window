using Microsoft.AspNetCore.Components;

namespace ModelingEvolution.FloatingWindow;

/// <summary>
/// Implementation of IFloatingWindowReference.
/// </summary>
public class FloatingWindowReference : IFloatingWindowReference, IFloatingWindowInstance
{
    private readonly TaskCompletionSource<FloatingWindowResult> _resultTcs = new();
    private readonly Action<FloatingWindowReference> _onClose;
    private readonly Action<FloatingWindowReference>? _onStateChanged;

    public Guid Id { get; }
    public string Title { get; set; }
    public Type? ComponentType { get; }
    public RenderFragment? Content { get; }
    public FloatingWindowParameters? Parameters { get; }
    public FloatingWindowOptions Options { get; }
    public FloatingWindowState State { get; private set; } = FloatingWindowState.Normal;
    public int ZIndex { get; set; }
    public Task<FloatingWindowResult> Result => _resultTcs.Task;

    // Position and size (can be updated by container during drag/resize)
    public double CurrentLeft { get; set; }
    public double CurrentTop { get; set; }
    public double CurrentWidth { get; set; }
    public double CurrentHeight { get; set; }

    // Pre-maximize state for restore
    public double PreMaxLeft { get; set; }
    public double PreMaxTop { get; set; }
    public double PreMaxWidth { get; set; }
    public double PreMaxHeight { get; set; }

    internal FloatingWindowReference(
        Type? componentType,
        RenderFragment? content,
        string title,
        FloatingWindowParameters? parameters,
        FloatingWindowOptions options,
        Action<FloatingWindowReference> onClose,
        Action<FloatingWindowReference>? onStateChanged = null)
    {
        Id = Guid.NewGuid();
        ComponentType = componentType;
        Content = content;
        Title = title;
        Parameters = parameters;
        Options = options;
        _onClose = onClose;
        _onStateChanged = onStateChanged;

        // Initialize position/size from options
        CurrentLeft = options.Left;
        CurrentTop = options.Top;
        CurrentWidth = options.Width;
        CurrentHeight = options.Height;
    }

    public void Close()
    {
        _resultTcs.TrySetResult(FloatingWindowResult.Cancel());
        _onClose(this);
    }

    public void Close<T>(T result)
    {
        _resultTcs.TrySetResult(FloatingWindowResult.Ok(result));
        _onClose(this);
    }

    public Task CloseAsync()
    {
        Close();
        return Task.CompletedTask;
    }

    public Task CloseAsync<T>(T result)
    {
        Close(result);
        return Task.CompletedTask;
    }

    public void BringToFront()
    {
        _onStateChanged?.Invoke(this);
    }

    public void Minimize()
    {
        if (State != FloatingWindowState.Minimized)
        {
            State = FloatingWindowState.Minimized;
            _onStateChanged?.Invoke(this);
        }
    }

    public void Maximize()
    {
        if (State != FloatingWindowState.Maximized)
        {
            if (State == FloatingWindowState.Normal)
            {
                // Save current state before maximizing
                PreMaxLeft = CurrentLeft;
                PreMaxTop = CurrentTop;
                PreMaxWidth = CurrentWidth;
                PreMaxHeight = CurrentHeight;
            }
            State = FloatingWindowState.Maximized;
            _onStateChanged?.Invoke(this);
        }
    }

    public void Restore()
    {
        if (State != FloatingWindowState.Normal)
        {
            if (State == FloatingWindowState.Maximized)
            {
                // Restore pre-maximize state
                CurrentLeft = PreMaxLeft;
                CurrentTop = PreMaxTop;
                CurrentWidth = PreMaxWidth;
                CurrentHeight = PreMaxHeight;
            }
            State = FloatingWindowState.Normal;
            _onStateChanged?.Invoke(this);
        }
    }

    internal void SetState(FloatingWindowState state)
    {
        State = state;
    }
}

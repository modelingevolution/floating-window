using Microsoft.AspNetCore.Components;

namespace ModelingEvolution.FloatingWindow;

/// <summary>
/// Service implementation for managing floating windows.
/// </summary>
public class FloatingWindowService : IFloatingWindowService
{
    private readonly List<FloatingWindowReference> _windows = new();
    private int _nextZIndex = 1000;

    public event Action<IFloatingWindowReference>? OnWindowOpened;
    public event Action<IFloatingWindowReference>? OnWindowClosed;

    public IReadOnlyList<IFloatingWindowReference> OpenWindows => _windows.AsReadOnly();

    internal IReadOnlyList<FloatingWindowReference> Windows => _windows.AsReadOnly();

    public IFloatingWindowReference Show<TComponent>(string title, FloatingWindowOptions? options = null)
        where TComponent : ComponentBase
    {
        return ShowInternal(typeof(TComponent), null, title, null, options ?? FloatingWindowOptions.Default);
    }

    public IFloatingWindowReference Show<TComponent>(
        string title,
        FloatingWindowParameters parameters,
        FloatingWindowOptions? options = null)
        where TComponent : ComponentBase
    {
        return ShowInternal(typeof(TComponent), null, title, parameters, options ?? FloatingWindowOptions.Default);
    }

    public IFloatingWindowReference Show(string title, RenderFragment content, FloatingWindowOptions? options = null)
    {
        return ShowInternal(null, content, title, null, options ?? FloatingWindowOptions.Default);
    }

    private IFloatingWindowReference ShowInternal(
        Type? componentType,
        RenderFragment? content,
        string title,
        FloatingWindowParameters? parameters,
        FloatingWindowOptions options)
    {
        var reference = new FloatingWindowReference(
            componentType,
            content,
            title,
            parameters,
            options,
            OnWindowCloseRequested,
            OnWindowStateChanged)
        {
            ZIndex = _nextZIndex++
        };

        _windows.Add(reference);
        OnWindowOpened?.Invoke(reference);
        return reference;
    }

    private void OnWindowCloseRequested(FloatingWindowReference reference)
    {
        _windows.Remove(reference);
        OnWindowClosed?.Invoke(reference);
    }

    private void OnWindowStateChanged(FloatingWindowReference reference)
    {
        // Bring to front when state changes
        BringToFront(reference.Id);
    }

    public void CloseAll()
    {
        // Close all windows in reverse order
        var windowsToClose = _windows.ToList();
        foreach (var window in windowsToClose)
        {
            window.Close();
        }
    }

    public void BringToFront(Guid windowId)
    {
        var window = _windows.FirstOrDefault(w => w.Id == windowId);
        if (window != null)
        {
            window.ZIndex = _nextZIndex++;
            // Raise event to trigger UI refresh
            OnWindowOpened?.Invoke(window);
        }
    }
}

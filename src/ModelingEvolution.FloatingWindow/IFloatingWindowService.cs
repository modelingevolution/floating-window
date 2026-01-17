using Microsoft.AspNetCore.Components;

namespace ModelingEvolution.FloatingWindow;

/// <summary>
/// Service for programmatically opening and managing floating windows.
/// Inject this service to show floating windows from anywhere in your application.
/// </summary>
public interface IFloatingWindowService
{
    /// <summary>
    /// Event raised when a window is opened.
    /// </summary>
    event Action<IFloatingWindowReference>? OnWindowOpened;

    /// <summary>
    /// Event raised when a window is closed.
    /// </summary>
    event Action<IFloatingWindowReference>? OnWindowClosed;

    /// <summary>
    /// Gets all currently open windows.
    /// </summary>
    IReadOnlyList<IFloatingWindowReference> OpenWindows { get; }

    /// <summary>
    /// Opens a floating window with the specified component type.
    /// </summary>
    /// <typeparam name="TComponent">The component type to render inside the window.</typeparam>
    /// <param name="title">The window title.</param>
    /// <param name="options">Optional window configuration.</param>
    /// <returns>A reference to the opened window.</returns>
    IFloatingWindowReference Show<TComponent>(string title, FloatingWindowOptions? options = null)
        where TComponent : ComponentBase;

    /// <summary>
    /// Opens a floating window with the specified component type and parameters.
    /// </summary>
    /// <typeparam name="TComponent">The component type to render inside the window.</typeparam>
    /// <param name="title">The window title.</param>
    /// <param name="parameters">Parameters to pass to the component.</param>
    /// <param name="options">Optional window configuration.</param>
    /// <returns>A reference to the opened window.</returns>
    IFloatingWindowReference Show<TComponent>(
        string title,
        FloatingWindowParameters parameters,
        FloatingWindowOptions? options = null)
        where TComponent : ComponentBase;

    /// <summary>
    /// Opens a floating window with the specified render fragment.
    /// </summary>
    /// <param name="title">The window title.</param>
    /// <param name="content">The content to render inside the window.</param>
    /// <param name="options">Optional window configuration.</param>
    /// <returns>A reference to the opened window.</returns>
    IFloatingWindowReference Show(string title, RenderFragment content, FloatingWindowOptions? options = null);

    /// <summary>
    /// Closes all open windows.
    /// </summary>
    void CloseAll();

    /// <summary>
    /// Brings the specified window to the front.
    /// </summary>
    /// <param name="windowId">The window ID.</param>
    void BringToFront(Guid windowId);
}

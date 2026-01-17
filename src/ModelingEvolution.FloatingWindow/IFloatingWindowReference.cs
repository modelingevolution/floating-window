namespace ModelingEvolution.FloatingWindow;

/// <summary>
/// A reference to an open floating window. Returned when opening a window via IFloatingWindowService.
/// Use this to control the window, await its result, or close it.
/// </summary>
public interface IFloatingWindowReference
{
    /// <summary>
    /// Unique identifier for this window.
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// The window title.
    /// </summary>
    string Title { get; set; }

    /// <summary>
    /// Task that completes when the window is closed. Contains the result data.
    /// </summary>
    Task<FloatingWindowResult> Result { get; }

    /// <summary>
    /// Gets or sets the current window state (normal, minimized, maximized).
    /// </summary>
    FloatingWindowState State { get; }

    /// <summary>
    /// Closes the window without a result.
    /// </summary>
    void Close();

    /// <summary>
    /// Closes the window with the specified result data.
    /// </summary>
    /// <typeparam name="T">The type of the result data.</typeparam>
    /// <param name="result">The result data.</param>
    void Close<T>(T result);

    /// <summary>
    /// Brings the window to the front of all other windows.
    /// </summary>
    void BringToFront();

    /// <summary>
    /// Minimizes the window.
    /// </summary>
    void Minimize();

    /// <summary>
    /// Maximizes the window.
    /// </summary>
    void Maximize();

    /// <summary>
    /// Restores the window to its normal state.
    /// </summary>
    void Restore();
}

namespace ModelingEvolution.FloatingWindow;

/// <summary>
/// Cascaded to window content components to allow them to interact with their containing window.
/// </summary>
public interface IFloatingWindowInstance
{
    /// <summary>
    /// Unique identifier for this window.
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// Gets or sets the window title dynamically.
    /// </summary>
    string Title { get; set; }

    /// <summary>
    /// Closes the window without returning a result (marks as cancelled).
    /// </summary>
    Task CloseAsync();

    /// <summary>
    /// Closes the window with the specified result data.
    /// </summary>
    /// <typeparam name="T">The type of the result data.</typeparam>
    /// <param name="result">The result data to return.</param>
    Task CloseAsync<T>(T result);

    /// <summary>
    /// Brings this window to the front.
    /// </summary>
    void BringToFront();

    /// <summary>
    /// Minimizes this window.
    /// </summary>
    void Minimize();

    /// <summary>
    /// Maximizes this window.
    /// </summary>
    void Maximize();

    /// <summary>
    /// Restores this window to normal state.
    /// </summary>
    void Restore();
}

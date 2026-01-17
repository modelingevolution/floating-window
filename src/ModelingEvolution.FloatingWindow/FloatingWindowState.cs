namespace ModelingEvolution.FloatingWindow;

/// <summary>
/// Represents the current state of a floating window.
/// </summary>
public enum FloatingWindowState
{
    /// <summary>
    /// The window is in its normal state (not minimized or maximized).
    /// </summary>
    Normal,

    /// <summary>
    /// The window is minimized.
    /// </summary>
    Minimized,

    /// <summary>
    /// The window is maximized.
    /// </summary>
    Maximized
}

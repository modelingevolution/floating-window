namespace ModelingEvolution.FloatingWindow;

/// <summary>
/// Configuration options for a floating window.
/// </summary>
public class FloatingWindowOptions
{
    /// <summary>
    /// Initial X position of the window in pixels. Default is 100.
    /// </summary>
    public double Left { get; set; } = 100;

    /// <summary>
    /// Initial Y position of the window in pixels. Default is 100.
    /// </summary>
    public double Top { get; set; } = 100;

    /// <summary>
    /// Width of the window in pixels. Default is 400.
    /// </summary>
    public double Width { get; set; } = 400;

    /// <summary>
    /// Height of the window in pixels. Default is 300.
    /// </summary>
    public double Height { get; set; } = 300;

    /// <summary>
    /// Minimum width of the window in pixels. Default is 200.
    /// </summary>
    public double MinWidth { get; set; } = 200;

    /// <summary>
    /// Minimum height of the window in pixels. Default is 150.
    /// </summary>
    public double MinHeight { get; set; } = 150;

    /// <summary>
    /// Whether the window can be closed by the user. Default is true.
    /// </summary>
    public bool CanClose { get; set; } = true;

    /// <summary>
    /// Whether the window can be maximized. Default is true.
    /// </summary>
    public bool CanMaximize { get; set; } = true;

    /// <summary>
    /// Whether the window can be resized. Default is true.
    /// </summary>
    public bool CanResize { get; set; } = true;

    /// <summary>
    /// Whether to center the window on screen when opened. Default is false.
    /// When true, Left and Top are ignored.
    /// </summary>
    public bool CenterOnScreen { get; set; }

    /// <summary>
    /// Whether to close the window when clicking outside of it. Default is false.
    /// </summary>
    public bool CloseOnOutsideClick { get; set; }

    /// <summary>
    /// Whether to close the window when pressing Escape. Default is true.
    /// </summary>
    public bool CloseOnEscape { get; set; } = true;

    /// <summary>
    /// Additional CSS class to apply to the window.
    /// </summary>
    public string? CssClass { get; set; }

    /// <summary>
    /// Creates default options.
    /// </summary>
    public static FloatingWindowOptions Default => new();

    /// <summary>
    /// Creates options for a centered window.
    /// </summary>
    public static FloatingWindowOptions Centered(double width = 400, double height = 300) => new()
    {
        Width = width,
        Height = height,
        CenterOnScreen = true
    };

    /// <summary>
    /// Creates options for a modal-like window (centered, closes on escape/outside click).
    /// </summary>
    public static FloatingWindowOptions Modal(double width = 400, double height = 300) => new()
    {
        Width = width,
        Height = height,
        CenterOnScreen = true,
        CloseOnOutsideClick = true,
        CloseOnEscape = true
    };
}

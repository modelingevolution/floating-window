namespace ModelingEvolution.FloatingWindow;

/// <summary>
/// Result returned when a floating window closes.
/// </summary>
public class FloatingWindowResult
{
    /// <summary>
    /// Whether the window was cancelled (closed without explicit result).
    /// </summary>
    public bool Cancelled { get; }

    /// <summary>
    /// The result data returned by the window, if any.
    /// </summary>
    public object? Data { get; }

    private FloatingWindowResult(bool cancelled, object? data)
    {
        Cancelled = cancelled;
        Data = data;
    }

    /// <summary>
    /// Creates a cancelled result.
    /// </summary>
    public static FloatingWindowResult Cancel() => new(true, null);

    /// <summary>
    /// Creates a successful result without data.
    /// </summary>
    public static FloatingWindowResult Ok() => new(false, null);

    /// <summary>
    /// Creates a successful result with data.
    /// </summary>
    /// <typeparam name="T">The type of the data.</typeparam>
    /// <param name="data">The result data.</param>
    public static FloatingWindowResult Ok<T>(T data) => new(false, data);

    /// <summary>
    /// Gets the result data as the specified type.
    /// </summary>
    /// <typeparam name="T">The expected type.</typeparam>
    /// <returns>The data cast to the specified type, or default if cancelled or wrong type.</returns>
    public T? GetData<T>()
    {
        if (Cancelled || Data is null)
            return default;

        return Data is T typed ? typed : default;
    }

    /// <summary>
    /// Tries to get the result data as the specified type.
    /// </summary>
    /// <typeparam name="T">The expected type.</typeparam>
    /// <param name="data">The data if successful.</param>
    /// <returns>True if not cancelled and data is of the correct type.</returns>
    public bool TryGetData<T>(out T? data)
    {
        if (!Cancelled && Data is T typed)
        {
            data = typed;
            return true;
        }
        data = default;
        return false;
    }
}

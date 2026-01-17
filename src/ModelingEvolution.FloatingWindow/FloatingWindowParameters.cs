namespace ModelingEvolution.FloatingWindow;

/// <summary>
/// Container for parameters to pass to a window content component.
/// </summary>
public class FloatingWindowParameters
{
    private readonly Dictionary<string, object?> _parameters = new();

    /// <summary>
    /// Creates an empty parameter set.
    /// </summary>
    public FloatingWindowParameters()
    {
    }

    /// <summary>
    /// Creates a parameter set with initial values.
    /// </summary>
    public FloatingWindowParameters(Dictionary<string, object?> parameters)
    {
        _parameters = new Dictionary<string, object?>(parameters);
    }

    /// <summary>
    /// Adds a parameter.
    /// </summary>
    /// <param name="name">The parameter name (must match the component's [Parameter] property name).</param>
    /// <param name="value">The parameter value.</param>
    /// <returns>This instance for fluent chaining.</returns>
    public FloatingWindowParameters Add(string name, object? value)
    {
        _parameters[name] = value;
        return this;
    }

    /// <summary>
    /// Gets a parameter value.
    /// </summary>
    /// <typeparam name="T">The expected type of the parameter.</typeparam>
    /// <param name="name">The parameter name.</param>
    /// <returns>The parameter value, or default if not found.</returns>
    public T? Get<T>(string name)
    {
        if (_parameters.TryGetValue(name, out var value) && value is T typed)
        {
            return typed;
        }
        return default;
    }

    /// <summary>
    /// Tries to get a parameter value.
    /// </summary>
    /// <typeparam name="T">The expected type of the parameter.</typeparam>
    /// <param name="name">The parameter name.</param>
    /// <param name="value">The parameter value if found.</param>
    /// <returns>True if the parameter was found and had the correct type.</returns>
    public bool TryGet<T>(string name, out T? value)
    {
        if (_parameters.TryGetValue(name, out var obj) && obj is T typed)
        {
            value = typed;
            return true;
        }
        value = default;
        return false;
    }

    /// <summary>
    /// Gets the underlying dictionary for component rendering.
    /// </summary>
    internal IReadOnlyDictionary<string, object?> ToDictionary() => _parameters;
}

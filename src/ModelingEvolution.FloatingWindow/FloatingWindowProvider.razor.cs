using Microsoft.AspNetCore.Components;

namespace ModelingEvolution.FloatingWindow;

/// <summary>
/// Provider component that renders all floating windows.
/// Place this in your main layout, similar to MudDialogProvider.
/// </summary>
public partial class FloatingWindowProvider : ComponentBase, IDisposable
{
    [Inject] private FloatingWindowService Service { get; set; } = default!;

    protected override void OnInitialized()
    {
        Service.OnWindowOpened += HandleWindowEvent;
        Service.OnWindowClosed += HandleWindowEvent;
    }

    private void HandleWindowEvent(IFloatingWindowReference _)
    {
        InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        Service.OnWindowOpened -= HandleWindowEvent;
        Service.OnWindowClosed -= HandleWindowEvent;
    }
}

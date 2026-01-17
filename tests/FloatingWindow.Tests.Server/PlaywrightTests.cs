using FluentAssertions;
using Microsoft.Playwright;
using Xunit;

namespace FloatingWindow.Tests.Server;

/// <summary>
/// End-to-end tests for the FloatingWindow component using Playwright.
/// </summary>
public class PlaywrightTests : IAsyncLifetime
{
    private IPlaywright _playwright = null!;
    private IBrowser _browser = null!;
    private IBrowserContext _context = null!;
    private IPage _page = null!;
    private const string BaseUrl = "https://localhost:5001";

    public async Task InitializeAsync()
    {
        _playwright = await Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true
        });
        _context = await _browser.NewContextAsync(new BrowserNewContextOptions
        {
            IgnoreHTTPSErrors = true
        });
        _page = await _context.NewPageAsync();
    }

    public async Task DisposeAsync()
    {
        await _context.CloseAsync();
        await _browser.CloseAsync();
        _playwright.Dispose();
    }

    [Fact]
    public async Task HomePage_Should_Load_Successfully()
    {
        await _page.GotoAsync(BaseUrl);

        var title = await _page.TitleAsync();
        title.Should().Contain("Floating Window");
    }

    [Fact]
    public async Task OpenWindow1_Button_Should_Show_Window()
    {
        await _page.GotoAsync(BaseUrl);

        await _page.ClickAsync("[data-testid='open-window-1']");
        await _page.WaitForSelectorAsync("[data-testid='floating-window-1']");

        var window = await _page.QuerySelectorAsync("[data-testid='floating-window-1']");
        window.Should().NotBeNull();
    }

    [Fact]
    public async Task Window_Should_Be_Draggable()
    {
        await _page.GotoAsync(BaseUrl);

        await _page.ClickAsync("[data-testid='open-window-1']");
        await _page.WaitForSelectorAsync(".floating-window-header");

        var header = await _page.QuerySelectorAsync(".floating-window-header");
        var boundingBox = await header!.BoundingBoxAsync();

        await _page.Mouse.MoveAsync(boundingBox!.X + 50, boundingBox.Y + 10);
        await _page.Mouse.DownAsync();
        await _page.Mouse.MoveAsync(boundingBox.X + 150, boundingBox.Y + 100);
        await _page.Mouse.UpAsync();

        var window = await _page.QuerySelectorAsync(".floating-window");
        var newBox = await window!.BoundingBoxAsync();

        newBox!.X.Should().BeGreaterThan(boundingBox.X);
        newBox.Y.Should().BeGreaterThan(boundingBox.Y);
    }

    [Fact]
    public async Task Window_Close_Button_Should_Hide_Window()
    {
        await _page.GotoAsync(BaseUrl);

        await _page.ClickAsync("[data-testid='open-window-1']");
        await _page.WaitForSelectorAsync("[data-testid='floating-window-1']");

        await _page.ClickAsync(".floating-window-btn:has(svg[data-testid='CloseIcon'])");

        await _page.WaitForTimeoutAsync(500);
        var window = await _page.QuerySelectorAsync("[data-testid='floating-window-1']");
        window.Should().BeNull();
    }

    [Fact]
    public async Task Window_Minimize_Button_Should_Collapse_Content()
    {
        await _page.GotoAsync(BaseUrl);

        await _page.ClickAsync("[data-testid='open-window-1']");
        await _page.WaitForSelectorAsync(".floating-window-content");

        await _page.ClickAsync(".floating-window-btn:has(svg[data-testid='MinimizeIcon'])");

        await _page.WaitForTimeoutAsync(300);
        var content = await _page.QuerySelectorAsync(".floating-window-content");
        content.Should().BeNull();

        var window = await _page.QuerySelectorAsync(".floating-window.minimized");
        window.Should().NotBeNull();
    }

    [Fact]
    public async Task Window_Input_Should_Be_Functional()
    {
        await _page.GotoAsync(BaseUrl);

        await _page.ClickAsync("[data-testid='open-window-1']");
        await _page.WaitForSelectorAsync("[data-testid='window1-input']");

        await _page.FillAsync("[data-testid='window1-input'] input", "Test input");

        var value = await _page.InputValueAsync("[data-testid='window1-input'] input");
        value.Should().Be("Test input");
    }

    [Fact]
    public async Task Multiple_Windows_Should_Open_Independently()
    {
        await _page.GotoAsync(BaseUrl);

        await _page.ClickAsync("[data-testid='open-window-1']");
        await _page.ClickAsync("[data-testid='open-window-2']");

        await _page.WaitForSelectorAsync("[data-testid='floating-window-1']");
        await _page.WaitForSelectorAsync("[data-testid='floating-window-2']");

        var windows = await _page.QuerySelectorAllAsync(".floating-window");
        windows.Count.Should().Be(2);
    }
}

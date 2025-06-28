using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using Xunit;

namespace AttendancePlatform.Tests.Frontend
{
    public abstract class FrontendTestBase : IAsyncLifetime
    {
        protected IBrowser Browser { get; private set; }
        protected IBrowserContext Context { get; private set; }
        protected IPage Page { get; private set; }

        private const string BaseUrl = "http://localhost:3000";

        public async Task InitializeAsync()
        {
            var playwright = await Playwright.CreateAsync();
            Browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = true,
                Args = new[] { "--no-sandbox", "--disable-dev-shm-usage" }
            });

            Context = await Browser.NewContextAsync(new BrowserNewContextOptions
            {
                ViewportSize = new ViewportSize { Width = 1920, Height = 1080 },
                Locale = "en-US"
            });

            Page = await Context.NewPageAsync();
        }

        public async Task DisposeAsync()
        {
            await Page?.CloseAsync();
            await Context?.CloseAsync();
            await Browser?.CloseAsync();
        }

        protected async Task NavigateToAsync(string path = "")
        {
            await Page.GotoAsync($"{BaseUrl}{path}");
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        protected async Task LoginAsAsync(string email = "admin@hudur.sa", string password = "AdminPassword123!")
        {
            await NavigateToAsync("/login");
            
            await Page.FillAsync("[data-testid=email-input]", email);
            await Page.FillAsync("[data-testid=password-input]", password);
            await Page.ClickAsync("[data-testid=login-button]");
            
            await Page.WaitForURLAsync("**/dashboard");
        }

        protected async Task<string> TakeScreenshotAsync(string name)
        {
            var screenshotPath = Path.Combine("screenshots", $"{name}_{DateTime.Now:yyyyMMdd_HHmmss}.png");
            Directory.CreateDirectory(Path.GetDirectoryName(screenshotPath));
            
            await Page.ScreenshotAsync(new PageScreenshotOptions
            {
                Path = screenshotPath,
                FullPage = true
            });
            
            return screenshotPath;
        }

        protected async Task WaitForElementAsync(string selector, int timeoutMs = 5000)
        {
            await Page.WaitForSelectorAsync(selector, new PageWaitForSelectorOptions
            {
                Timeout = timeoutMs
            });
        }

        protected async Task AssertElementExistsAsync(string selector)
        {
            var element = await Page.QuerySelectorAsync(selector);
            Assert.NotNull(element);
        }

        protected async Task AssertElementTextAsync(string selector, string expectedText)
        {
            var element = await Page.QuerySelectorAsync(selector);
            Assert.NotNull(element);
            
            var actualText = await element.TextContentAsync();
            Assert.Equal(expectedText, actualText?.Trim());
        }
    }
}

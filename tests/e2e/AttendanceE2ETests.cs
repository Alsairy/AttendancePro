using Microsoft.Playwright;
using Xunit;
using FluentAssertions;

namespace AttendancePlatform.Tests.E2E
{
    public class AttendanceE2ETests : IAsyncLifetime
    {
        private IPlaywright _playwright;
        private IBrowser _browser;
        private IPage _page;

        public async Task InitializeAsync()
        {
            _playwright = await Playwright.CreateAsync();
            _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = true
            });
            _page = await _browser.NewPageAsync();
        }

        public async Task DisposeAsync()
        {
            await _browser?.CloseAsync();
            _playwright?.Dispose();
        }

        [Fact]
        public async Task LoginFlow_ShouldWorkEndToEnd()
        {
            await _page.GotoAsync("http://localhost:3000/login");
            
            await _page.FillAsync("[data-testid=email-input]", "test@example.com");
            await _page.FillAsync("[data-testid=password-input]", "password123");
            await _page.ClickAsync("[data-testid=login-button]");
            
            await _page.WaitForURLAsync("**/dashboard");
            
            var title = await _page.TitleAsync();
            title.Should().Contain("Dashboard");
        }

        [Fact]
        public async Task AttendanceCheckIn_ShouldWorkEndToEnd()
        {
            await LoginAsTestUser();
            
            await _page.GotoAsync("http://localhost:3000/attendance");
            await _page.ClickAsync("[data-testid=checkin-button]");
            
            await _page.WaitForSelectorAsync("[data-testid=checkin-success]");
            
            var successMessage = await _page.TextContentAsync("[data-testid=checkin-success]");
            successMessage.Should().Contain("checked in successfully");
        }

        private async Task LoginAsTestUser()
        {
            await _page.GotoAsync("http://localhost:3000/login");
            await _page.FillAsync("[data-testid=email-input]", "test@example.com");
            await _page.FillAsync("[data-testid=password-input]", "password123");
            await _page.ClickAsync("[data-testid=login-button]");
            await _page.WaitForURLAsync("**/dashboard");
        }
    }
}

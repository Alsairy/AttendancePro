using Xunit;
using FluentAssertions;
using Microsoft.Playwright;

namespace AttendancePlatform.Tests.Frontend
{
    public class ComponentTests : FrontendTestBase
    {
        [Fact]
        public async Task LoginPage_ShouldRenderCorrectly()
        {
            await NavigateToAsync("/login");

            await AssertElementExistsAsync("[data-testid=login-form]");
            await AssertElementExistsAsync("[data-testid=email-input]");
            await AssertElementExistsAsync("[data-testid=password-input]");
            await AssertElementExistsAsync("[data-testid=login-button]");
            
            await AssertElementTextAsync("h1", "Sign In to Hudur");
        }

        [Fact]
        public async Task Dashboard_ShouldDisplayUserInformation()
        {
            await LoginAsAsync();

            await AssertElementExistsAsync("[data-testid=user-profile]");
            await AssertElementExistsAsync("[data-testid=attendance-status]");
            await AssertElementExistsAsync("[data-testid=quick-actions]");
        }

        [Fact]
        public async Task AttendancePage_ShouldAllowCheckIn()
        {
            await LoginAsAsync();
            await NavigateToAsync("/attendance");

            await AssertElementExistsAsync("[data-testid=check-in-button]");
            
            await Page.ClickAsync("[data-testid=check-in-button]");
            await WaitForElementAsync("[data-testid=check-in-success]");
            
            await AssertElementExistsAsync("[data-testid=check-out-button]");
        }

        [Fact]
        public async Task LeaveRequestForm_ShouldValidateInput()
        {
            await LoginAsAsync();
            await NavigateToAsync("/leave/request");

            await Page.ClickAsync("[data-testid=submit-leave-request]");
            
            await WaitForElementAsync("[data-testid=validation-error]");
            await AssertElementExistsAsync("[data-testid=validation-error]");
        }

        [Fact]
        public async Task Navigation_ShouldWorkCorrectly()
        {
            await LoginAsAsync();

            var navigationItems = new[]
            {
                ("[data-testid=nav-dashboard]", "/dashboard"),
                ("[data-testid=nav-attendance]", "/attendance"),
                ("[data-testid=nav-leave]", "/leave"),
                ("[data-testid=nav-reports]", "/reports")
            };

            foreach (var (selector, expectedPath) in navigationItems)
            {
                await Page.ClickAsync(selector);
                await Page.WaitForURLAsync($"**{expectedPath}");
                
                var currentUrl = Page.Url;
                currentUrl.Should().Contain(expectedPath);
            }
        }

        [Fact]
        public async Task ResponsiveDesign_ShouldWorkOnMobile()
        {
            await Context.SetViewportSizeAsync(375, 667); // iPhone SE size
            await NavigateToAsync("/dashboard");

            await AssertElementExistsAsync("[data-testid=mobile-menu-button]");
            
            await Page.ClickAsync("[data-testid=mobile-menu-button]");
            await WaitForElementAsync("[data-testid=mobile-navigation]");
            
            await AssertElementExistsAsync("[data-testid=mobile-navigation]");
        }

        [Fact]
        public async Task FaceRecognition_ShouldInitializeCamera()
        {
            await LoginAsAsync();
            await NavigateToAsync("/face-recognition");

            await Page.ClickAsync("[data-testid=start-camera-button]");
            
            await WaitForElementAsync("[data-testid=camera-preview]", 10000);
            
            await AssertElementExistsAsync("[data-testid=camera-preview]");
            await AssertElementExistsAsync("[data-testid=capture-button]");
        }

        [Fact]
        public async Task GeofenceMap_ShouldDisplayLocation()
        {
            await LoginAsAsync();
            await NavigateToAsync("/geofence");

            await WaitForElementAsync("[data-testid=geofence-map]", 10000);
            
            await AssertElementExistsAsync("[data-testid=geofence-map]");
            await AssertElementExistsAsync("[data-testid=current-location]");
        }

        [Fact]
        public async Task NotificationCenter_ShouldDisplayNotifications()
        {
            await LoginAsAsync();
            
            await Page.ClickAsync("[data-testid=notification-bell]");
            await WaitForElementAsync("[data-testid=notification-dropdown]");
            
            await AssertElementExistsAsync("[data-testid=notification-dropdown]");
            await AssertElementExistsAsync("[data-testid=notification-list]");
        }

        [Fact]
        public async Task ThemeToggle_ShouldSwitchThemes()
        {
            await LoginAsAsync();
            
            var initialTheme = await Page.GetAttributeAsync("html", "data-theme");
            
            await Page.ClickAsync("[data-testid=theme-toggle]");
            await Page.WaitForTimeoutAsync(500); // Wait for theme transition
            
            var newTheme = await Page.GetAttributeAsync("html", "data-theme");
            newTheme.Should().NotBe(initialTheme);
        }

        [Fact]
        public async Task ErrorBoundary_ShouldHandleErrors()
        {
            await NavigateToAsync("/error-test");
            
            await WaitForElementAsync("[data-testid=error-boundary]");
            await AssertElementExistsAsync("[data-testid=error-boundary]");
            await AssertElementTextAsync("[data-testid=error-message]", "Something went wrong");
        }

        [Fact]
        public async Task LoadingStates_ShouldDisplayCorrectly()
        {
            await NavigateToAsync("/dashboard");
            
            var loadingSpinner = await Page.QuerySelectorAsync("[data-testid=loading-spinner]");
            
            if (loadingSpinner != null)
            {
                await Page.WaitForSelectorAsync("[data-testid=loading-spinner]", new PageWaitForSelectorOptions
                {
                    State = WaitForSelectorState.Detached,
                    Timeout = 10000
                });
            }
            
            await AssertElementExistsAsync("[data-testid=dashboard-content]");
        }
    }
}

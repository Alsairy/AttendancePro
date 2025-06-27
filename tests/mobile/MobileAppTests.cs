using OpenQA.Selenium;
using Xunit;
using FluentAssertions;

namespace AttendancePlatform.Tests.Mobile
{
    public class MobileAppTests : MobileTestBase
    {
        [Fact]
        public async Task MobileApp_ShouldLaunchSuccessfully()
        {
            await WaitForElementAsync(By.Id("splash-screen"), 10000);
            AssertElementExists(By.Id("splash-screen"));
        }

        [Fact]
        public async Task Login_ShouldWorkOnMobile()
        {
            await LoginAsync();
            AssertElementExists(By.Id("dashboard-screen"));
        }

        [Fact]
        public async Task AttendanceCheckIn_ShouldWorkOnMobile()
        {
            await LoginAsync();
            
            var checkInButton = Driver.FindElement(By.Id("check-in-button"));
            checkInButton.Click();
            
            await WaitForElementAsync(By.Id("check-in-success"), 5000);
            AssertElementExists(By.Id("check-in-success"));
        }

        [Fact]
        public async Task FaceRecognition_ShouldInitializeOnMobile()
        {
            await LoginAsync();
            
            var faceRecognitionTab = Driver.FindElement(By.Id("face-recognition-tab"));
            faceRecognitionTab.Click();
            
            await WaitForElementAsync(By.Id("camera-preview"), 10000);
            AssertElementExists(By.Id("camera-preview"));
        }

        [Fact]
        public async Task OfflineMode_ShouldWorkCorrectly()
        {
            await LoginAsync();
            
            Driver.SetNetworkConnection(ConnectionType.None);
            
            var offlineIndicator = Driver.FindElement(By.Id("offline-indicator"));
            offlineIndicator.Should().NotBeNull();
            
            var checkInButton = Driver.FindElement(By.Id("check-in-button"));
            checkInButton.Click();
            
            await WaitForElementAsync(By.Id("offline-queue-message"), 5000);
            AssertElementExists(By.Id("offline-queue-message"));
        }

        [Fact]
        public async Task BiometricAuthentication_ShouldWorkOnMobile()
        {
            var biometricButton = Driver.FindElement(By.Id("biometric-login-button"));
            biometricButton.Click();
            
            await WaitForElementAsync(By.Id("biometric-prompt"), 5000);
            AssertElementExists(By.Id("biometric-prompt"));
        }

        [Fact]
        public async Task LocationServices_ShouldRequestPermission()
        {
            await LoginAsync();
            
            var locationButton = Driver.FindElement(By.Id("enable-location-button"));
            locationButton.Click();
            
            await WaitForElementAsync(By.Id("location-permission-dialog"), 5000);
            AssertElementExists(By.Id("location-permission-dialog"));
        }

        [Fact]
        public async Task PushNotifications_ShouldBeConfigurable()
        {
            await LoginAsync();
            
            var settingsTab = Driver.FindElement(By.Id("settings-tab"));
            settingsTab.Click();
            
            var notificationSettings = Driver.FindElement(By.Id("notification-settings"));
            notificationSettings.Click();
            
            AssertElementExists(By.Id("push-notification-toggle"));
        }

        [Fact]
        public async Task DataSync_ShouldWorkWhenBackOnline()
        {
            await LoginAsync();
            
            Driver.SetNetworkConnection(ConnectionType.None);
            
            var checkInButton = Driver.FindElement(By.Id("check-in-button"));
            checkInButton.Click();
            
            Driver.SetNetworkConnection(ConnectionType.All);
            
            await WaitForElementAsync(By.Id("sync-complete-message"), 10000);
            AssertElementExists(By.Id("sync-complete-message"));
        }

        [Fact]
        public async Task AppNavigation_ShouldWorkSmoothly()
        {
            await LoginAsync();
            
            var tabs = new[]
            {
                "dashboard-tab",
                "attendance-tab",
                "leave-tab",
                "profile-tab"
            };

            foreach (var tabId in tabs)
            {
                var tab = Driver.FindElement(By.Id(tabId));
                tab.Click();
                
                await Task.Delay(1000); // Wait for navigation
                
                var activeTab = Driver.FindElement(By.CssSelector("[data-active='true']"));
                activeTab.Should().NotBeNull();
            }
        }
    }
}

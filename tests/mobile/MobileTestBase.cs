using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.iOS;
using OpenQA.Selenium.Appium.Service;
using Xunit;

namespace AttendancePlatform.Tests.Mobile
{
    public abstract class MobileTestBase : IAsyncLifetime
    {
        protected AppiumDriver Driver { get; private set; }
        protected AppiumLocalService AppiumService { get; private set; }

        public async Task InitializeAsync()
        {
            AppiumService = new AppiumServiceBuilder()
                .UsingAnyFreePort()
                .Build();

            AppiumService.Start();

            var options = GetAppiumOptions();
            Driver = new AndroidDriver(AppiumService, options);
            
            await Task.CompletedTask;
        }

        public async Task DisposeAsync()
        {
            Driver?.Quit();
            AppiumService?.Dispose();
            await Task.CompletedTask;
        }

        private AppiumOptions GetAppiumOptions()
        {
            var options = new AppiumOptions();
            
            options.AddAdditionalCapability("platformName", "Android");
            options.AddAdditionalCapability("platformVersion", "11.0");
            options.AddAdditionalCapability("deviceName", "Android Emulator");
            options.AddAdditionalCapability("app", GetAppPath());
            options.AddAdditionalCapability("automationName", "UiAutomator2");
            options.AddAdditionalCapability("newCommandTimeout", 300);
            options.AddAdditionalCapability("autoGrantPermissions", true);

            return options;
        }

        private string GetAppPath()
        {
            return Path.Combine(
                Directory.GetCurrentDirectory(),
                "..", "..", "..", "..", "..",
                "src", "mobile", "AttendanceMobile", "android", "app", "build", "outputs", "apk", "debug",
                "app-debug.apk"
            );
        }

        protected async Task LoginAsync(string email = "test@hudur.sa", string password = "TestPassword123!")
        {
            var emailField = Driver.FindElement(By.Id("email-input"));
            var passwordField = Driver.FindElement(By.Id("password-input"));
            var loginButton = Driver.FindElement(By.Id("login-button"));

            emailField.SendKeys(email);
            passwordField.SendKeys(password);
            loginButton.Click();

            await WaitForElementAsync(By.Id("dashboard-screen"), 10000);
        }

        protected async Task WaitForElementAsync(By locator, int timeoutMs = 5000)
        {
            var wait = new OpenQA.Selenium.Support.UI.WebDriverWait(Driver, TimeSpan.FromMilliseconds(timeoutMs));
            wait.Until(driver => driver.FindElement(locator));
            await Task.CompletedTask;
        }

        protected void AssertElementExists(By locator)
        {
            var element = Driver.FindElement(locator);
            Assert.NotNull(element);
        }

        protected void AssertElementText(By locator, string expectedText)
        {
            var element = Driver.FindElement(locator);
            Assert.NotNull(element);
            Assert.Equal(expectedText, element.Text);
        }

        protected async Task TakeScreenshotAsync(string name)
        {
            var screenshot = ((ITakesScreenshot)Driver).GetScreenshot();
            var screenshotPath = Path.Combine("screenshots", $"mobile_{name}_{DateTime.Now:yyyyMMdd_HHmmss}.png");
            
            Directory.CreateDirectory(Path.GetDirectoryName(screenshotPath));
            screenshot.SaveAsFile(screenshotPath);
            
            await Task.CompletedTask;
        }

        protected void SwipeUp()
        {
            var size = Driver.Manage().Window.Size;
            var startX = size.Width / 2;
            var startY = (int)(size.Height * 0.8);
            var endY = (int)(size.Height * 0.2);

            var touchAction = new OpenQA.Selenium.Appium.Interactions.PointerInputDevice(PointerKind.Touch);
            var sequence = new OpenQA.Selenium.Interactions.ActionSequence(touchAction);
            
            sequence.AddAction(touchAction.CreatePointerMove(CoordinateOrigin.Viewport, startX, startY, TimeSpan.Zero));
            sequence.AddAction(touchAction.CreatePointerDown(MouseButton.Left));
            sequence.AddAction(touchAction.CreatePointerMove(CoordinateOrigin.Viewport, startX, endY, TimeSpan.FromMilliseconds(1000)));
            sequence.AddAction(touchAction.CreatePointerUp(MouseButton.Left));

            Driver.PerformActions(new[] { sequence });
        }

        protected void SwipeDown()
        {
            var size = Driver.Manage().Window.Size;
            var startX = size.Width / 2;
            var startY = (int)(size.Height * 0.2);
            var endY = (int)(size.Height * 0.8);

            var touchAction = new OpenQA.Selenium.Appium.Interactions.PointerInputDevice(PointerKind.Touch);
            var sequence = new OpenQA.Selenium.Interactions.ActionSequence(touchAction);
            
            sequence.AddAction(touchAction.CreatePointerMove(CoordinateOrigin.Viewport, startX, startY, TimeSpan.Zero));
            sequence.AddAction(touchAction.CreatePointerDown(MouseButton.Left));
            sequence.AddAction(touchAction.CreatePointerMove(CoordinateOrigin.Viewport, startX, endY, TimeSpan.FromMilliseconds(1000)));
            sequence.AddAction(touchAction.CreatePointerUp(MouseButton.Left));

            Driver.PerformActions(new[] { sequence });
        }
    }
}

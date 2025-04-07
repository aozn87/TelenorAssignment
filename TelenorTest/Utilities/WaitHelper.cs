using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;

namespace TelenorTest.Utilities
{
    public static class WaitHelper
    {
        public static IWebElement WaitForElementVisible(IWebDriver driver, By by, int timeoutSeconds = 15)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutSeconds));
            return wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(by));
        }

        public static bool WaitForText(IWebDriver driver, By by, string text, int timeoutSeconds = 15)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutSeconds));
            return wait.Until(d =>
            {
                var element = d.FindElement(by);
                return element.Text.Contains(text);
            });
        }

        public static bool WaitForElementCountGreaterThan(IWebDriver driver, By by, int count = 0, int timeoutSeconds = 10)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutSeconds));
            return wait.Until(d => d.FindElements(by).Count > count);
        }
    }
}

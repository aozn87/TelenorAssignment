using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;

namespace TelenorTest.Utilities
{
    public static class WebDriverFactory
    {
        public static IWebDriver CreateDriver()
        {
            var options = new ChromeOptions();
            options.AddArgument("--start-maximized");
            options.AddArgument("--disable-notifications");

            // headless mode if needed
            // options.AddArgument("--headless");

            return new ChromeDriver(options);
        }
    }
}

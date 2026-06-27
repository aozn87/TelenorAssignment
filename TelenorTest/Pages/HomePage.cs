using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Linq;
using TelenorTest.Utilities;

namespace TelenorTest.Pages
{
    public class HomePage
    {
        private readonly IWebDriver _driver;

        private readonly WebDriverWait _wait;

        public HomePage(IWebDriver driver)
        {
            _driver = driver;
            _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        private IWebElement GetAcceptCookiesButton()
        {
            return _wait.Until(d => d.FindElement(By.Id("onetrust-accept-btn-handler")));
        }

        private IWebElement GetBredbandLink()
        {
            // Prefer an anchor with the exact href to the broadband shopping page
            var locators = new By[] {
                By.CssSelector("a[href='/handla/bredband/']"),
                By.CssSelector("a[href*='/handla/bredband']"),
                By.XPath("//a[contains(translate(@href,'ABCDEFGHIJKLMNOPQRSTUVWXYZ','abcdefghijklmnopqrstuvwxyz'),'/handla/bredband')]")
            };

            return _wait.Until(driver =>
            {
                foreach (var by in locators)
                {
                    try
                    {
                        var el = driver.FindElement(by);
                        if (el != null && el.Displayed && el.Enabled)
                            return el;
                    }
                    catch (NoSuchElementException) { }
                }
                return null;
            });
        }

        public void AcceptCookies()
        {
            try
            {
                var btn = GetAcceptCookiesButton();
                btn.Click();
            }
            catch (WebDriverTimeoutException)
            {
                // Cookie button not present within wait — ignore
            }
            catch (NoSuchElementException)
            {
                // Not present — ignore
            }
        }

        public void NavigateToBroadbandPage()
        {
            // Try to click the 'Bredband via fiber' link directly from the header/submenu
            try
            {
                var fiberBy = By.XPath("//a[contains(translate(normalize-space(.), 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'),'bredband via fiber') and contains(translate(@href,'ABCDEFGHIJKLMNOPQRSTUVWXYZ','abcdefghijklmnopqrstuvwxyz'),'/handla/bredband')]");

                // Try to find it visible first
                var fiber = _driver.FindElements(fiberBy).FirstOrDefault(e => e.Displayed && e.Enabled);

                if (fiber == null)
                {
                    // Try opening the top 'Bredband' menu to reveal submenu
                    try
                    {
                        var topBy = By.XPath("//a[contains(normalize-space(.),'Bredband') or contains(translate(normalize-space(.),'ABCDEFGHIJKLMNOPQRSTUVWXYZ','abcdefghijklmnopqrstuvwxyz'),'bredband')]");
                        var top = _driver.FindElements(topBy).FirstOrDefault(e => e.Displayed && e.Enabled);
                        top?.Click();

                        // wait for fiber link to appear
                        fiber = _wait.Until(d => d.FindElements(fiberBy).FirstOrDefault(el => el.Displayed && el.Enabled));
                    }
                    catch { }
                }

                if (fiber != null)
                {
                    fiber.Click();
                    return;
                }
            }
            catch { }

            // Fallback: navigate directly to the broadband shopping URL and try clicking the fiber option there
            var url = TestData.BaseUrl?.TrimEnd('/') + "/handla/bredband/";
            _driver.Navigate().GoToUrl(url);

            try
            {
                var fiberOnPageBy = By.XPath("//a[contains(translate(normalize-space(.), 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'),'bredband via fiber')] | //button[contains(translate(normalize-space(.), 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'),'bredband via fiber')]");
                var fiberOnPage = _wait.Until(d => d.FindElements(fiberOnPageBy).FirstOrDefault(el => el.Displayed && el.Enabled));
                fiberOnPage?.Click();
            }
            catch { }
        }
    }
}

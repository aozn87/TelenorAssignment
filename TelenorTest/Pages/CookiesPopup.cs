using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace TelenorTest.Pages
{
    public class CookiesPopup
    {
        private readonly IWebDriver _driver;
        private WebDriverWait _wait;


        public CookiesPopup(IWebDriver driver)
        {
            _driver = driver;
            _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

        }

        private IWebElement AcceptCookiesButton => _driver.FindElement(By.Id("onetrust-accept-btn-handler"));

        public void AcceptCookies()
        {
            // Wait until the Accept Cookies button is visible
            _wait.Until(d => AcceptCookiesButton.Displayed && AcceptCookiesButton.Enabled);
            AcceptCookiesButton.Click();
        }
    }
}

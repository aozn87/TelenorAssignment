using OpenQA.Selenium;

namespace TelenorTest.Pages
{
    public class HomePage
    {
        private readonly IWebDriver _driver;

        public HomePage(IWebDriver driver)
        {
            _driver = driver;
        }

        private IWebElement AcceptCookiesButton => _driver.FindElement(By.Id("onetrust-accept-btn-handler"));
        private IWebElement HandlaMenu => _driver.FindElement(By.XPath("//a[@data-test='Handla']"));
        private IWebElement BroadbandMenu => _driver.FindElement(By.XPath("//div[@class='tn-page-header__slide-down']//a[@class='cnt-link tn-page-header__secondary-item__link tn-page-header__secondary-item__link--has-children']//span[contains(text(),'Bredband')]"));

        public void AcceptCookies()
        {
            AcceptCookiesButton.Click();
        }

        public void NavigateToBroadbandPage()
        {
            HandlaMenu.Click();
            BroadbandMenu.Click();
        }
    }
}

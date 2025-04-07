using NUnit.Framework;
using OpenQA.Selenium;
using TelenorTest.Utilities;
using TelenorTest.Pages;

namespace TelenorTest.Tests
{
    public class BaseTest
    {
        protected IWebDriver _driver;
        protected HomePage _homePage;
        protected BroadbandPage _broadbandPage;
        protected CookiesPopup _cookiesPopup;

        [SetUp]
        public void SetUp()
        {
            _driver = WebDriverFactory.CreateDriver();
            _driver.Navigate().GoToUrl(TestData.BaseUrl);
            


            _homePage = new HomePage(_driver);
            _broadbandPage = new BroadbandPage(_driver);
            _cookiesPopup = new CookiesPopup(_driver);

            // Accept cookies 
            _cookiesPopup.AcceptCookies();

        }

        [TearDown]
        public void TearDown()
        {
            var testContext = TestContext.CurrentContext;

            if (testContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Failed)
            {
                ScreenshotHelper.TakeScreenshot(_driver, testContext.Test.Name);
            }
            _driver?.Quit();
            _driver.Dispose();
        }
    }
}

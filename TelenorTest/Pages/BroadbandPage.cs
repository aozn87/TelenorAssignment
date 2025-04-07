using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace TelenorTest.Pages
{
    public class BroadbandPage
    {
        private readonly IWebDriver _driver;
        private WebDriverWait _wait;
        
        private static readonly Random _rand = new Random();

        public BroadbandPage(IWebDriver driver)
        {
            _driver = driver;
            _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        private IWebElement AddressInput => _driver.FindElement(By.CssSelector("input[placeholder='Sök adress']")); 
       
        
        public void EnterAddress(string address)
        {
            if (AddressInput== null)
            {
                throw new Exception("addressField is NULL");
            }

            AddressInput.Clear();
            AddressInput.SendKeys(address);

            // Wait until autocomplete
            _wait.Until(d => d.FindElements(By.CssSelector("#address-list li")).Count > 0);

            // Select the first option from autocomplete
            var firstAddress = _driver.FindElement(By.CssSelector("#address-list li"));
            firstAddress.Click();

        }
        
        public void SelectRandomApartment()
        {
            // Find the dropdown
            var dropdown = _wait.Until(d => d.FindElement(By.CssSelector("select[data-v-f06f59fd]")));
            var selectElement = new SelectElement(dropdown);
            var options = selectElement.Options;

            // Select a random option beside "Välj"
            int startIndex = options[0].Text.Contains("Välj") ? 1 : 0;
            int randomIndex = new Random().Next(startIndex, options.Count);
            var selectedOption = options[randomIndex];
            string value = selectedOption.GetAttribute("value");

            // 3. Select value
            selectElement.SelectByValue(value);

            //Initiate js for Vue
            var js = (IJavaScriptExecutor)_driver;
            js.ExecuteScript(@"
            const dropdown = arguments[0];
            dropdown.dispatchEvent(new Event('change', { bubbles: true }));
            dropdown.dispatchEvent(new Event('input', { bubbles: true }));
            dropdown.dispatchEvent(new Event('blur', { bubbles: true }));", dropdown);

            // 5. Wait for the offers
            _wait.Until(driver =>
            {
                var offers = driver.FindElements(By.CssSelector("[data-test='product-grid']"));
                return offers.Any(e => e.Displayed);
            });
        }

        public bool IsProductDisplayed(string productName)
        {
            try
            {
                _wait.Until(d => d.FindElements(By.CssSelector("[data-test='product-grid']")).Count > 0);

                var productElements = _driver.FindElements(By.CssSelector("[data-test='product-grid']"));

                foreach (var product in productElements)
                {
                    if (product.Text.Contains(productName, StringComparison.OrdinalIgnoreCase))
                    {
                        Console.WriteLine($"Product grid is displayed and {productName} is included");
                        return true;
                    }
                }

                Console.WriteLine($"Product not found: {productName}");
                return false;
            }
            catch (WebDriverTimeoutException)
            {
                Console.WriteLine("Timeout: Offers could not load");
                return false;
            }
        }

    }
}

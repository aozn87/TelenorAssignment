using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Linq;

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
            _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
        }

        private IWebElement GetAddressInput()
        {
            // Prefer the address input inside the broadband page's address-search area
            var preferred = new By[] {
                By.CssSelector("[data-test='address-search'] input"),
                By.CssSelector("section.address-search input"),
                By.CssSelector(".address-search-input_wrapper input"),
                By.CssSelector("input.address-search-input__wrapper__input")
            };

            foreach (var by in preferred)
            {
                try
                {
                    var el = _driver.FindElements(by).FirstOrDefault(e => e.Displayed && e.Enabled);
                    if (el != null)
                        return el;
                }
                catch { }
            }

            // Fallback to general address-like inputs (legacy)
            var locators = new By[] {
                By.CssSelector("input[placeholder='Sök adress']"),
                By.CssSelector("input[placeholder*='Sök']"),
                By.CssSelector("input[placeholder*='adress']"),
                By.XPath("//input[contains(translate(@placeholder,'ABCDEFGHIJKLMNOPQRSTUVWXYZ','abcdefghijklmnopqrstuvwxyz'),'sök') or contains(translate(@placeholder,'ABCDEFGHIJKLMNOPQRSTUVWXYZ','abcdefghijklmnopqrstuvwxyz'),'search') or contains(translate(@placeholder,'ABCDEFGHIJKLMNOPQRSTUVWXYZ','abcdefghijklmnopqrstuvwxyz'),'adress') or contains(translate(@name,'ABCDEFGHIJKLMNOPQRSTUVWXYZ','abcdefghijklmnopqrstuvwxyz'),'address')]")
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

        public void EnterAddress(string address)
        {
            var addressInput = GetAddressInput();
            if (addressInput == null)
                throw new Exception("addressField is NULL");
            try { addressInput.SendKeys(Keys.Escape); } catch { }
            try { addressInput.Click(); } catch { }

            addressInput.Clear();
            addressInput.SendKeys(address);

            try
            {
                var firstSuggestion = WaitForFirstAddressSuggestion(addressInput);
                firstSuggestion.Click();
            }
            catch (WebDriverTimeoutException)
            {
                try
                {
                    addressInput.SendKeys(Keys.ArrowDown);
                    addressInput.SendKeys(Keys.Enter);
                }
                catch { }
            }

            try
            {
                _wait.Until(d => d.FindElements(By.CssSelector("[data-test='product-grid']")).Any()
                                  || d.FindElements(By.XPath("//*[contains(translate(., 'ABCDEFGHIJKLMNOPQRSTUVWXYZ','abcdefghijklmnopqrstuvwxyz'),'bredband via 5g')]")).Any());
            }
            catch (WebDriverTimeoutException)
            {
                // leave flow to caller; product check will handle absence
            }
        }

        private IWebElement WaitForFirstAddressSuggestion(IWebElement addressInput)
        {
            return _wait.Until(d =>
            {
                var ariaControls = addressInput.GetAttribute("aria-controls");
                if (!string.IsNullOrEmpty(ariaControls))
                {
                    try
                    {
                        var list = d.FindElement(By.Id(ariaControls));
                        var item = list.FindElements(By.CssSelector("li, button, a, div, span, [role='option']"))
                            .FirstOrDefault(e => e.Displayed && e.Enabled && !string.IsNullOrWhiteSpace(e.Text));
                        if (item != null)
                            return item;
                    }
                    catch (NoSuchElementException) { }
                }

                var candidates = d.FindElements(By.CssSelector("#address-list li, #address-list button, #address-list a, #address-list div, #address-list span, ul[role='listbox'] li, ul[role='listbox'] button, ul[role='listbox'] a, ul[role='listbox'] div, ul[role='listbox'] span, [role='option']"));
                return candidates.FirstOrDefault(e => e.Displayed && e.Enabled && !string.IsNullOrWhiteSpace(e.Text));
            });
        }

        public void SelectRandomApartment()
        {
            // Wait for the apartment dropdown to load and be visible
            var dropdown = _wait.Until(d =>
            {
                var selects = d.FindElements(By.CssSelector("select[data-v-f06f59fd], select[id*='apartment-number-options-select'], select[data-test*='apartment-number-select']")).Where(s => s.Displayed && s.Enabled).ToList();
                return selects.FirstOrDefault();
            });

            if (dropdown == null)
            {
                Console.WriteLine("No apartment dropdown found - skipping apartment selection");
                return;
            }

            var selectElement = new SelectElement(dropdown);
            var options = selectElement.Options;

            if (options.Count <= 1)
            {
                Console.WriteLine("Apartment dropdown has no selectable options");
                return;
            }

            var validOptions = options.Where(o => !string.IsNullOrWhiteSpace(o.Text) && !o.Text.Contains("Välj", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(o.GetAttribute("value"))).ToList();
            if (!validOptions.Any())
            {
                validOptions = options.Where(o => !string.IsNullOrWhiteSpace(o.Text)).ToList();
            }

            var selectedOption = validOptions[_rand.Next(validOptions.Count)];
            Console.WriteLine($"Selecting random apartment option: {selectedOption.Text}");

            selectElement.SelectByValue(selectedOption.GetAttribute("value"));

            // Some Vue bindings require manual event dispatch after selection
            var js = (IJavaScriptExecutor)_driver;
            js.ExecuteScript(@"
            const dropdown = arguments[0];
            dropdown.dispatchEvent(new Event('change', { bubbles: true }));
            dropdown.dispatchEvent(new Event('input', { bubbles: true }));
            dropdown.dispatchEvent(new Event('blur', { bubbles: true }));", dropdown);

            // Wait for the result section to update after apartment selection
            _wait.Until(driver =>
            {
                var offers = driver.FindElements(By.CssSelector("[data-test='product-grid'], .product-grid, .result-grid"));
                return offers.Any(e => e.Displayed);
            });
        }

        public bool IsProductDisplayed(string productName)
        {
            try
            {
                // Wait either for the product-grid or any element that contains the product name text (case-insensitive)
                var lower = productName.ToLowerInvariant();
                var foundByGrid = _wait.Until(d => d.FindElements(By.CssSelector("[data-test='product-grid']")).Any());
                if (foundByGrid)
                {
                    var productElements = _driver.FindElements(By.CssSelector("[data-test='product-grid']"));
                    if (productElements.Any(p => p.Text.IndexOf(productName, StringComparison.OrdinalIgnoreCase) >= 0))
                    {
                        Console.WriteLine($"Product grid is displayed and {productName} is included");
                        return true;
                    }
                }

                // Fallback: search the whole page for the product text (case-insensitive)
                var xpath = $"//*[contains(translate(normalize-space(.), 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'), '{lower}') ]";
                var nodes = _driver.FindElements(By.XPath(xpath));
                if (nodes.Any(e => e.Displayed))
                {
                    Console.WriteLine($"Found product text on page: {productName}");
                    return true;
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

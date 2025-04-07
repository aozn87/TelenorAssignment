using NUnit.Allure.Attributes;
using NUnit.Allure.Core;
using TelenorTest.Utilities;
using Allure.Commons;

namespace TelenorTest.Tests
{
    [AllureNUnit]
    [AllureSuite("Broadband Tests")]
    public class BroadbandTest : BaseTest
    {
        [Test]
        [AllureTag("Smoke")]
        [AllureSeverity(SeverityLevel.critical)]
        [AllureOwner("aho")]
        public void Should_Display5GOffer_When_ValidAddressAndApartmentSelected()
        {
            _homePage.NavigateToBroadbandPage();
            _broadbandPage.EnterAddress(TestDataReader.Get("validAddress"));
            _broadbandPage.SelectRandomApartment();

            var expectedProduct = TestDataReader.Get("ExpectedProduct");

            var isFound = _broadbandPage.IsProductDisplayed(expectedProduct);
            Assert.That(isFound, Is.True, $"{expectedProduct} product not found!");
            // so product grid is loaded and Bredband via 5G is included!!!
        }
    }
}


using TelenorTest.Utilities;

namespace TelenorTest.Tests
{
    public class BroadbandTest : BaseTest
    {
        [Test]
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


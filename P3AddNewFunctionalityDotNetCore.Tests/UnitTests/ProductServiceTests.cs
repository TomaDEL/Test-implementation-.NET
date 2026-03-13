using Microsoft.Extensions.Localization;
using Moq;
using P3AddNewFunctionalityDotNetCore.Models;
using P3AddNewFunctionalityDotNetCore.Models.Repositories;
using P3AddNewFunctionalityDotNetCore.Models.Services;
using P3AddNewFunctionalityDotNetCore.Models.ViewModels;
using Xunit;

namespace P3AddNewFunctionalityDotNetCore.Tests.UnitTests
{
    public class ProductServiceTests
    {
        /// <summary>
        /// Helper method that creates a ProductService with mocks for testing
        /// </summary>
        private ProductService CreateProductService()
        {
            // We create ‘mock’ objects to simulate dependencies
            var mockCart = new Mock<ICart>();
            var mockProductRepo = new Mock<IProductRepository>();
            var mockOrderRepo = new Mock<IOrderRepository>();
            var mockLocalizer = new Mock<IStringLocalizer<ProductService>>();

            // Configuring the mock localiser to return the key as is
            mockLocalizer.Setup(l => l[It.IsAny<string>()])
                .Returns((string key) => new LocalizedString(key, key));

            return new ProductService(
                mockCart.Object,
                mockProductRepo.Object,
                mockOrderRepo.Object,
                mockLocalizer.Object
            );
        }
        // TEST 1 : Check that an empty name returns an error
        [Fact]
        public void CheckProductModelErrors_MissingName_ReturnsError()
        {
            // ARRANGE - Prepare the test data
            var productService = CreateProductService();
            var product = new ProductViewModel
            {
                Name = "",      // Empty name = INVALID
                Price = "10",   // Price valid
                Stock = "5"     // Stock valid
            };

            // ACT - Run the method under test
            var errors = productService.CheckProductModelErrors(product);

            // ASSERT - Check the result
            Assert.Single(errors);
            Assert.Contains("MissingName", errors[0]);  // The error must be "MissingName"
        }

        // TEST 2 : Check that an empty price returns an error
        [Fact]
        public void CheckProductModelErrors_MissingPrice_ReturnsError()
        {
            // ARRANGE
            var productService = CreateProductService();
            var product = new ProductViewModel
            {
                Name = "Produit Test",
                Price = "",     // Empty price = INVALIDE
                Stock = "5"
            };

            // ACT
            var errors = productService.CheckProductModelErrors(product);

            // ASSERT
            var count = errors.Count;
            Assert.True(errors.Count >=1);
            Assert.Contains("MissingPrice", errors[0]);
        }

        // TEST 3 : Check that a non-numeric price returns an error
        [Fact]
        public void CheckProductModelErrors_PriceNotANumber_ReturnsError()
        {
            // ARRANGE
            var productService = CreateProductService();
            var product = new ProductViewModel
            {
                Name = "Produit Test",
                Price = "abc",  // Non-numeric price = INVALID
                Stock = "5"
            };

            // ACT
            var errors = productService.CheckProductModelErrors(product);

            // ASSERT
            Assert.Single(errors);
            Assert.Contains("PriceNotANumber", errors[0]);
        }

        // TEST 4 : Check that a price of <= 0 returns an error
        [Fact]
        public void CheckProductModelErrors_PriceNotGreaterThanZero_ReturnsError()
        {
            // ARRANGE
            var productService = CreateProductService();
            var product = new ProductViewModel
            {
                Name = "Produit Test",
                Price = "0",    // Price = 0 (INVALID)
                Stock = "5"
            };

            // ACT
            var errors = productService.CheckProductModelErrors(product);

            // ASSERT
            Assert.Single(errors);
            Assert.Contains("PriceNotGreaterThanZero", errors[0]);
        }


        // TEST 5 : Check that an empty quantity returns an error
        [Fact]
        public void CheckProductModelErrors_MissingQuantity_ReturnsError()
        {
            // ARRANGE
            var productService = CreateProductService();
            var product = new ProductViewModel
            {
                Name = "Produit Test",
                Price = "10",
                Stock = ""      // Empty stock = INVALID
            };

            // ACT
            var errors = productService.CheckProductModelErrors(product);

            // ASSERT
            Assert.True(errors.Count >= 1);
            Assert.Contains("MissingQuantity", errors[0]);
        }


        // TEST 6 : Check that a non-integer value returns an error

        [Fact]
        public void CheckProductModelErrors_QuantityNotAnInteger_ReturnsError()
        {
            // ARRANGE
            var productService = CreateProductService();
            var product = new ProductViewModel
            {
                Name = "Produit Test",
                Price = "10",
                Stock = "abc"   // Non-numeric stock = INVALID
            };

            // ACT
            var errors = productService.CheckProductModelErrors(product);

            // ASSERT
            Assert.Single(errors);
            Assert.Contains("QuantityNotAnInteger", errors[0]);
        }


        // TEST 7 : Check that a quantity of <= 0 returns an error
        [Fact]
        public void CheckProductModelErrors_QuantityNotGreaterThanZero_ReturnsError()
        {
            // ARRANGE
            var productService = CreateProductService();
            var product = new ProductViewModel
            {
                Name = "Produit Test",
                Price = "10",
                Stock = "0"     // Stock = 0 (INVALID)
            };

            // ACT
            var errors = productService.CheckProductModelErrors(product);

            // ASSERT
            Assert.Single(errors);
            Assert.Contains("QuantityNotGreaterThanZero", errors[0]);
        }


        // TEST 8 : Check that a valid product does not return any errors
        [Fact]
        public void CheckProductModelErrors_ValidProduct_ReturnsNoErrors()
        {
            // ARRANGE
            var productService = CreateProductService();
            var product = new ProductViewModel
            {
                Name = "Laptop Dell XPS 13",
                Price = "999,99",
                Stock = "10",
                Description = "Ordinateur portable haut de gamme",
                Details = "Intel i7, 16GB RAM, 512GB SSD"
            };

            // ACT
            var errors = productService.CheckProductModelErrors(product);

            // ASSERT
            Assert.Empty(errors);  // No errors expected
        }


        // TEST 9 : Check that a negative price returns an error
        [Fact]
        public void CheckProductModelErrors_NegativePrice_ReturnsError()
        {
            // ARRANGE
            var productService = CreateProductService();
            var product = new ProductViewModel
            {
                Name = "Produit Test",
                Price = "-10",  // Negative price = INVALID
                Stock = "5"
            };

            // ACT
            var errors = productService.CheckProductModelErrors(product);

            // ASSERT
            Assert.Single(errors);
            Assert.Contains("PriceNotGreaterThanZero", errors[0]);
        }


        // TEST 10 : Check that a negative quantity returns an error
        [Fact]
        public void CheckProductModelErrors_NegativeStock_ReturnsError()
        {
            // ARRANGE
            var productService = CreateProductService();
            var product = new ProductViewModel
            {
                Name = "Produit Test",
                Price = "10",
                Stock = "-5"    // Negative stock = INVALID
            };

            // ACT
            var errors = productService.CheckProductModelErrors(product);

            // ASSERT
            Assert.Single(errors);
            Assert.Contains("QuantityNotGreaterThanZero", errors[0]);
        }
    }
}
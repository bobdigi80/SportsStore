using System;
using System.Linq;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Controllers;

namespace SportsStore.UnitTests
{
    [TestClass]
    public class ImageTests
    {
        [TestMethod]
        public void Can_Retrieve_Image_Data()
        {
            //Arrange - create a Product with image data
            var prod = new Product
            {
                ProductID = 2,
                Name = "Test",
                ImageData = new byte[] {},
                ImageMimeType = "image/png"
            };

            //Arrange - create the mock repository
            var mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new []
            {
                new Product {ProductID = 1, Name = "P1"},
                prod,
                new Product
                {
                    ProductID = 3,
                    Name = "P3"
                }
            }.AsQueryable());

            //Arrange - create the controller
            var target = new ProductController(mock.Object);

            //Act - call the GetImage action method
            var result = target.GetImage(2);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(FileResult));
            Assert.AreEqual(prod.ImageMimeType, ((FileResult)result).ContentType);
        }

        [TestMethod]
        public void Can_Retrieve_Image_Data_For_Invalid_ID()
        {
            //Arrange - create the mock repository
            var mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product
                {
                    ProductID = 1, Name = "P1"
                },
                new Product
                {
                    ProductID = 2, Name = "P2"
                }
            }.AsQueryable);

            //Arrange - create the controller
            var target = new ProductController(mock.Object);

            //Act - call the GetImage action method
            var result = target.GetImage(100);

            //Assert
            Assert.IsNull(result);
        }
    }
}

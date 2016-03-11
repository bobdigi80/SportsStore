using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Controllers;

namespace SportsStore.UnitTests
{
    [TestClass]
    public class AdminTests
    {
        [TestMethod]
        public void Index_Contains_all_Products()
        {
            //AArange - vreate the mock repository
            var mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product
                {
                    ProductID = 1,
                    Name = "P1"
                },
                new Product
                {
                    ProductID = 2,
                    Name = "P2"
                },
                new Product
                {
                    ProductID = 3,
                    Name = "P3"
                }
            });
     
        //Arrange - create a controller
        var target = new AdminController(mock.Object);
        var result = ((IEnumerable<Product>)target.Index().ViewData.Model).ToArray();
        Assert.AreEqual(result.Length, 3);
        Assert.AreEqual("P1", result[0].Name);
        Assert.AreEqual("P2", result[1].Name);
        Assert.AreEqual("P3", result[2].Name);
    }

        [TestMethod]
        public void Can_Edit_Product()
        {
            //Arrange - create the mock repository
            var mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product
                {
                    ProductID = 1,
                    Name = "P1"
                },
                new Product
                {
                    ProductID = 2,
                    Name = "P2"
                },
                new Product
                {
                    ProductID = 3,
                    Name = "P3"
                }
            });

            //Arrange - create the controller
            var target = new AdminController(mock.Object);

            var p1 = target.Edit(1).ViewData.Model as Product;
            var p2 = target.Edit(1).ViewData.Model as Product;
            var p3 = target.Edit(1).ViewData.Model as Product;
        }

        [TestMethod]
        public void Cannot_Edit_Nonexistant_Product()
        {
            //Arrange - create the mock repo
            var mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product
                {
                    ProductID = 1,
                    Name = "P1"
                },
                new Product
                {
                    ProductID = 2,
                    Name = "P2"
                },
                new Product
                {
                    ProductID = 3,
                    Name = "P3"
                }
            });

            //Arrange - create the controller
            var target = new AdminController(mock.Object);

            //Act
            var result = (Product) target.Edit(4).ViewData.Model;
            
            //Assert
            Assert.IsNull(result);
        }
    }
}

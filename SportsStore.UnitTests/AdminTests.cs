using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Web.Mvc;
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
            //Arange - vreate the mock repository
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
            var p2 = target.Edit(2).ViewData.Model as Product;
            var p3 = target.Edit(3).ViewData.Model as Product;

            if (p1 != null) Assert.AreEqual(1, p1.ProductID);
            if (p2 != null) Assert.AreEqual(2, p2.ProductID);
            if (p3 != null) Assert.AreEqual(3, p3.ProductID);
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

        [TestMethod]
        public void Can_Save_Valid_Changes()
        {
            //Arrange - create mock repository
            var mock = new Mock<IProductRepository>();
            //Arrange - create the controller
            var target = new AdminController(mock.Object);
            //Arrange - create a product
            var product = new Product {Name = "Test"};

            //Act - try to save product
            var result = target.Edit(product);

            //Assert
            mock.Verify(m => m.SaveProduct(product));

            //Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void Cannot_Save_Invalid_Changes()
        {
            // Arrange - create repository
            var mock = new Mock<IProductRepository>();
            // Arrange - create the controller
            var target = new AdminController(mock.Object);
            //Arrange - create a product
            var product = new Product {Name = "Test"};
            //Arrange - add an error to the model state 
            target.ModelState.AddModelError("error", "error");

            //Act - try to save product
            var result = target.Edit(product);

            //Assert - check that the repository was not called
            mock.Verify(m => m.SaveProduct(It.IsAny<Product>()), Times.Never());
            //Assert - check the method result type
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void Can_Delete_Valid_Products()
        {
            //Arrange - create a Product
            var prod = new Product
            {
                ProductID = 2,
                Name = "Test"
            };

            //Arrange - create the mock repository
            var mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product {ProductID = 1, Name = "P1"},
                prod,
                new Product {ProductID = 3, Name = "P3"}
            });

            //Arrange - create the controller
            var target = new AdminController(mock.Object);

            target.Delete(prod.ProductID);

            //Assert - ensure that the repository delete method was called with the correct Product
            mock.Verify(m => m.DeleteProduct(prod.ProductID));

        }
    }
}

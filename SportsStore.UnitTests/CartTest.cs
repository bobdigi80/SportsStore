using System;
using System.Dynamic;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Collections.Generic;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Controllers;
using SportsStore.WebUI.Models;

namespace SportsStore.UnitTests
{
    /// <summary>
    /// Summary description for CartTest
    /// </summary>
    [TestClass]
    public class CartTest
    {
        [TestMethod]
        public void Can_Add_New_Lines()
        {
            //Arrange - create some test products
            Product p1 = new Product
            {
                ProductID = 1,
                Name = "P1"
            };
            Product p2 = new Product
            {
                ProductID = 2,
                Name = "P2"
            };

            Cart target = new Cart();

            //Act
            target.AddItem(p1, 1);
            target.AddItem(p2, 2);
            CartLine[] results = target.Lines.ToArray();

            //Assert
            Assert.AreEqual(results.Length, 2);
            Assert.AreEqual(results[0].Product, p1);
            Assert.AreEqual(results[1].Product, p2);
        }

        [TestMethod]
        public void Can_Add_Quantity_For_Existing_Lines()
        {
            //Arrange - create for test products
            Product p1 = new Product
            {
                ProductID = 1,
                Name = "P1"
            };
            Product p2 = new Product
            {
                ProductID = 2,
                Name = "P2"
            };

            Cart target = new Cart();

            //Act
            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            target.AddItem(p1, 10);
            CartLine[] results = target.Lines.OrderBy(c => c.Product.ProductID).ToArray();

            //Assert
            Assert.AreEqual(results.Length, 2);
            Assert.AreEqual(results[0].Quantity, 11);
            Assert.AreEqual(results[1].Quantity, 1);
        }

        [TestMethod]
        public void Can_Remove_Line()
        {
            //Arrange - create some test products

            Product p1 = new Product
            {
                ProductID = 1,
                Name = "P1"
            };
            Product p2 = new Product
            {
                ProductID = 2,
                Name = "P2"
            };
            Product p3 = new Product
            {
                ProductID = 3,
                Name = "P3"
            };

            Cart target = new Cart();
            //Arrange - add some products to the cart
            target.AddItem(p1, 1);
            target.AddItem(p2, 3);
            target.AddItem(p3, 5);
            target.AddItem(p2, 1);

            //Act
            target.RemoveLine(p2);

            Assert.AreEqual(target.Lines.Count(c => c.Product == p2), 0);
            Assert.AreEqual(target.Lines.Count(), 2);
        }

        [TestMethod]
        public void Calculate_Cart_Total()
        {
            //Arrange = create some test products

            Product p1 = new Product
            {
                ProductID = 1,
                Name = "P1",
                Price = 100M
            };
            Product p2 = new Product
            {
                ProductID = 2,
                Name = "P2",
                Price = 50M
            };

            Cart target = new Cart();

            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            target.AddItem(p1, 1);
            decimal result = target.ComputeTotalValue();

            Assert.AreEqual(result, 250M);
        }

        [TestMethod]
        public void Can_Clear_Contents()
        {
            //Arrange - create some test products
            Product p1 = new Product
            {
                ProductID = 2,
                Name = "P1",
                Price = 100M
            };
            Product p2 = new Product
            {
                ProductID = 2,
                Name = "P2",
                Price = 50M
            };

            Cart target = new Cart();

            target.AddItem(p1, 1);
            target.AddItem(p2, 1);

            target.Clear();

            Assert.AreEqual(target.Lines.Count(), 0);
        }

        [TestMethod]
        public void Can_Add_To_Cart()
        {
            //Arrange - create mock repository
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product
                {
                    ProductID = 1,
                    Name = "P1",
                    Category = "Apples"
                }
            }.AsQueryable());

            //Arrange - create a Cart
            Cart cart = new Cart();

            //Arrange - create controller
            CartController target = new CartController(mock.Object, null);

            //Act - add product to the cart
            target.AddToCart(cart, 1, null);

            //Assert
            Assert.AreEqual(cart.Lines.Count(), 1);
            Assert.AreEqual(cart.Lines.ToArray()[0].Product.ProductID, 1);
        }

        [TestMethod]
        public void Adding_Product_To_Cart_Goes_To_Cart_Screen()
        {
            //Arrange - create the mock repository
            var mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
            {
                new Product
                {
                    ProductID = 1,
                    Name = "P1",
                    Category = "Apples"
                }
            }.AsQueryable());

            // Arrange - create a Cart
            Cart cart = new Cart();

            //Arrange - create the controller
            CartController target = new CartController(mock.Object, null);

            //Act - add a product to the cart
            RedirectToRouteResult result = target.AddToCart(cart, 2, "myUrl");

            //Assert
            Assert.AreEqual(result.RouteValues["action"], "Index");
            Assert.AreEqual(result.RouteValues["returnUrl"], "myUrl");
        }

        [TestMethod]
        public void Can_View_Cart_Contents()
        {
            //Arrange - create a Cart
            Cart cart = new Cart();

            //Arrange - create the controller
            CartController target = new CartController(null, null);

            //Act - call the Index action method
            CartIndexViewModel result = (CartIndexViewModel) target.Index(cart, "myUrl").ViewData.Model;

            //Assert
            Assert.AreSame(result.Cart, cart);
            Assert.AreEqual(result.ReturnUrl, "myUrl");

        }

        [TestMethod]
        public void Cannot_Checkout_Empty_Cart()
        {
            //Arrange - create a mock order processor
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();
            //Arrange - create an empty cart
            Cart cart = new Cart();
            //Arrnage - creat eshipping details
            ShippingDetails shippingDetails = new ShippingDetails();
            //Acreate and nre omstance of the controller
            var target = new CartController(null, mock.Object);

            //Act
            var result = target.Checkout(cart, shippingDetails);

            //Assert - check that the order hasn;t been passed on to the processor
            mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()),
                Times.Never());

            //Assert - check that the method is returning the default view
            Assert.AreEqual("", result.ViewName);
            //Assert - check that I am passing an invalid model to the view
            Assert.AreEqual(false, result.ViewData.ModelState.IsValid);
        }

        [TestMethod]
        public void Cannot_Checkout_Invalid_ShippingDetails()
        {

            //Arrnage - create a mock order processor
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();

            //Arange - CreateInstanceBinder a cart with an item
            mock = new Mock<IOrderProcessor>();

            //Arrange - creat ea cart with an item
            var cart = new Cart();
            cart.AddItem(new Product(), 1);

            //Arange - create an instance of the controller
            var target = new CartController(null, mock.Object);

            //Arrange - add model to the error model
            target.ModelState.AddModelError("error", "error");

            //Act - try to checkout
            var result = target.Checkout(cart, new ShippingDetails());

            //Assert - check that the order has been passed on to the processor
            mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()),
                Times.Never());

            //Assert - check that the method is returning the default view
            Assert.AreEqual("", result.ViewName);
            //Assert - chec that I am passing an invalid model to the view
            Assert.AreEqual(false, result.ViewData.ModelState.IsValid);
        }

        [TestMethod]
        public void Can_Checkout_And_submit_Order()
        {
            //Arrange - create a mock order processor
            var mock = new Mock<IOrderProcessor>();
            var cart = new Cart();
            cart.AddItem(new Product(), 1);
            var target = new CartController(null, mock.Object);

            //Act - try to checkout
            var result = target.Checkout(cart, new ShippingDetails());

            //Assert - check that the order has been passed on to the processor
            mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()),
            Times.Once);
            Assert.AreEqual("Completed", result.ViewName);
            Assert.AreEqual(true, result.ViewData.ModelState.IsValid);
        }
    }
}


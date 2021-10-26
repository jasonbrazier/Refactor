using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moneybox.App.Domain;
using Moneybox.App.Domain.Services;
using Moq;

namespace MoneyBox.UnitTests
{
    [TestClass]
    public class AccountUnitTests
    {
        public const decimal StartAmount = 2000m;

        /// <summary>
        /// Test account
        /// </summary>
        private Account _account;

        /// <summary>
        /// Notification message
        /// </summary>
        private string _notification;

        [TestInitialize]
        public void Initialize()
        {
            // Mock the notification service and create a test account
            var mockNotification = new Mock<INotificationService>();
            mockNotification.Setup(n => n.NotifyFundsLow(It.IsAny<string>())).
                Callback(() => _notification = "Funds Low");
            mockNotification.Setup(n => n.NotifyApproachingPayInLimit(It.IsAny<string>())).
                Callback(() => _notification = "Approaching Limit");
            _account = new Account(mockNotification.Object)
            {
                User = new User{Email = "bob@bob.com"}
            };
            _account.PayIn(StartAmount);
        }

        [TestMethod]
        [DataRow(-100)]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(500)]
        [DataRow(1500)]
        public void WithdrawOk(double amount)
        {
            // Act
            var withdraw = Convert.ToDecimal(amount);
            _account.Withdraw(withdraw);

            // Assert
            Assert.AreEqual(StartAmount - withdraw, _account.Balance, "Unexpected balance");
            Assert.AreEqual(0 - withdraw, _account.Withdrawn, "Unexpected withdrawn amount");
        }

        [TestMethod]
        public void WithdrawFundsLow()
        {
            // Act
            _account.Withdraw(1750m);

            // Assert
            Assert.AreEqual(250m, _account.Balance, "Unexpected balance");
            Assert.AreEqual(-1750m, _account.Withdrawn, "Unexpected withdrawn amount");
            Assert.AreEqual("Funds Low", _notification, "No notification raised");
        }

        [TestMethod]
        public void WithdrawFundsException()
        {
            try
            {
                // Act
                _account.Withdraw(2001m);

                // Assert
                Assert.Fail("No exception raised");
            }
            catch (Exception ex)
            {
                // Assert
                Assert.AreEqual("Insufficient funds to make withdrawal", ex.Message, "Unexpected exception");
            }
        }

        [TestMethod]
        public void PayInOk()
        {
            // Act
            _account.PayIn(500m);

            // Assert
            Assert.AreEqual(2500m, _account.Balance, "Unexpected balance");
            Assert.AreEqual(2500m, _account.PaidIn, "Unexpected withdrawn amount");
        }

        [TestMethod]
        public void PayInOkReachingLimit()
        {
            // Act
            _account.PayIn(1750m);

            // Assert
            Assert.AreEqual(3750m, _account.Balance, "Unexpected balance");
            Assert.AreEqual(3750m, _account.PaidIn, "Unexpected withdrawn amount");
            Assert.AreEqual("Approaching Limit", _notification, "No notification raised");
        }

        [TestMethod]
        public void PayInException()
        {
            try
            {
                // Act
                _account.PayIn(2001m);

                // Assert
                Assert.Fail("No exception raised");
            }
            catch (Exception ex)
            {
                // Assert
                Assert.AreEqual("Account pay in limit reached", ex.Message, "Unexpected exception");
            }
        }
    }
}

using System;
using Moneybox.App.Domain.Services;

namespace Moneybox.App.Domain
{
    /// <summary>
    /// Account domain
    /// </summary>
    /// <remarks>I have considered also that there could be additional validation on the methods here e.g. negative numbers
    /// but wasn't sure if that was in the scope of the test</remarks>
    public class Account
    {
        private readonly INotificationService notificationService;

        public const decimal PayInLimit = 4000m;

        public Guid Id { get; set; }

        public User User { get; set; }

        public decimal Balance { get; private set; }

        public decimal Withdrawn { get; private set; }

        public decimal PaidIn { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="notificationService">Notification service</param>
        /// <remarks>I have made an assumption here that whatever implements IAccountRepository can inject the notification service</remarks>
        /// <remarks>Rather than passing the notification service in via the methods below to avoid creating any sort of dependency</remarks>
        public Account(INotificationService notificationService)
        {
            this.notificationService = notificationService;
        }

        /// <summary>
        /// Withdraw funds from the account
        /// </summary>
        /// <param name="amount">Amount to withdraw</param>
        public void Withdraw(decimal amount)
        {
            var fromBalance = Balance - amount;
            if (fromBalance < 0m)
            {
                throw new InvalidOperationException("Insufficient funds to make withdrawal");
            }

            if (fromBalance < 500m)
            {
                notificationService.NotifyFundsLow(User.Email);
            }

            Balance -= amount;
            // I think this may be a defect, should the amount withdrawn not be a positive value?
            Withdrawn -= amount;
        }

        /// <summary>
        /// Pay funds into the account
        /// </summary>
        /// <param name="amount">Amount to transfer</param>
        public void PayIn(decimal amount)
        {
            var paidIn = PaidIn + amount;
            if (paidIn > PayInLimit)
            {
                throw new InvalidOperationException("Account pay in limit reached");
            }

            if (PayInLimit - paidIn < 500m)
            {
                notificationService.NotifyApproachingPayInLimit(User.Email);
            }

            Balance += amount;
            PaidIn += amount;
        }
    }
}

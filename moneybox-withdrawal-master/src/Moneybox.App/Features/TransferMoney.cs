using System;
using Moneybox.App.DataAccess;

namespace Moneybox.App.Features
{
    public class TransferMoney
    {
        private readonly IAccountRepository accountRepository;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="accountRepository">Account Repository</param>
        public TransferMoney(IAccountRepository accountRepository)
        {
            this.accountRepository = accountRepository;
        }

        /// <summary>
        /// Execute transfer
        /// </summary>
        /// <param name="fromAccountId">Account to withdraw from</param>
        /// <param name="toAccountId">Account to transfer to</param>
        /// <param name="amount">Amount to transfer</param>
        public void Execute(Guid fromAccountId, Guid toAccountId, decimal amount)
        {
            var from = accountRepository.GetAccountById(fromAccountId);
            var to = accountRepository.GetAccountById(toAccountId);

            // Withdraw funds
            from.Withdraw(amount);

            // Transfer funds
            to.PayIn(amount);

            accountRepository.Update(from);
            accountRepository.Update(to);
        }
    }
}

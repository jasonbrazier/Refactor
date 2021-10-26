using System;
using Moneybox.App.DataAccess;

namespace Moneybox.App.Features
{
    public class WithdrawMoney
    {
        private readonly IAccountRepository accountRepository;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="accountRepository">Account Repository</param>
        public WithdrawMoney(IAccountRepository accountRepository)
        {
            this.accountRepository = accountRepository;
        }

        /// <summary>
        /// Execute withdrawal
        /// </summary>
        /// <param name="fromAccountId">Account to withdraw from</param>
        /// <param name="amount">Amount to withdraw</param>
        public void Execute(Guid fromAccountId, decimal amount)
        {
            var from = this.accountRepository.GetAccountById(fromAccountId);

            // Withdraw funds
            from.Withdraw(amount);

            // Update account
            accountRepository.Update(from);
        }
    }
}

using System.Collections.Generic;
using System.Fabric;
using System.Threading.Tasks;
using BookStoreShared.Interfaces;
using BookStoreShared.Models;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

namespace BankService
{
    internal sealed class BankService : StatefulService, IBankService
    {
        private const string AccountsDictionaryName = "accounts";

        public BankService(StatefulServiceContext context)
            : base(context)
        {
        }

        public async Task<AccountDto> GetAccountAsync(string accountId)
        {
            var accountsDictionary =
                await this.StateManager.GetOrAddAsync<IReliableDictionary<string, AccountDto>>(AccountsDictionaryName);

            using (var tx = this.StateManager.CreateTransaction())
            {
                var result = await accountsDictionary.TryGetValueAsync(tx, accountId);

                if (!result.HasValue)
                {
                    return null;
                }

                return result.Value;
            }
        }

        public async Task<bool> WithdrawAsync(string accountId, decimal amount)
        {
            var accountsDictionary =
                await this.StateManager.GetOrAddAsync<IReliableDictionary<string, AccountDto>>(AccountsDictionaryName);

            using (var tx = this.StateManager.CreateTransaction())
            {
                var result = await accountsDictionary.TryGetValueAsync(tx, accountId);

                if (!result.HasValue)
                {
                    return false;
                }

                var account = result.Value;

                if (account.Balance < amount)
                {
                    return false;
                }

                account.Balance -= amount;

                await accountsDictionary.SetAsync(tx, accountId, account);
                await tx.CommitAsync();

                return true;
            }
        }

        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return this.CreateServiceRemotingReplicaListeners();
        }

        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            var accountsDictionary =
                await this.StateManager.GetOrAddAsync<IReliableDictionary<string, AccountDto>>(AccountsDictionaryName);

            using (var tx = this.StateManager.CreateTransaction())
            {
                await accountsDictionary.TryAddAsync(tx, "1", new AccountDto
                {
                    AccountId = "1",
                    UserName = "Danilo",
                    Balance = 200
                });

                await accountsDictionary.TryAddAsync(tx, "2", new AccountDto
                {
                    AccountId = "2",
                    UserName = "Ana",
                    Balance = 120
                });

                await accountsDictionary.TryAddAsync(tx, "3", new AccountDto
                {
                    AccountId = "3",
                    UserName = "Marko",
                    Balance = 15
                });

                await tx.CommitAsync();
            }

            await Task.CompletedTask;
        }

    }
}

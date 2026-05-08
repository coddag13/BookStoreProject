using System.Threading.Tasks;
using BookStoreShared.Models;
using Microsoft.ServiceFabric.Services.Remoting;

namespace BookStoreShared.Interfaces
{
    public interface IBankService : IService
    {
        Task<AccountDto> GetAccountAsync(string accountId);
        Task<bool> WithdrawAsync(string accountId, decimal amount);
    }
}

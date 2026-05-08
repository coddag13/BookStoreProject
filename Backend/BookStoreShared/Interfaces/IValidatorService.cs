using BookStoreShared.Models;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Remoting;

namespace BookStoreShared.Interfaces
{
    public interface IValidatorService : IService
    {
        Task<ValidationResultDto> ValidatePurchaseAsync(PurchaseRequestDto request);
    }
}

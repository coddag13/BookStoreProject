using BookStoreShared.Interfaces;
using BookStoreShared.Models;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using System.Collections.Generic;
using System.Fabric;
using System.Threading.Tasks;

namespace Validator
{
    internal sealed class Validator : StatelessService, IValidatorService
    {
        public Validator(StatelessServiceContext context)
            : base(context)
        {
        }

        public async Task<ValidationResultDto> ValidatePurchaseAsync(PurchaseRequestDto request)
        {
            if (request == null)
            {
                return new ValidationResultDto
                {
                    IsValid = false,
                    Message = "Zahtjev nije poslan."
                };
            }

            if (string.IsNullOrWhiteSpace(request.BookTitle))
            {
                return new ValidationResultDto
                {
                    IsValid = false,
                    Message = "Naziv knjige je obavezan."
                };
            }

            if (string.IsNullOrWhiteSpace(request.Author))
            {
                return new ValidationResultDto
                {
                    IsValid = false,
                    Message = "Autor je obavezan."
                };
            }

            if (request.Quantity <= 0)
            {
                return new ValidationResultDto
                {
                    IsValid = false,
                    Message = "Količina mora biti veća od nule."
                };
            }

            if (string.IsNullOrWhiteSpace(request.AccountId))
            {
                return new ValidationResultDto
                {
                    IsValid = false,
                    Message = "Korisnički nalog je obavezan."
                };
            }

            var libraryProxy = ServiceProxy.Create<ILibraryService>(
                new Uri("fabric:/BookStoreBackend/LibraryService")
            );

            var bankProxy = ServiceProxy.Create<IBankService>(
                new Uri("fabric:/BookStoreBackend/BankService")
            );

            var book = await libraryProxy.GetBookByTitleAndAuthorAsync(request.BookTitle, request.Author);

            if (book == null)
            {
                return new ValidationResultDto
                {
                    IsValid = false,
                    Message = "Tražena knjiga ne postoji."
                };
            }

            if (book.AvailableQuantity < request.Quantity)
            {
                return new ValidationResultDto
                {
                    IsValid = false,
                    Message = "Nema dovoljno knjiga na stanju."
                };
            }

            var account = await bankProxy.GetAccountAsync(request.AccountId);

            if (account == null)
            {
                return new ValidationResultDto
                {
                    IsValid = false,
                    Message = "Korisnički nalog ne postoji."
                };
            }

            var totalPrice = book.Price * request.Quantity;

            if (account.Balance < totalPrice)
            {
                return new ValidationResultDto
                {
                    IsValid = false,
                    Message = "Korisnik nema dovoljno sredstava na računu."
                };
            }

            var moneyWithdrawn = await bankProxy.WithdrawAsync(request.AccountId, totalPrice);

            if (!moneyWithdrawn)
            {
                return new ValidationResultDto
                {
                    IsValid = false,
                    Message = "Skidanje novca nije uspjelo."
                };
            }

            var quantityReduced = await libraryProxy.DecreaseBookQuantityAsync(book.BookId, request.Quantity);

            if (!quantityReduced)
            {
                return new ValidationResultDto
                {
                    IsValid = false,
                    Message = "Smanjenje količine knjige nije uspjelo."
                };
            }

            return new ValidationResultDto
            {
                IsValid = true,
                Message = $"Kupovina uspješna. Ukupan iznos: {totalPrice} KM."
            };
        }


        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return this.CreateServiceRemotingInstanceListeners();
        }
    }
}

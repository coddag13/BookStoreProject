using System.Collections.Generic;
using System.Fabric;
using System.Threading.Tasks;
using BookStoreShared.Interfaces;
using BookStoreShared.Models;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

namespace Validator
{
    internal sealed class Validator : StatelessService, IValidatorService
    {
        public Validator(StatelessServiceContext context)
            : base(context)
        {
        }

        public Task<ValidationResultDto> ValidatePurchaseAsync(PurchaseRequestDto request)
        {
            if (request == null)
            {
                return Task.FromResult(new ValidationResultDto
                {
                    IsValid = false,
                    Message = "Zahtjev nije poslan."
                });
            }

            if (string.IsNullOrWhiteSpace(request.BookTitle))
            {
                return Task.FromResult(new ValidationResultDto
                {
                    IsValid = false,
                    Message = "Naziv knjige je obavezan."
                });
            }

            if (string.IsNullOrWhiteSpace(request.Author))
            {
                return Task.FromResult(new ValidationResultDto
                {
                    IsValid = false,
                    Message = "Autor je obavezan."
                });
            }

            if (request.Quantity <= 0)
            {
                return Task.FromResult(new ValidationResultDto
                {
                    IsValid = false,
                    Message = "Količina mora biti veća od nule."
                });
            }

            if (string.IsNullOrWhiteSpace(request.AccountId))
            {
                return Task.FromResult(new ValidationResultDto
                {
                    IsValid = false,
                    Message = "Korisnički nalog je obavezan."
                });
            }

            return Task.FromResult(new ValidationResultDto
            {
                IsValid = true,
                Message = "Validacija uspješna."
            });
        }

        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return this.CreateServiceRemotingInstanceListeners();
        }
    }
}

using System;
using System.Threading.Tasks;
using BookStoreShared.Interfaces;
using BookStoreShared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace BackendSF.Controllers
{
    [ApiController]
    [Route("purchase")]
    public class PurchaseController : ControllerBase
    {
        
        [HttpPost]
        public async Task<IActionResult> CreatePurchase([FromBody] PurchaseRequestDto request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest("Zahtjev nije poslan.");
                }

                var validatorProxy = ServiceProxy.Create<IValidatorService>(
                    new Uri("fabric:/BookStoreBackend/Validator")
                );

                var validationResult = await validatorProxy.ValidatePurchaseAsync(request);

                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult);
                }

                return Ok(new
                {
                    Message = "Kupovina je validirana i zahtjev je uspješno primljen.",
                    Validation = validationResult,
                    Request = request
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "Greška u backendu.",
                    Error = ex.Message,
                    InnerError = ex.InnerException?.Message,
                    StackTrace = ex.StackTrace
                });
            }
        }

    }
}

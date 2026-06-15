using Game.Backend.Modules.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Game.Backend.Modules.Purchases;

[ApiController]
[Authorize]
[Route("api/v1/purchases")]
public sealed class PurchasesController : ControllerBase
{
    private readonly PurchaseService _purchaseService;

    public PurchasesController(PurchaseService purchaseService)
    {
        _purchaseService = purchaseService;
    }

    [HttpPost("validate")]
    public async Task<ActionResult<ValidatePurchaseResponse>> Validate(
        ValidatePurchaseRequest? request,
        CancellationToken cancellationToken)
    {
        return await _purchaseService.ValidatePurchaseAsync(
            CurrentAccount.GetAccountId(User),
            request,
            cancellationToken);
    }
}

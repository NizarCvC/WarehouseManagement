using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace WarehouseAPI.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/stocks")]
[Authorize]
[Tags("Stocks")]
[Produces("application/json")]
[EnableRateLimiting(policyName: "GeneralPolicy")]
public class StockController : ControllerBase
{
    
}

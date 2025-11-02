using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Zerphin.RentACar.Application.Common;

namespace Zerphin.RentACar.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseController : ControllerBase
{
    protected readonly IMediator _mediator;

    protected BaseController(IMediator mediator)
    {
        _mediator = mediator;
    }
    [NonAction]
    public IActionResult CreateActionResult<T>(ServiceResult<T> result)
    {
        return result.Status switch
        {
            HttpStatusCode.NoContent => NoContent(),
            HttpStatusCode.Created => Created(result.UrlAsCreated, result),
            _ => new ObjectResult(result) { StatusCode = result.Status.GetHashCode() }
        };
    }

    [NonAction]
    public IActionResult CreateActionResult(ServiceResult result)
    {
        return result.Status switch
        {
            HttpStatusCode.NoContent => new ObjectResult(null) { StatusCode = result.Status.GetHashCode() },
            _ => new ObjectResult(result) { StatusCode = result.Status.GetHashCode() }
        };
    }
}

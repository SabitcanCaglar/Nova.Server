using Base.Application.Common.Interfaces;
using CleanArchitecture.WebUI.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.ApiGateway.Controllers;

public class TestController : ApiControllerBase
{
    private IApplicationDbContext context;
    
    public TestController(IApplicationDbContext context)
    {
        this.context = context;
    }
    
    [HttpGet]
    public async Task<ActionResult> Delete()
    {
        var test1 = context;
        return NoContent();
    }
}

using Microsoft.AspNetCore.Mvc;

namespace SpaceLaunchPlugin.Controllers;

[ApiController]
[Route("[controller]")]
public class SpaceLaunchController : ControllerBase
{
    private readonly ILogger<SpaceLaunchController> _logger;

    public SpaceLaunchController(ILogger<SpaceLaunchController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetSpaceLaunchInfo")]
    public SpaceLaunch Get()
    {
        var launch = new SpaceLaunch()
        {
            DateUtc = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
            FlightNumber = 111,
            WebcastLink = "test link"
        };

        return launch;
    }
}


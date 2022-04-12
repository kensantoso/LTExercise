using LTExercise.LogAnalysis;
using Microsoft.AspNetCore.Mvc;

namespace LTExercise.Controllers;

[ApiController]
[Route("[controller]")]
public class LogAnalysisController : ControllerBase
{
    private readonly ILogger<LogAnalysisController> _logger;
    private readonly LogService _logService;

    public LogAnalysisController(LogService logService, ILogger<LogAnalysisController> logger)
    {
        _logger = logger;
        _logService = logService;
    }

    [HttpGet(Name = "GetLogAnalysis")]
    public IEnumerable<SharedSessionResponse> Get(int from, int to)
    {
       return  _logService.GetMaxSharedUserSessions(from, to);
    }
}

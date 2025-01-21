using Backend.Functionality;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Backend.Function
{
    public class GetEvents(ILogger<GetEvents> logger, IEventService _event)
    {
        private readonly ILogger<GetEvents> _logger = logger;

        [Function("GetEvents")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var allEvents = await _event.GetAllEvents();

            return new OkObjectResult(allEvents);
        }
    }
}

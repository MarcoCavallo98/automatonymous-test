using AutomatonymousTest.Common.Commands;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Mime;

namespace AutomatonymousTest.ServiceA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestSagaController : ControllerBase
    {
        private readonly ISendEndpointProvider _bus;
        private readonly ILogger<TestSagaController> _logger;

        public TestSagaController(ISendEndpointProvider bus, ILogger<TestSagaController> logger)
        {
            _bus = bus;
            _logger = logger;
        }

        [HttpPost]
        [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(APIResponse<string?>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> StartSaga()
        {
            try
            {
                var id = NewId.NextGuid();
                var endpoint = await _bus.GetSendEndpoint(new Uri("queue:test-saga"));
                using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                await endpoint.Send<StartTestSaga>(new
                {
                    Id = NewId.NextGuid(),
                    Name = $"Test {Guid.NewGuid()}"
                }, timeout.Token);

                _logger.LogInformation($"Starting saga: {id}");

                return Ok(new APIResponse<string> { ErrorCode = -1, Description = "OK", Body = id.ToString() });
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, ex.Message);

                return new ContentResult
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    ContentType = MediaTypeNames.Application.Json,
                    Content = JsonConvert.SerializeObject(new APIResponse<string?>
                    {
                        ErrorCode = -1,
                        Description = "internal_error",
                        Body = null
                    })
                };
            }
        }
    }
}

using System.Threading;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AspNetBox.Controllers
{
    [Produces("text/plain")]
    [Route("[controller]")]
    public class WaitForController : ControllerBase
    {
        private readonly ILogger<WaitForController> _logger;

        public WaitForController(ILogger<WaitForController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Responds after the specified delay.
        /// </summary>
        /// <param name="delay">Delay in milleseconds</param>
        /// <returns>A sample reponse with the delay</returns>
        [HttpGet]
        public string Get(int delay)
        {
            _logger.LogInformation($"Will sleep for {delay}");

            Thread.Sleep(delay);

            return $"Response after {delay} ms";
        }
    }
}

using System.Threading;

using Microsoft.AspNetCore.Mvc;

namespace AspNetBox.Controllers
{
    [Produces("text/plain")]
    [Route("[controller]")]
    public class WaitForController : ControllerBase
    {
        public WaitForController() { }

        /// <summary>
        /// Responds after the specified delay.
        /// </summary>
        /// <param name="delay">Delay in milleseconds</param>
        /// <returns>A sample reponse with the delay</returns>
        [HttpGet]
        public string Get(int delay)
        {
            Thread.Sleep(delay);

            return $"Response after {delay} ms";
        }
    }
}

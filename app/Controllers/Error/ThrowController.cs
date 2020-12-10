using System.IO;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AspNetBox.Controllers
{
    [Produces("text/plain")]
    [Route("[controller]")]
    public class ThrowController : ControllerBase
    {
        /// <summary>
        /// Thows an unhandled exception
        /// </summary>
        /// <param name="message">Custom error message</param>
        /// <returns>Never</returns>
        [HttpGet()]
        public string Get(string message = "this is a sample error message")
        {
            throw new System.Exception(message);
        }

    }
}
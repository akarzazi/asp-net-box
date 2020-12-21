using System.IO;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AspNetBox.Controllers
{
    [Produces("text/plain")]
    [Route("[controller]")]
    public class EchoController : ControllerBase
    {
        /// <summary>
        /// Retuns the provided text
        /// </summary>
        /// <param name="text">The text that will be returned</param>
        /// <returns>Returns the provided text</returns>
        [HttpGet()]
        public string Get(string text)
        {
            if (text == null)
            {
                return "use /echo?text=mytext";
            }

            return text;
        }

        /// <summary>
        /// Retuns the provided text from the request body
        /// </summary>
        /// <returns>Returns the text obtained from the body</returns>
        [HttpPost]
        [Consumes("text/plain")]
        public string Post([FromBody] string text)
        {
            return text;
        }
    }
}

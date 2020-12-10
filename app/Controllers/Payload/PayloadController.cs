using System;
using System.Text;

using Microsoft.AspNetCore.Mvc;

namespace AspNetBox.Controllers
{
    [Produces("text/plain")]
    [Route("[controller]")]
    public class PayloadController : ControllerBase
    {
        const string chars90 = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt\r\n";

        public PayloadController() { }

        /// <summary>
        /// Generates a sample response text with the specified length.
        /// </summary>
        /// <param name="length">Text length</param>
        /// <returns>A text with the specified length</returns>
        [HttpGet]
        public string Get(int length)
        {
            var sb = new StringBuilder();
            var i = 0;
            while (sb.Length < length)
            {
                i++;
                var num = i.ToString().PadRight(9, '-');
                sb.Append(num);
                sb.Append(chars90);
            }

            var final = sb.ToString();
            final = final.Substring(0, Math.Min(final.Length, length));


            return final;
        }

        /// <summary>
        /// Measures the received body payload length
        /// </summary>
        /// <returns>Body payload length</returns>
        [HttpPost]
        [Consumes("text/plain")]
        public string Post([FromBody] string text)
        {
            return $"Received text length:\n{text.Length}";
        }
    }
}

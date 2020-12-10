using System.Text.Json;
using System.Text.Json.Serialization;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AspNetBox.Controllers
{
    [Route("[controller]")]
    public class RequestInfoController : ControllerBase
    {
        private JsonSerializerOptions _jsonOpts;

        public RequestInfoController()
        {
            _jsonOpts = new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
                WriteIndented = true,
            };
        }

        [HttpGet]
        public string Get()
        {
            return JsonSerializer.Serialize(new
            {
                RemoteIpAddress = HttpContext.Connection?.RemoteIpAddress?.ToString(),
                RemotePort = HttpContext.Connection?.RemotePort,
                LocalIpAddress = HttpContext.Connection?.LocalIpAddress?.ToString(),
                LocalPort = HttpContext.Connection?.LocalPort,
                Request.Protocol,
                Request.Scheme,
                Request.Host,
                Request.Path,
                Request.Query,
                Request.Headers,
                Request.Cookies
            }
            , _jsonOpts);
        }
    }
}

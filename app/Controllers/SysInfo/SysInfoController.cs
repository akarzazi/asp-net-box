using System;
using System.Collections;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;

using AspNetBox.Utils;

using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using YamlDotNet.Serialization;

namespace AspNetBox.Controllers
{
    [Route("/")]
    public class SysInfoController : ControllerBase
    {

        private ISerializer _serializer;
        private JsonSerializerOptions _jsonOpts;

        public SysInfoController(ILogger<SysInfoController> logger)
        {
            _logger = logger;
            _serializer = new SerializerBuilder()
                            .DisableAliases()
                            .Build();

            _jsonOpts = new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
                WriteIndented = true,
            };

        }

        /// <summary>
        /// Dumps request and system information
        /// </summary>
        /// <returns>Request and system information</returns>
        [HttpGet]
        public string Get()
        {
            _logger.LogInformation("Getting system info");

            var dumpInfo = $"More at {Url.AbsoluteContent(Request, "~/swagger")}\n" +
                 DumpRequestInfo()
                 +
                 DumpEnvVars()
                 +
                 GetRuntimeInfo()
                 +
                 GetEnvironnementInfo()
                 +
                 GetTimeInfo()
                 +
                 GetTlsInfo()
                 +
                 typeof(SysInfoController).Assembly.FullName
                 + "\n";

            _logger.LogInformation("[OK] System info");
            return dumpInfo;
        }

        private string DumpRequestInfo()
        {
            return $"Request info\n" + _serializer.Serialize(
                  new
                  {
                      RemoteIpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                      RemotePort = HttpContext.Connection.RemotePort,
                      LocalIpAddress = HttpContext.Connection.LocalIpAddress?.ToString(),
                      LocalPort = HttpContext.Connection.LocalPort,
                      Request.Protocol,
                      Request.Scheme,
                      Host = Request.Host.ToString(),
                      Path = Request.Path.ToString(),
                      Request.Query,
                      Request.Headers,
                      Request.Cookies
                  }).Indent(4);
        }

        private string DumpEnvVars()
        {
            return $"Environnement vars\n" + _serializer.Serialize(
                Environment.GetEnvironmentVariables()
                .OfType<DictionaryEntry>()
                .OrderBy(p => p.Key)
                )
                .Indent(4);
        }

        private string GetRuntimeInfo()
        {
            return $"RuntimeInfo \n" + _serializer.Serialize(
                new
                {
                    RuntimeInformation.FrameworkDescription,
                    RuntimeInformation.RuntimeIdentifier,
                    RuntimeInformation.OSArchitecture,
                    RuntimeInformation.OSDescription,
                    RuntimeInformation.ProcessArchitecture,
                }
                ).Indent(4);
        }

        private string GetEnvironnementInfo()
        {
            return $"Environment info \n" + _serializer.Serialize(
                new
                {
                    Environment.MachineName,
                    Environment.UserName,
                    Environment.UserDomainName,
                    OSVersion = Environment.OSVersion.VersionString,
                    Environment.Is64BitProcess,
                    Environment.Is64BitOperatingSystem,
                    Environment.CommandLine,
                    Environment.CurrentDirectory,
                    Environment.ProcessorCount,
                    Environment.ProcessId,
                    Environment.CurrentManagedThreadId,
                }
                ).Indent(4);
        }

        private string GetTlsInfo()
        {
            var TlsHandshakeFeature = HttpContext.Features.Get<ITlsHandshakeFeature>();
            var TlsConnectionFeature = HttpContext.Features.Get<ITlsConnectionFeature>();

            return $"TlsInfo \n" + JsonSerializer.Serialize(
                new
                {
                    TlsHandshakeFeature,
                    TlsConnectionFeature,
                },
                _jsonOpts
                ).Indent(4);
        }

        private string GetTimeInfo()
        {
            TimeZoneInfo localZone = TimeZoneInfo.Local;

            return $"Time info \n" + _serializer.Serialize(
                new
                {
                    DateTime = new
                    {
                        DateTime.Now,
                        Utc = DateTime.UtcNow
                    },
                    LocalTimeZone = new
                    {
                        localZone.Id,
                        localZone.DisplayName,
                        localZone.StandardName,
                        localZone.DaylightName,
                        localZone.BaseUtcOffset,
                    }
                }
                ).Indent(4);
        }
    }
}

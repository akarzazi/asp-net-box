using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AspNetBox
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<RequestLoggingMiddleware>();
        }

        public async Task Invoke(HttpContext context)
        {
            var requestDate = DateTime.UtcNow;

            try
            {

                //   _logger.LogInformation($"Request start - { context.Request?.Method} {context.Request?.Path}{context.Request?.QueryString} ");
                await _next(context);
            }
            finally
            {
                var responseDate = DateTime.UtcNow;

                //   .LogInformation($"Request end => { context.Response?.StatusCode} - { context.Request?.Method} {context.Request?.Path}{context.Request?.QueryString} ");

                var ev = new MyLogEvent("Request")
                    .WithProperty("RequestDateUtc", requestDate)
                    .WithProperty("ResponseDateUtc", responseDate)
                    .WithProperty("Method", context.Request?.Method)
                    .WithProperty("Protocol", context.Request?.Protocol)
                    .WithProperty("Scheme", context.Request?.Scheme)
                    .WithProperty("Host", context.Request?.Host.Host)
                    .WithProperty("Port", context.Request?.Host.Port)
                    .WithProperty("LocalPort", context.Connection?.LocalPort)
                    .WithProperty("Path", context.Request?.Path)
                    .WithProperty("Query", context.Request?.QueryString)
                    .WithProperty("TraceIdentifier", context.TraceIdentifier)
                    .WithProperty("RemoteIpAddress", context.Connection?.RemoteIpAddress?.ToString())
                    .WithProperty("UserAgent", context.Request?.Headers["User-Agent"])
                    .WithProperty("Referer", context.Request?.Headers["Referer"])
                    .WithProperty("StatusCode", context.Response?.StatusCode)
                    .WithProperty("ContentLength", context.Response?.ContentLength)
                    .WithProperty("Elapsed", (int)responseDate.Subtract(requestDate).TotalMilliseconds);

                _logger.Log(LogLevel.Information,
                           default,
                           ev,
                           exception: null,
                           MyLogEvent.Formatter);

            }
        }
    }

    internal class MyLogEvent : IEnumerable<KeyValuePair<string, object>>
    {
        private List<KeyValuePair<string, object?>> _properties = new();

        public string Message { get; }

        public MyLogEvent(string message)
        {
            Message = message;
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return _properties.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

        public MyLogEvent WithProperty(string name, object? value)
        {
            _properties.Add(new KeyValuePair<string, object?>(name, value));
            return this;
        }

        public static Func<MyLogEvent, Exception?, string> Formatter { get; } = (l, e) => l.Message;
    }
}

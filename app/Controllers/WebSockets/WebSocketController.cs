using Azure;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using System;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Unicode;
using System.Threading;
using System.Threading.Tasks;

using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AspNetBox.Controllers.WebSockets
{
    public class WebSocketController : ControllerBase
    {
        private readonly ILogger<WebSocketController> _logger;

        public WebSocketController(ILogger<WebSocketController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Echo websocket messages
        /// </summary>
        /// <remarks>
        /// To try out, go to <a href='/websocket-echo.html'>/websocket-echo.html</a>
        /// 
        /// Sample javascript code
        /// 
        ///     var ws = new WebSocket("ws://127.0.0.1:5000/ws");
        ///     ws.onclose = (ev) => console.log("onclose",ev);
        ///     ws.onerror = (ev) => console.log("onerror",ev);
        ///     ws.onmessage = (ev) =>  console.log("onmessage",ev);
        ///     ws.onopen = (ev) =>  { console.log("onopen",ev)
        ///         ws.send("Hello World");
        ///     };
        ///     
        /// </remarks>
        [Route("/ws")]
        [HttpGet]
        public async Task Get()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                _logger.LogInformation("Incoming WebSocket connection");

                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                await Echo(webSocket);
            }
            else
            {
                _logger.LogInformation("Bad request for WebSocket connection");

                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }

        private async Task Echo(WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            var receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            while (!receiveResult.CloseStatus.HasValue)
            {
                LogMessage(buffer, receiveResult);

                var responseBytes = Encoding.UTF8.GetBytes($"Echo from {Environment.MachineName}");

                await webSocket.SendAsync(
                    new ArraySegment<byte>(responseBytes, 0, responseBytes.Length),
                    WebSocketMessageType.Text,
                    endOfMessage: true,
                    CancellationToken.None);

                await webSocket.SendAsync(
                      new ArraySegment<byte>(buffer, 0, receiveResult.Count),
                      receiveResult.MessageType,
                      receiveResult.EndOfMessage,
                      CancellationToken.None);

                receiveResult = await webSocket.ReceiveAsync(
                    new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            await webSocket.CloseAsync(
                receiveResult.CloseStatus.Value,
                receiveResult.CloseStatusDescription,
                CancellationToken.None);
        }

        private void LogMessage(byte[] buffer, WebSocketReceiveResult receiveResult)
        {
            switch (receiveResult.MessageType)
            {
                case WebSocketMessageType.Text:
                    _logger.LogInformation($"Received Websocket {WebSocketMessageType.Text}, length: {receiveResult.Count}, content: {Safe_UTF8_GetString(buffer, receiveResult.Count)}");
                    break;
                case WebSocketMessageType.Binary:
                    _logger.LogInformation($"Received Websocket {WebSocketMessageType.Binary}, length: {receiveResult.Count}");
                    break;
            }
        }

        private string Safe_UTF8_GetString(byte[] msg, int length)
        {
            try
            {
                return Encoding.UTF8.GetString(msg, 0, length);
            }
            catch (Exception)
            {
                return $"Not an UTF8 message, length {length}";
            }
        }
    }
}

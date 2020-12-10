using System;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AspNetBox.Utils
{
    public static class UrlUtils
    {
        public static string AbsoluteContent(this IUrlHelper urlHelper, HttpRequest request, string? contentPath)
        {
            UriBuilder uriBuilder = new UriBuilder(request.Scheme, request.Host.Host);

            if(request.Host.Port.HasValue)
            {
                uriBuilder.Port = request.Host.Port.Value;
            }

            uriBuilder.Path = urlHelper.Content(contentPath);

            return uriBuilder.Uri.AbsoluteUri;
        }
    }
}

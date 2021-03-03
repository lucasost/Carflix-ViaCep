
using Microsoft.AspNetCore.Http;
using System;

namespace Carflix.Extensions
{
    public static class AjaxRequestExtensions
    {
        
        public static bool IsAjaxRequest(this HttpRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.Headers != null)
                return request.Headers["x-requested-with"] == "XMLHttpRequest";
            return false;
        }
    }
}

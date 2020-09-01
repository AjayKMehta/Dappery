using System;
using System.Collections.Generic;
using System.Net;

namespace Dappery.Core.Exceptions
{
    public class DapperyApiException : Exception
    {
        public DapperyApiException(string message, HttpStatusCode statusCode)
            : base(message)
        {
            this.StatusCode = statusCode;
            this.ApiErrors = new List<DapperyApiError>();
        }

        public HttpStatusCode StatusCode { get; }

        public ICollection<DapperyApiError> ApiErrors { get; }
    }
}

using System;
using System.Collections.Generic;
using System.Net;

namespace Dappery.Core.Exceptions;

public class DapperyApiException : Exception
{
    public DapperyApiException(string message, HttpStatusCode statusCode)
        : this(message) => StatusCode = statusCode;

    public DapperyApiException() => ApiErrors = [];

    public DapperyApiException(string message) : base(message) => ApiErrors = [];

    public DapperyApiException(string message, Exception innerException) : base(message, innerException) =>
    ApiErrors = [];

    public HttpStatusCode StatusCode { get; }

    public ICollection<DapperyApiError> ApiErrors { get; }
}

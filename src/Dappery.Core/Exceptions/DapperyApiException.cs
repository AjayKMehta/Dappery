using System;
using System.Collections.Generic;
using System.Net;

namespace Dappery.Core.Exceptions;

public class DapperyApiException : Exception
{
    public DapperyApiException(string message, HttpStatusCode statusCode)
        : this(message) => this.StatusCode = statusCode;

    public DapperyApiException() => this.ApiErrors = new List<DapperyApiError>();

    public DapperyApiException(string message) : base(message) => this.ApiErrors = new List<DapperyApiError>();

    public DapperyApiException(string message, Exception innerException) : base(message, innerException) =>
    this.ApiErrors = new List<DapperyApiError>();

    public HttpStatusCode StatusCode { get; }

    public ICollection<DapperyApiError> ApiErrors { get; }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using FluentValidation;
using FluentValidation.Results;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Dappery.Core.Infrastructure;

public class RequestValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull, IRequest<TResponse>
{
    private static readonly Action<ILogger, TRequest, Exception?> s_logValidationFailure = LoggerMessage.Define<TRequest>(
        LogLevel.Information,
        new EventId(1, "RequestValidationException"),
        "Validation failures for request [{Request}]");

    private readonly IEnumerable<IValidator<TRequest>> _validators;
    private readonly ILogger<TRequest> _logger;

    public RequestValidationBehavior(IEnumerable<IValidator<TRequest>> validators, ILogger<TRequest> logger)
    {
        _validators = validators ?? [];
        _logger = logger;
    }

    public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(next);

        var context = new ValidationContext<TRequest>(request);

        IEnumerable<ValidationFailure> failures = _validators
            .SelectMany(v => v.Validate(context).Errors)
            .Where(f => f is not null);

        if (failures.Any())
        {
            s_logValidationFailure(_logger, request, null);
            throw new ValidationException(failures);
        }
        return next(cancellationToken);
    }
}

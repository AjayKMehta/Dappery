using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Dappery.Core.Infrastructure
{
    public class RequestValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull, IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> validators;
        private readonly ILogger<TRequest> logger;

        public RequestValidationBehavior(IEnumerable<IValidator<TRequest>> validators, ILogger<TRequest> logger)
        {
            this.validators = validators ?? Enumerable.Empty<IValidator<TRequest>>();
            this.logger = logger;
        }

        public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            if (next is null)
            {
                throw new ArgumentNullException(nameof(next));
            }

            var context = new ValidationContext<TRequest>(request);

            var failures = this.validators
                .Select(v => v.Validate(context))
                .SelectMany(result => result.Errors)
                .Where(f => f is not null);

            if (failures.Any())
            {
                this.logger.LogInformation("Validation failures for request [{Request}]", request);
                throw new ValidationException(failures);
            }

            return next();
        }
    }
}

using System.Reflection;

using Dappery.Core.Infrastructure;

using MediatR;

using Microsoft.Extensions.DependencyInjection;

namespace Dappery.Core.Extensions;

public static class StartupExtensions
{
    /// <summary>
    /// Extension to contain all of our business layer dependencies for our external server providers (ASP.NET Core in our case).
    /// </summary>
    /// <param name="services">Service collection for dependency injection</param>
    public static void AddDapperyCore(this IServiceCollection services)
    {
        // Add our MediatR and FluentValidation dependencies
        _ = services
            .AddMediatR(cfg =>
            {
                _ = cfg
                    .RegisterServicesFromAssembly(typeof(StartupExtensions).Assembly)
                    .AddOpenBehavior(typeof(RequestValidationBehavior<,>));
            }
            );
    }
}

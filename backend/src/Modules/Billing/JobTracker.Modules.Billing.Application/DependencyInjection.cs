using System.Reflection;
using FluentValidation;
using JobTracker.SharedKernel.Behaviors;
using Microsoft.Extensions.DependencyInjection;

namespace JobTracker.Modules.Billing.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddBillingApplication(this IServiceCollection services)
    {
        Assembly assembly = typeof(DependencyInjection).Assembly;

        services.AddMediatR(configuration => configuration.RegisterServicesFromAssembly(assembly));
        services.AddValidatorsFromAssembly(assembly);
        services.AddTransient(typeof(MediatR.IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }
}

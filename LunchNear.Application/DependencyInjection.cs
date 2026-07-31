namespace LunchNear.Application;

using System.Reflection;
using FluentValidation;
using Mapster;
using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddValidatorsFromAssembly(assembly);

        TypeAdapterConfig.GlobalSettings.Scan(assembly);

        return services;
    }
}

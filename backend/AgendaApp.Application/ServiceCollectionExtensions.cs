using AgendaApp.Application.Features.Contact.Commands.Validators;
using AgendaApp.Application.Interfaces;
using AgendaApp.Application.Mappings;
using AgendaApp.Application.Services;
using AgendaApp.Domain.Services;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace AgendaApp.Application;

public static class ServiceCollectionExtensions
{
    public static void AddApplicationService(this IServiceCollection services)
    {
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        services.AddAutoMapper(typeof(ContactProfile).Assembly);

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        services.AddValidatorsFromAssemblyContaining<CreateContactValidator>();

        services.AddScoped<IUserAppService, UserAppService>();

        services.AddSingleton<IRabbitMqService, RabbitMqService>();
    }
}
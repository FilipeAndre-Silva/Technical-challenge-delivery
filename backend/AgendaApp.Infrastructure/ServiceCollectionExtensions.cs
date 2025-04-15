using AgendaApp.Domain.Interfaces.ReadRepository;
using AgendaApp.Domain.Interfaces.Repositories;
using AgendaApp.Infrastructure.Context;
using AgendaApp.Infrastructure.ReadRepository;
using AgendaApp.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Data;

namespace AgendaApp.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static void AddInfraDependency(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("AgendaAppConnection");

        services.AddDbContext<AgendaAppDbContext>(options =>
            options.UseSqlite(connectionString));

        services.AddIdentity<IdentityUserCustom, IdentityRole<Guid>>()
        .AddEntityFrameworkStores<AgendaAppDbContext>()
        .AddDefaultTokenProviders();

        services.AddScoped<IDbConnection>(sp =>
            new SqliteConnection(connectionString));

        DapperConfiguration.ConfigureTypeHandlers();

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IContactRepository, ContactRepository>();
        services.AddScoped<IContactReadRepository, ContactReadRepository>();
    }
}
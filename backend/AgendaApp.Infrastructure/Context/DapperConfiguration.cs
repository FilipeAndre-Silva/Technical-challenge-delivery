using AgendaApp.Infrastructure.Helpers;
using Dapper;

namespace AgendaApp.Infrastructure.Context;

public static class DapperConfiguration
{
    public static void ConfigureTypeHandlers()
    {
        SqlMapper.AddTypeHandler(new GuidTypeHandler());
    }
}
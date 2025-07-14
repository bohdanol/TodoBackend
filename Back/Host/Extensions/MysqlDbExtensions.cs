using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Model.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Todo.Core.Data;

namespace Host.Extensions;

public static class MysqlDbExtensions
{
    public static IServiceCollection AddMySqlDbCollections(this IServiceCollection services, MysqlDbSettings settings)
    {
        services.AddDbContext<MysqlDbContext>(options =>
        options.UseMySql(settings.ConnectionString, ServerVersion.AutoDetect(settings.ConnectionString)));
        return services;
    }
}

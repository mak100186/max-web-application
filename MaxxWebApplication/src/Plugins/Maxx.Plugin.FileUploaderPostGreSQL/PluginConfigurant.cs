using Maxx.Plugin.FileUploaderPostGreSQL.Models;
using Maxx.Plugin.Shared.Abstractions;

using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Maxx.Plugin.FileUploaderPostGreSQL;

public class PluginConfigurant : IAppConfigurant
{
    public void Configure(IHost host)
    {
        var app = (WebApplication)host;

        if (Environment.GetCommandLineArgs().Contains("--migrate"))
        {
            using var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<DbContextClass>();

            var logger = app.Services.GetRequiredService<ILogger<PluginConfigurant>>();

            var pending = dbContext.Database.GetPendingMigrations();

            logger.LogInformation(
                pending.Any()
                    ? $"Applying Migrations :{Environment.NewLine}{string.Join(Environment.NewLine, pending)}"
                    : "No outstanding migrations");

            dbContext.Database.Migrate();

            logger.LogInformation($"{pending.Count()} Migrations applied");
        }
    }
}

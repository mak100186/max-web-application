﻿using Maxx.Plugin.FileUploaderPostGreSQL.Models;
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

            var pendingMigrations = dbContext.Database.GetPendingMigrations().ToList();
            var logger = app.Services.GetRequiredService<ILogger<PluginConfigurant>>();
            if (pendingMigrations.Count > 0)
            {
                logger.LogInformation($"Applying pending migrations:\n{string.Join('\n', pendingMigrations)}");
            }

            dbContext.Database.Migrate();

            logger.LogInformation($"{pendingMigrations.Count} Migrations applied");
        }
    }
}

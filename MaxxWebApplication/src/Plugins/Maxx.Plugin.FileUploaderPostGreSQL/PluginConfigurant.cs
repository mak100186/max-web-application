using Maxx.Plugin.FileUploaderPostGreSQL.Models;
using Maxx.Plugin.Shared.Abstractions;

using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
            
            dbContext.Database.Migrate();
        }
    }
}

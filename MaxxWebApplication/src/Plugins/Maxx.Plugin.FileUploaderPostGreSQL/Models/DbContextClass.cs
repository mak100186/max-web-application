using Maxx.Plugin.FileUploaderPostGreSQL.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Maxx.Plugin.FileUploaderPostGreSQL.Models;

public class DbContextClass : DbContext
{
    private readonly IConfiguration _configuration;

    private static DbContextOptions GetDbContextOptionsForMigrations()
    {
        var configs = typeof(DbContextClass).LoadConfigurationsFromAssemblyWithType();

        var connectionString = configs.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new("ConnectionString cannot be null");
        }

        return
            new DbContextOptionsBuilder<DbContextClass>()
                .UseNpgsql(connectionString)
                .Options;
    }

    public DbContextClass() : base(GetDbContextOptionsForMigrations())
    {
        _configuration = typeof(DbContextClass).LoadConfigurationsFromAssemblyWithType();
    }

    public DbContextClass(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        var connectionString = _configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new("ConnectionString cannot be null");
        }

        options.UseNpgsql(connectionString,
            x => x.MigrationsAssembly(typeof(DbContextClass).Assembly.FullName));
    }

    public DbSet<FileDetails> FileDetails { get; set; }
}

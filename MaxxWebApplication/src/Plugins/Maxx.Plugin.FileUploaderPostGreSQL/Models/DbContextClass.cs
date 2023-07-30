using Maxx.Plugin.Common.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Maxx.Plugin.FileUploaderPostGreSQL.Models;

public class DbContextClass : DbContext
{
    private readonly IConfiguration _configuration;

    public DbContextClass() => _configuration = typeof(DbContextClass).LoadConfigurationsFromAssemblyWithType();

    public DbContextClass(IConfiguration configuration) => _configuration = configuration;

    public DbSet<FileDetails> FileDetails { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        var connectionString = _configuration.GetConnectionString("FileUploaderConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new Exception("ConnectionString cannot be null");
        }

        options.UseNpgsql(connectionString,
            x => x.MigrationsAssembly(typeof(DbContextClass).Assembly.FullName));
    }
}

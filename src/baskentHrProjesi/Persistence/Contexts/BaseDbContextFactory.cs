using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Persistence.Contexts
{
    public class BaseDbContextFactory : IDesignTimeDbContextFactory<BaseDbContext>
    {
        public BaseDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile(Path.Combine("..", "WebAPI", "appsettings.json"), optional: true)
                .Build();

            var connectionString = configuration.GetConnectionString("BaseDb");

            var optionsBuilder = new DbContextOptionsBuilder<BaseDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new BaseDbContext(optionsBuilder.Options, configuration);
        }
    }
}

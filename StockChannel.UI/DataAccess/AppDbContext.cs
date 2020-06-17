using System.IO;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using StockChannel.Domain.Entities;

namespace StockChannel.UI.DataAccess
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            var builder = new DbContextOptionsBuilder<AppDbContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            builder.UseSqlServer(connectionString);
            return new AppDbContext(builder.Options);
        }
    }
    public class AppDbContext : IdentityDbContext
    {
        public static string connectionName = "DefaultConnection";
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            //builder.Entity<ChatMessage>().OwnsOne(x => x.);
            // builder.Entity<ChatMessage>()
            //     .HasOne<JobsityUser>(a => a.Sender)
            //     .WithMany(d => d.Messages)
            //     .HasForeignKey(d => d.UserID);
        }

        public DbSet<ChatMessage> Messages { get; set; }
    }
}
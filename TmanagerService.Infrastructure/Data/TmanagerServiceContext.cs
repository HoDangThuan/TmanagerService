using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using TmanagerService.Core.Entities;

namespace TmanagerService.Infrastructure.Data
{
    public class TmanagerServiceContext : IdentityDbContext<ApplicationUser>
    {
        public TmanagerServiceContext(DbContextOptions<TmanagerServiceContext> options) : base(options)
        { }

        public TmanagerServiceContext()
        { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            /*
            builder.Entity<User_Request>().HasKey(table => new {
                table.UserId
            });
            builder.Entity<User_FeedBack>().HasKey(table => new {
                table.UserId
            });
            */
        }

        public DbSet<Request> Requests { get; set; }
    }
}
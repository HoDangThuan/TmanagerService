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

            //builder.Entity<AreaWorking>().HasKey(table => new
            //{
            //    table.AdminId,
            //    table.Area
            //});
        }

        public DbSet<Company> Companys { get; set; }
        //public DbSet<AreaWorking> AreaWorkings { get; set; }
        public DbSet<Request> Requests { get; set; }
    }
}
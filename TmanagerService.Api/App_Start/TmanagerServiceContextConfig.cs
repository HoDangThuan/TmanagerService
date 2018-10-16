using Microsoft.EntityFrameworkCore;

namespace TmanagerService.Api.App_Start
{
    public class TmanagerServiceContextConfig
    {
        public static void Config(DbContextOptionsBuilder options)
        {
            var connectionString = Startup.Configuration["identity_db_connection_string"];
            options.UseSqlServer(connectionString, b => b.MigrationsAssembly("TmanagerService.Api"));
        }
    }
}

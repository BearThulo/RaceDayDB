
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace RaceDay.Api.Data
{
    public class RaceDayDbContextFactory
        : IDesignTimeDbContextFactory<RaceDayDbContext>
    {
        public RaceDayDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder =
                new DbContextOptionsBuilder<RaceDayDbContext>();

            optionsBuilder.UseSqlServer(
                "Server=UNCK0FFI3\\SQLEXPRESS01;Database=RACEDAY;Trusted_Connection=True;TrustServerCertificate=True;");

            return new RaceDayDbContext(optionsBuilder.Options);
        }
    }
}

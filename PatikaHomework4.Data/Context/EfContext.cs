using Microsoft.EntityFrameworkCore;
using PatikaHomework4.Data.Model;

namespace PatikaHomework4.Data.Context
{
    public class EfContext : DbContext
    {
        public EfContext(DbContextOptions<EfContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //base.OnModelCreating(modelBuilder);
            modelBuilder.UseSerialColumns();
        }

        public DbSet<Person> Person { get; set; }
        

    }
}

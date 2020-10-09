using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using RentACarMVC.Models;

namespace RentACarMVC.Context
{
    public class RentACarDbContext:DbContext
    {
        public RentACarDbContext()
        {
            
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<RentACarDbContext>(null);
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            modelBuilder.Entity<Tipo>().ToTable("Tipos");
            modelBuilder.Entity<Modelo>().ToTable("Modelos");
            modelBuilder.Entity<Usuario>().ToTable("Usuarios");
            modelBuilder.Entity<Auto>().ToTable("Autos");

        }

        public DbSet<Tipo> Tipos { get; set; }
        public DbSet<Marca> Marcas { get; set; }
        public DbSet<Modelo> Modelos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Auto> Autos { get; set; }

    }
}
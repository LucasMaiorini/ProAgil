using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProAgil.Domain;
using ProAgil.Domain.Identity;

namespace ProAgil.Repository
{
    //All the insertions is needed to work with Identity
    public class ProAgilContext : IdentityDbContext<User, Role, int, IdentityUserClaim<int>,
UserRole, IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public ProAgilContext(DbContextOptions<ProAgilContext> options) : base(options) { }

        public DbSet<Evento> Eventos { get; set; }
        public DbSet<Palestrante> Palestrantes { get; set; }
        public DbSet<PalestranteEvento> PalestranteEventos { get; set; }
        public DbSet<Lote> Lotes { get; set; }
        public DbSet<RedeSocial> RedesSociais { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Needes to be configured
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserRole>(userRole =>
            {
                //Sets the two Keys in UserRole
                userRole.HasKey(ur => new { ur.UserId, ur.RoleId });
                //In userRole, one role has many userRoles and the fk thar represents it is RoleId
                userRole.HasOne(ur => ur.Role).WithMany(r => r.UserRoles).HasForeignKey(ur => ur.RoleId).IsRequired();
                //In userRole, one user may have many userRoles and the fk thar represents it is UserId
                userRole.HasOne(ur => ur.User).WithMany(r => r.UserRoles).HasForeignKey(ur => ur.UserId).IsRequired();
            });
            //Sets the two Keys in PalestranteEvento
            modelBuilder.Entity<PalestranteEvento>().HasKey(PE => new { PE.EventoId, PE.PalestranteId });
        }
    }
}
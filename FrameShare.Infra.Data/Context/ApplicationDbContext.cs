using FrameShare.Application.Interfaces;
using FrameShare.Application.Utils;
using FrameShare.Domain.Entity;
using FrameShare.Infa.Data.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FrameShare.Infra.Data.Context
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>,IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Missao> Missao {  get; set; }
        public DbSet<Foto> Foto { get; set; }
        public DbSet<Evento> Evento { get; set; }
        public DbSet<Usuario> Usuario { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //aplica as configuracoes da pasta EntitiesConfiguration
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
            var nomesIniciais = new List<string> { "Natália Judice de Novais", "Admin FrameShare" };

            for (int i = 0; i < nomesIniciais.Count; i++)
            {
                var nome = nomesIniciais[i];
                var novoUsuario = new Usuario
                {
                    Id = i + 1,
                    NomeCompleto = nome,
                    EventoId = 1,
                    Role = nome.Contains("Admin") ? "Admin" : "Convidado"
                };

                // Como o método GerarSlug é estático no seu Helper:
                modelBuilder.Entity<Usuario>().HasData(new
                {
                    Id = novoUsuario.Id,
                    NomeCompleto = novoUsuario.NomeCompleto,
                    SlugLogin = StringHelper.GerarSlug(nome), // Gera o "natalianovais"
                    Role = novoUsuario.Role,
                    EventoId = novoUsuario.EventoId
                });
            }

        }
    }
}

using HelpTech.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace HelpTech.Data.Context
{
    public class HelpTechContext : DbContext
    {
        public DbSet<Ocorrencia> OcorrenciaSet { get; set; }
        public DbSet<Usuario> UsuariosSet { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.
            //    ApplyConfiguration(new CategoriaConfiguration());
            //modelBuilder.
            //    ApplyConfiguration(new FilmeConfiguration());

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //const string conexao = "server=localhost;port=3306;database=HelpTech;uid=root"; //;password=SENHA_DO_BANCO
            const string conexao = "server=mysql.em3soft.com.br;database=em3soft35;uid=em3soft35;password=a05C80";
            optionsBuilder.UseMySql(conexao, ServerVersion.AutoDetect(conexao));
            base.OnConfiguring(optionsBuilder);
        }
    }
}

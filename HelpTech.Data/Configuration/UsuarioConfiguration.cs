using HelpTech.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpTech.Data.Configuration
{
    internal class UsuarioConfiguration :
        IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Nome)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(p => p.EmailLogin)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(p => p.Senha)
                .HasMaxLength(2000)
                .IsRequired();

            builder.ToTable("TB_Usuario");
        }
    }

}

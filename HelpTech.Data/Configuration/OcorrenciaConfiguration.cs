using HelpTech.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HelpTech.Data.Configuration
{
    public class OcorrenciaConfiguration :
        IEntityTypeConfiguration<Ocorrencia>
    {
        public void Configure(EntityTypeBuilder<Ocorrencia> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Descricao)
                .HasMaxLength(1000)
                .IsRequired();

            builder.Property(p => p.TipoOcorrencia)
                .HasMaxLength(50)
                .IsRequired();

            builder.ToTable("TB_Ocorrencia");
        }
    }
}

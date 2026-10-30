using MarcasAutos.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarcasAutos.Api.Infrastructure.Persistence.Configurations;

public class MarcaAutoConfiguration : IEntityTypeConfiguration<MarcaAuto>
{
    public const int NombreMaxLength = 100;

    public void Configure(EntityTypeBuilder<MarcaAuto> builder)
    {
        builder.ToTable("MarcasAutos");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Nombre)
            .IsRequired()
            .HasMaxLength(NombreMaxLength);

        // Evita marcas duplicadas; además protege el seed ante ejecuciones concurrentes.
        builder.HasIndex(m => m.Nombre).IsUnique();
    }
}

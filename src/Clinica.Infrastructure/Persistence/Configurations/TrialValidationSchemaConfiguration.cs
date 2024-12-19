using Clinica.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clinica.Infrastructure.Persistence.Configurations;

public class TrialValidationSchemaConfiguration : IEntityTypeConfiguration<TrialsValidationSchema>
{
    public void Configure(EntityTypeBuilder<TrialsValidationSchema> builder)
    {
        builder.ToTable("ValidationSchemas");
        
        builder.HasKey(ct => ct.Id);
        builder.Property(p => p.Id).ValueGeneratedOnAdd();
        builder.Property(p => p.ValidationSchema).HasColumnType("NVARCHAR(MAX)");
        builder.Property(p => p.CreatedAt).HasDefaultValue(DateTime.UtcNow);
    }
}
using Clinica.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clinica.Infrastructure.Persistence.Configurations;

public class TrialMetadataConfiguration : IEntityTypeConfiguration<TrialMetadata>
{
    public void Configure(EntityTypeBuilder<TrialMetadata> builder)
    {
        builder.ToTable("Trials");
        
        builder.HasKey(ct => ct.TrialId);
        
        builder.Property(ct => ct.TrialId)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(ct => ct.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(ct => ct.StartDate)
            .IsRequired();

        builder.Property(ct => ct.EndDate)
            .IsRequired(false);

        builder.Property(ct => ct.Participants)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(ct => ct.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);
        
        builder.HasIndex(ct => ct.TrialId)
            .IsUnique();
        
        builder.Property(t => t.DurationInDays)
            .IsRequired();
    }
}
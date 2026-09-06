using JobTracker.Modules.Jobs.Domain;
using JobTracker.Modules.Jobs.Infrastructure.Persistence.Placeholders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobTracker.Modules.Jobs.Infrastructure.Persistence.Configurations;

internal sealed class JobConfiguration : IEntityTypeConfiguration<Job>
{
    public void Configure(EntityTypeBuilder<Job> builder)
    {
        builder.ToTable("jobs");
        builder.HasKey(job => job.Id);

        builder.Property(job => job.Title).IsRequired().HasMaxLength(200);
        builder.Property(job => job.Description).IsRequired().HasMaxLength(2000);
        builder.Property(job => job.Notes).HasMaxLength(2000);
        builder.Property(job => job.SignatureUrl).HasMaxLength(1000);
        builder.Property(job => job.CancellationReason).HasMaxLength(1000);

        // Enum stored as string, not an int, so the column stays readable/stable
        // across enum member reordering.
        builder.Property(job => job.Status).HasConversion<string>().HasMaxLength(50);

        // Address is a Value Object: mapped as an EF owned type — no identity of its
        // own, columns land directly on the jobs table.
        builder.OwnsOne(job => job.Address, addressBuilder =>
        {
            addressBuilder.Property(address => address.Street).IsRequired().HasColumnName("street");
            addressBuilder.Property(address => address.City).IsRequired().HasColumnName("city");
            addressBuilder.Property(address => address.State).IsRequired().HasColumnName("state");
            addressBuilder.Property(address => address.ZipCode).IsRequired().HasColumnName("zip_code");
            addressBuilder.Property(address => address.Latitude).HasColumnName("latitude");
            addressBuilder.Property(address => address.Longitude).HasColumnName("longitude");
        });
        builder.Navigation(job => job.Address).IsRequired();

        // JobPhoto is only reachable through the Job aggregate root — an owned
        // collection matches that intent exactly. Photos is exposed as
        // IReadOnlyList<JobPhoto>, so EF must be told to materialize it through the
        // private _photos backing field instead of the (nonexistent) property setter.
        builder.OwnsMany(job => job.Photos, photoBuilder =>
        {
            photoBuilder.ToTable("job_photos");
            photoBuilder.WithOwner().HasForeignKey("JobId");
            photoBuilder.HasKey(photo => photo.Id);
            photoBuilder.Property(photo => photo.Url).IsRequired();
            photoBuilder.Property(photo => photo.Caption).HasMaxLength(500);
        });
        builder.Navigation(job => job.Photos).UsePropertyAccessMode(PropertyAccessMode.Field);

        // Domain events are runtime-only bookkeeping for the outbox interceptor —
        // never persisted as a column/navigation.
        builder.Ignore(job => job.DomainEvents);

        // FK constraints without CLR navigation properties: Job's domain model
        // shouldn't know about Assignee/Customer as types, but the schema (AC.md
        // Part 4.1) still needs real referential integrity.
        builder.HasOne<Assignee>().WithMany()
            .HasForeignKey(job => job.AssigneeId)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);

        builder.HasOne<Customer>().WithMany()
            .HasForeignKey(job => job.CustomerId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        // Multi-tenant queries always filter by organization first.
        builder.HasIndex(job => job.OrganizationId);
        builder.HasIndex(job => new { job.OrganizationId, job.Status });
        builder.HasIndex(job => new { job.OrganizationId, job.ScheduledDate });
    }
}

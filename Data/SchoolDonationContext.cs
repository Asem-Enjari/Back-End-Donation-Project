using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SchoolDonation.API.Models;

namespace SchoolDonation.API.Data;

public partial class SchoolDonationContext : DbContext
{
    public SchoolDonationContext()
    {
    }

    public SchoolDonationContext(DbContextOptions<SchoolDonationContext> options)
        : base(options)
    {
    }

	public DbSet<Payment> Payments { get; set; } = null!;

	public virtual DbSet<Donation> Donations { get; set; }

    public virtual DbSet<Image> Images { get; set; }

    public virtual DbSet<Manager> Managers { get; set; }

    public virtual DbSet<Need> Needs { get; set; }

    public virtual DbSet<Report> Reports { get; set; }

    public virtual DbSet<School> Schools { get; set; }

    public virtual DbSet<SchoolLocation> SchoolLocations { get; set; }

    public virtual DbSet<StudentSponsorship> StudentSponsorships { get; set; }

    public virtual DbSet<UsedItem> UsedItems { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<VolunteerActivity> VolunteerActivities { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.;Database=SchoolDonationDB;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
		modelBuilder.Entity<Payment>(entity =>
		{
			entity.HasKey(e => e.PaymentId);

			entity.Property(e => e.Amount)
				  .HasColumnType("decimal(18,2)");

			entity.HasOne(e => e.Need)
				  .WithMany()
				  .HasForeignKey(e => e.NeedId)
				  .OnDelete(DeleteBehavior.Cascade);

			entity.HasOne(e => e.School)
				  .WithMany()
				  .HasForeignKey(e => e.SchoolId)
				  .OnDelete(DeleteBehavior.NoAction); 
		});


		modelBuilder.Entity<Donation>(entity =>
        {
            entity.HasKey(e => e.DonationId).HasName("PK__Donation__C5082EFB7B6263DC");

            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.DonationDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.TransactionReference).HasMaxLength(100);

            entity.HasOne(d => d.Donor).WithMany(p => p.Donations)
                .HasForeignKey(d => d.DonorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Donations_Users");

            entity.HasOne(d => d.Need).WithMany(p => p.Donations)
                .HasForeignKey(d => d.NeedId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Donations_Needs");
        });

        modelBuilder.Entity<Image>(entity =>
        {
            entity.HasKey(e => e.ImageId).HasName("PK__Images__7516F70C3662C111");

            entity.Property(e => e.ImageType).HasMaxLength(50);
            entity.Property(e => e.UploadDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Need).WithMany(p => p.Images)
                .HasForeignKey(d => d.NeedId)
                .HasConstraintName("FK_Images_Needs");
        });

        modelBuilder.Entity<Manager>(entity =>
        {
            entity.HasKey(e => e.ManagerId).HasName("PK__Managers__3BA2AAE12155D9AF");

            entity.HasIndex(e => e.EmployeeNumber, "UQ__Managers__8D66359897AA7AA5").IsUnique();

            entity.Property(e => e.ManagerId).ValueGeneratedNever();
            entity.Property(e => e.EmployeeNumber).HasMaxLength(50);

            entity.HasOne(d => d.ManagerNavigation).WithOne(p => p.Manager)
                .HasForeignKey<Manager>(d => d.ManagerId)
                .HasConstraintName("FK_Managers_Users");
        });

        modelBuilder.Entity<Need>(entity =>
        {
            entity.HasKey(e => e.NeedId).HasName("PK__Needs__CEDBFB586A0D4E8A");

            entity.Property(e => e.CollectedAmount)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Cost).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PublishDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Pending");
            entity.Property(e => e.Title).HasMaxLength(100);

            entity.HasOne(d => d.School).WithMany(p => p.Needs)
                .HasForeignKey(d => d.SchoolId)
                .HasConstraintName("FK_Needs_Schools");
        });

        modelBuilder.Entity<Report>(entity =>
        {
            entity.HasKey(e => e.ReportId).HasName("PK__Reports__D5BD4805DA0E641E");

            entity.Property(e => e.PublishDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Need).WithMany(p => p.Reports)
                .HasForeignKey(d => d.NeedId)
                .HasConstraintName("FK_Reports_Needs");
        });

        modelBuilder.Entity<School>(entity =>
        {
            entity.HasKey(e => e.SchoolId).HasName("PK__Schools__3DA4675B56E1B323");

            entity.HasIndex(e => e.ManagerId, "UQ__Schools__3BA2AAE0B1F59227").IsUnique();

            entity.Property(e => e.City).HasMaxLength(50);
            entity.Property(e => e.Governorate).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Operational");
            entity.Property(e => e.Street).HasMaxLength(100);

            entity.HasOne(d => d.Manager).WithOne(p => p.School)
                .HasForeignKey<School>(d => d.ManagerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Schools_Managers");
        });

        modelBuilder.Entity<SchoolLocation>(entity =>
        {
            entity.HasKey(e => e.LocationId).HasName("PK__SchoolLo__E7FEA497BE0FA57F");

            entity.HasIndex(e => e.SchoolId, "UQ__SchoolLo__3DA4675A138CDE2A").IsUnique();

            entity.Property(e => e.Latitude).HasColumnType("decimal(9, 6)");
            entity.Property(e => e.Longitude).HasColumnType("decimal(9, 6)");

            entity.HasOne(d => d.School).WithOne(p => p.SchoolLocation)
                .HasForeignKey<SchoolLocation>(d => d.SchoolId)
                .HasConstraintName("FK_Locations_Schools");
        });

        modelBuilder.Entity<StudentSponsorship>(entity =>
        {
            entity.HasKey(e => e.SponsorId).HasName("PK__StudentS__3B609ED5EAD8D9FD");

            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.IsAnonymous).HasDefaultValue(false);
            entity.Property(e => e.ItemType).HasMaxLength(50);
            entity.Property(e => e.SponsorshipDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.SponsorshipReason).HasMaxLength(255);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Pending");
            entity.Property(e => e.StudentFullName).HasMaxLength(100);

            entity.HasOne(d => d.Donor).WithMany(p => p.StudentSponsorships)
                .HasForeignKey(d => d.DonorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Sponsorships_Users");

            entity.HasOne(d => d.School).WithMany(p => p.StudentSponsorships)
                .HasForeignKey(d => d.SchoolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Sponsorships_Schools");
        });

        modelBuilder.Entity<UsedItem>(entity =>
        {
            entity.HasKey(e => e.ItemId).HasName("PK__UsedItem__727E838B6E344075");

            entity.Property(e => e.DonationDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Pending Approval");

            entity.HasOne(d => d.Donor).WithMany(p => p.UsedItems)
                .HasForeignKey(d => d.DonorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UsedItems_Users");

            entity.HasOne(d => d.School).WithMany(p => p.UsedItems)
                .HasForeignKey(d => d.SchoolId)
                .HasConstraintName("FK_UsedItems_Schools");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4CE566F4B5");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D105348ECE42E3").IsUnique();

            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.JoinDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.UserRole).HasMaxLength(20);
        });

        modelBuilder.Entity<VolunteerActivity>(entity =>
        {
            entity.HasKey(e => e.ActivityId).HasName("PK__Voluntee__45F4A79147C537AD");

            entity.Property(e => e.ActivityType).HasMaxLength(50);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Pending");

            entity.HasOne(d => d.School).WithMany(p => p.VolunteerActivities)
                .HasForeignKey(d => d.SchoolId)
                .HasConstraintName("FK_Activities_Schools");

            entity.HasOne(d => d.Volunteer).WithMany(p => p.VolunteerActivities)
                .HasForeignKey(d => d.VolunteerId)
                .HasConstraintName("FK_Activities_Users");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

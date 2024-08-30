using Microsoft.EntityFrameworkCore;
using RbacDashboard.DAL.Models;
using System.Diagnostics.CodeAnalysis;

namespace RbacDashboard.DAL.Data;

public partial class RbacSqlDbContext(DbContextOptions<RbacSqlDbContext> options) : DbContext(options)
{
    public virtual DbSet<Access> Accesses { get; set; }

    public virtual DbSet<Application> Applications { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<OptionsetMaster> OptionsetMasters { get; set; }

    public virtual DbSet<Permissionset> Permissionsets { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RoleAccess> RoleAccesses { get; set; }

    public virtual DbSet<TypeMaster> TypeMasters { get; set; }

    [ExcludeFromCodeCoverage(Justification = "Memory DB migration cannot be tested due to limitations.")]
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("RBAC");

        modelBuilder.Entity<Access>(entity =>
        {
            entity.ToTable("Access", "RBAC");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedOn)
                  .HasConversion(
                      v => v.ToUniversalTime(),
                      v => v.ToLocalTime()
                  )
                  .HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);

            entity.HasOne(d => d.Application).WithMany(p => p.Accesses)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Access_ApplicationId_Application_Id");

            entity.HasOne(d => d.OptionsetMaster).WithMany(p => p.Accesses)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Access_OptionsetMasterId_OptionsetMaster_Id");

            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent).HasConstraintName("FK_Access_ParentId_Access_Id");
        });

        modelBuilder.Entity<Application>(entity =>
        {
            entity.ToTable("Application", "RBAC");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedOn)
                   .HasConversion(
                       v => v.ToUniversalTime(),
                       v => v.ToLocalTime()
                   )
                   .HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);

            entity.HasOne(d => d.Customer).WithMany(p => p.Applications)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Application_CustomerId_Customer_Id");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.ToTable("Customer", "RBAC");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedOn)
                  .HasConversion(
                      v => v.ToUniversalTime(),
                      v => v.ToLocalTime()
                  )
                  .HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
        });

        modelBuilder.Entity<OptionsetMaster>(entity =>
        {
            entity.ToTable("OptionsetMaster", "RBAC");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedOn)
                  .HasConversion(
                      v => v.ToUniversalTime(),
                      v => v.ToLocalTime()
                  )
                  .HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
        });

        modelBuilder.Entity<Permissionset>(entity =>
        {
            entity.ToTable("Permissionset", "RBAC");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedOn)
                  .HasConversion(
                      v => v.ToUniversalTime(),
                      v => v.ToLocalTime()
                  )
                  .HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);

            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent).HasConstraintName("FK_Permissionset_ParentId_Permissionset_Id");

            entity.HasOne(d => d.PermissionType).WithMany(p => p.Permissionsets)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Permissionset_PermissionTypeId_OptionsetMaster_Id");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Role", "RBAC");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedOn)
                  .HasConversion(
                      v => v.ToUniversalTime(),
                      v => v.ToLocalTime()
                  )
                  .HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);

            entity.HasOne(d => d.Application).WithMany(p => p.Roles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Roles_ApplicationId_Application_Id");

            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent).HasConstraintName("FK_Roles_ParentId_Roles_Id");

            entity.HasOne(d => d.TypeMaster).WithMany(p => p.Roles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Roles_TypeMasterId_TypeMaster_Id");
        });

        modelBuilder.Entity<RoleAccess>(entity =>
        {
            entity.ToTable("RoleAccess", "RBAC");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedOn)
                  .HasConversion(
                      v => v.ToUniversalTime(),
                      v => v.ToLocalTime()
                  )
                  .HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);

            entity.HasOne(d => d.Access).WithMany(p => p.RoleAccesses)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RoleAccess_AccessId_Access_Id");

            entity.HasOne(d => d.Application).WithMany(p => p.RoleAccesses)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_RoleAccess_ApplicationId_Application_Id");

            entity.HasOne(d => d.Role)
                  .WithMany(p => p.RoleAccesses)
                  .OnDelete(DeleteBehavior.ClientSetNull)
                  .HasConstraintName("FK_RoleAccess_RoleId_Roles_Id");
        });

        modelBuilder.Entity<TypeMaster>(entity =>
        {
            entity.ToTable("TypeMaster", "RBAC");
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedOn)
                  .HasConversion(
                      v => v.ToUniversalTime(),
                      v => v.ToLocalTime()
                  )
                  .HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);

            entity.HasOne(d => d.OptionsetMaster).WithMany(p => p.TypeMasters).HasConstraintName("FK_TypeMaster_OptionsetMasterId_OptionsetMaster_Id");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    [ExcludeFromCodeCoverage(Justification = "Memory DB migration cannot be tested due to limitations.")]
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

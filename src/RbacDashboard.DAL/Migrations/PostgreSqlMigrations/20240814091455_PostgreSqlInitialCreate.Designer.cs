﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using RbacDashboard.DAL.Data;

#nullable disable

namespace RbacDashboard.DAL.Migrations.PostgreSqlMigrations
{
    [DbContext(typeof(PgSqlServerDbContext))]
    [Migration("20240814091455_PostgreSqlInitialCreate")]
    partial class PostgreSqlInitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("RbacDashboard.DAL.Models.Access", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<string>("AccessName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<Guid>("ApplicationId")
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset>("CreatedOn")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("NOW()");

                    b.Property<bool>("IsActive")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(true);

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<string>("MetaData")
                        .HasColumnType("text");

                    b.Property<Guid>("OptionsetMasterId")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("ParentId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("ApplicationId");

                    b.HasIndex("OptionsetMasterId");

                    b.HasIndex("ParentId");

                    b.ToTable("Access", "RBAC");
                });

            modelBuilder.Entity("RbacDashboard.DAL.Models.Application", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<string>("ApplicationName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<DateTimeOffset>("CreatedOn")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("NOW()");

                    b.Property<Guid>("CustomerId")
                        .HasColumnType("uuid");

                    b.Property<bool>("IsActive")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(true);

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.HasIndex("CustomerId");

                    b.ToTable("Application", "RBAC");
                });

            modelBuilder.Entity("RbacDashboard.DAL.Models.Customer", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<DateTimeOffset>("CreatedOn")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("NOW()");

                    b.Property<string>("CustomerName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<bool>("IsActive")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(true);

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.ToTable("Customer", "RBAC");
                });

            modelBuilder.Entity("RbacDashboard.DAL.Models.OptionsetMaster", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset>("CreatedOn")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("NOW()");

                    b.Property<bool>("IsActive")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(true);

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<string>("JsonObject")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<Guid>("Value")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.ToTable("OptionsetMaster", "RBAC");
                });

            modelBuilder.Entity("RbacDashboard.DAL.Models.Permissionset", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset>("CreatedOn")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("NOW()");

                    b.Property<bool>("IsActive")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(true);

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<Guid?>("ParentId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("PermissionTypeId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("ParentId");

                    b.HasIndex("PermissionTypeId");

                    b.ToTable("Permissionset", "RBAC");
                });

            modelBuilder.Entity("RbacDashboard.DAL.Models.Role", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<Guid>("ApplicationId")
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset>("CreatedOn")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("NOW()");

                    b.Property<bool>("IsActive")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(true);

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<Guid?>("ParentId")
                        .HasColumnType("uuid");

                    b.Property<string>("RoleName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<Guid>("TypeMasterId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("ApplicationId");

                    b.HasIndex("ParentId");

                    b.HasIndex("TypeMasterId");

                    b.ToTable("Role", "RBAC");
                });

            modelBuilder.Entity("RbacDashboard.DAL.Models.RoleAccess", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<Guid>("AccessId")
                        .HasColumnType("uuid");

                    b.Property<string>("AccessMetaData")
                        .HasColumnType("text");

                    b.Property<Guid>("ApplicationId")
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset>("CreatedOn")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("NOW()");

                    b.Property<bool>("IsActive")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(true);

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsOverwrite")
                        .HasColumnType("boolean");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("AccessId");

                    b.HasIndex("ApplicationId");

                    b.HasIndex("RoleId");

                    b.ToTable("RoleAccess", "RBAC");
                });

            modelBuilder.Entity("RbacDashboard.DAL.Models.TypeMaster", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset>("CreatedOn")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("NOW()");

                    b.Property<bool>("IsActive")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("boolean")
                        .HasDefaultValue(true);

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<Guid?>("OptionsetMasterId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("OptionsetMasterId");

                    b.ToTable("TypeMaster", "RBAC");
                });

            modelBuilder.Entity("RbacDashboard.DAL.Models.Access", b =>
                {
                    b.HasOne("RbacDashboard.DAL.Models.Application", "Application")
                        .WithMany("Accesses")
                        .HasForeignKey("ApplicationId")
                        .IsRequired()
                        .HasConstraintName("FK_Access_ApplicationId_Application_Id");

                    b.HasOne("RbacDashboard.DAL.Models.OptionsetMaster", "OptionsetMaster")
                        .WithMany("Accesses")
                        .HasForeignKey("OptionsetMasterId")
                        .IsRequired()
                        .HasConstraintName("FK_Access_OptionsetMasterId_OptionsetMaster_Id");

                    b.HasOne("RbacDashboard.DAL.Models.Access", "Parent")
                        .WithMany("InverseParent")
                        .HasForeignKey("ParentId")
                        .HasConstraintName("FK_Access_ParentId_Access_Id");

                    b.Navigation("Application");

                    b.Navigation("OptionsetMaster");

                    b.Navigation("Parent");
                });

            modelBuilder.Entity("RbacDashboard.DAL.Models.Application", b =>
                {
                    b.HasOne("RbacDashboard.DAL.Models.Customer", "Customer")
                        .WithMany("Applications")
                        .HasForeignKey("CustomerId")
                        .IsRequired()
                        .HasConstraintName("FK_Application_CustomerId_Customer_Id");

                    b.Navigation("Customer");
                });

            modelBuilder.Entity("RbacDashboard.DAL.Models.Permissionset", b =>
                {
                    b.HasOne("RbacDashboard.DAL.Models.Permissionset", "Parent")
                        .WithMany("InverseParent")
                        .HasForeignKey("ParentId")
                        .HasConstraintName("FK_Permissionset_ParentId_Permissionset_Id");

                    b.HasOne("RbacDashboard.DAL.Models.OptionsetMaster", "PermissionType")
                        .WithMany("Permissionsets")
                        .HasForeignKey("PermissionTypeId")
                        .IsRequired()
                        .HasConstraintName("FK_Permissionset_PermissionTypeId_OptionsetMaster_Id");

                    b.Navigation("Parent");

                    b.Navigation("PermissionType");
                });

            modelBuilder.Entity("RbacDashboard.DAL.Models.Role", b =>
                {
                    b.HasOne("RbacDashboard.DAL.Models.Application", "Application")
                        .WithMany("Roles")
                        .HasForeignKey("ApplicationId")
                        .IsRequired()
                        .HasConstraintName("FK_Roles_ApplicationId_Application_Id");

                    b.HasOne("RbacDashboard.DAL.Models.Role", "Parent")
                        .WithMany("InverseParent")
                        .HasForeignKey("ParentId")
                        .HasConstraintName("FK_Roles_ParentId_Roles_Id");

                    b.HasOne("RbacDashboard.DAL.Models.TypeMaster", "TypeMaster")
                        .WithMany("Roles")
                        .HasForeignKey("TypeMasterId")
                        .IsRequired()
                        .HasConstraintName("FK_Roles_TypeMasterId_TypeMaster_Id");

                    b.Navigation("Application");

                    b.Navigation("Parent");

                    b.Navigation("TypeMaster");
                });

            modelBuilder.Entity("RbacDashboard.DAL.Models.RoleAccess", b =>
                {
                    b.HasOne("RbacDashboard.DAL.Models.Access", "Access")
                        .WithMany("RoleAccesses")
                        .HasForeignKey("AccessId")
                        .IsRequired()
                        .HasConstraintName("FK_RoleAccess_AccessId_Access_Id");

                    b.HasOne("RbacDashboard.DAL.Models.Application", "Application")
                        .WithMany("RoleAccesses")
                        .HasForeignKey("ApplicationId")
                        .IsRequired()
                        .HasConstraintName("FK_RoleAccess_ApplicationId_Application_Id");

                    b.HasOne("RbacDashboard.DAL.Models.Role", "Role")
                        .WithMany("RoleAccesses")
                        .HasForeignKey("RoleId")
                        .IsRequired()
                        .HasConstraintName("FK_RoleAccess_RoleId_Roles_Id");

                    b.Navigation("Access");

                    b.Navigation("Application");

                    b.Navigation("Role");
                });

            modelBuilder.Entity("RbacDashboard.DAL.Models.TypeMaster", b =>
                {
                    b.HasOne("RbacDashboard.DAL.Models.OptionsetMaster", "OptionsetMaster")
                        .WithMany("TypeMasters")
                        .HasForeignKey("OptionsetMasterId")
                        .HasConstraintName("FK_TypeMaster_OptionsetMasterId_OptionsetMaster_Id");

                    b.Navigation("OptionsetMaster");
                });

            modelBuilder.Entity("RbacDashboard.DAL.Models.Access", b =>
                {
                    b.Navigation("InverseParent");

                    b.Navigation("RoleAccesses");
                });

            modelBuilder.Entity("RbacDashboard.DAL.Models.Application", b =>
                {
                    b.Navigation("Accesses");

                    b.Navigation("RoleAccesses");

                    b.Navigation("Roles");
                });

            modelBuilder.Entity("RbacDashboard.DAL.Models.Customer", b =>
                {
                    b.Navigation("Applications");
                });

            modelBuilder.Entity("RbacDashboard.DAL.Models.OptionsetMaster", b =>
                {
                    b.Navigation("Accesses");

                    b.Navigation("Permissionsets");

                    b.Navigation("TypeMasters");
                });

            modelBuilder.Entity("RbacDashboard.DAL.Models.Permissionset", b =>
                {
                    b.Navigation("InverseParent");
                });

            modelBuilder.Entity("RbacDashboard.DAL.Models.Role", b =>
                {
                    b.Navigation("InverseParent");

                    b.Navigation("RoleAccesses");
                });

            modelBuilder.Entity("RbacDashboard.DAL.Models.TypeMaster", b =>
                {
                    b.Navigation("Roles");
                });
#pragma warning restore 612, 618
        }
    }
}

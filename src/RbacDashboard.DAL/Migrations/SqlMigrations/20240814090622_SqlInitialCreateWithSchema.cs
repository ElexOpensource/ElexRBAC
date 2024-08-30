using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RbacDashboard.DAL.Migrations.SqlMigrations
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage(Justification = "Memory DB migration cannot be tested due to limitations.")]
    public partial class SqlInitialCreateWithSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "RBAC");

            migrationBuilder.CreateTable(
                name: "Customer",
                schema: "RBAC",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "(getdate())"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customer", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OptionsetMaster",
                schema: "RBAC",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "(getdate())"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Value = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JsonObject = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OptionsetMaster", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Application",
                schema: "RBAC",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "(getdate())"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ApplicationName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Application", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Application_CustomerId_Customer_Id",
                        column: x => x.CustomerId,
                        principalSchema: "RBAC",
                        principalTable: "Customer",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Permissionset",
                schema: "RBAC",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "(getdate())"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PermissionTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissionset", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Permissionset_ParentId_Permissionset_Id",
                        column: x => x.ParentId,
                        principalSchema: "RBAC",
                        principalTable: "Permissionset",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Permissionset_PermissionTypeId_OptionsetMaster_Id",
                        column: x => x.PermissionTypeId,
                        principalSchema: "RBAC",
                        principalTable: "OptionsetMaster",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TypeMaster",
                schema: "RBAC",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "(getdate())"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    OptionsetMasterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TypeMaster", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TypeMaster_OptionsetMasterId_OptionsetMaster_Id",
                        column: x => x.OptionsetMasterId,
                        principalSchema: "RBAC",
                        principalTable: "OptionsetMaster",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Access",
                schema: "RBAC",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "(getdate())"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    AccessName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    OptionsetMasterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MetaData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Access", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Access_ApplicationId_Application_Id",
                        column: x => x.ApplicationId,
                        principalSchema: "RBAC",
                        principalTable: "Application",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Access_OptionsetMasterId_OptionsetMaster_Id",
                        column: x => x.OptionsetMasterId,
                        principalSchema: "RBAC",
                        principalTable: "OptionsetMaster",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Access_ParentId_Access_Id",
                        column: x => x.ParentId,
                        principalSchema: "RBAC",
                        principalTable: "Access",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Role",
                schema: "RBAC",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "(getdate())"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    RoleName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TypeMasterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Roles_ApplicationId_Application_Id",
                        column: x => x.ApplicationId,
                        principalSchema: "RBAC",
                        principalTable: "Application",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Roles_ParentId_Roles_Id",
                        column: x => x.ParentId,
                        principalSchema: "RBAC",
                        principalTable: "Role",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Roles_TypeMasterId_TypeMaster_Id",
                        column: x => x.TypeMasterId,
                        principalSchema: "RBAC",
                        principalTable: "TypeMaster",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RoleAccess",
                schema: "RBAC",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "(getdate())"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccessId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccessMetaData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsOverwrite = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleAccess", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleAccess_AccessId_Access_Id",
                        column: x => x.AccessId,
                        principalSchema: "RBAC",
                        principalTable: "Access",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RoleAccess_ApplicationId_Application_Id",
                        column: x => x.ApplicationId,
                        principalSchema: "RBAC",
                        principalTable: "Application",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RoleAccess_RoleId_Roles_Id",
                        column: x => x.RoleId,
                        principalSchema: "RBAC",
                        principalTable: "Role",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Access_ApplicationId",
                schema: "RBAC",
                table: "Access",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_Access_OptionsetMasterId",
                schema: "RBAC",
                table: "Access",
                column: "OptionsetMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_Access_ParentId",
                schema: "RBAC",
                table: "Access",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Application_CustomerId",
                schema: "RBAC",
                table: "Application",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Permissionset_ParentId",
                schema: "RBAC",
                table: "Permissionset",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Permissionset_PermissionTypeId",
                schema: "RBAC",
                table: "Permissionset",
                column: "PermissionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Role_ApplicationId",
                schema: "RBAC",
                table: "Role",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_Role_ParentId",
                schema: "RBAC",
                table: "Role",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Role_TypeMasterId",
                schema: "RBAC",
                table: "Role",
                column: "TypeMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleAccess_AccessId",
                schema: "RBAC",
                table: "RoleAccess",
                column: "AccessId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleAccess_ApplicationId",
                schema: "RBAC",
                table: "RoleAccess",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleAccess_RoleId",
                schema: "RBAC",
                table: "RoleAccess",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_TypeMaster_OptionsetMasterId",
                schema: "RBAC",
                table: "TypeMaster",
                column: "OptionsetMasterId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Permissionset",
                schema: "RBAC");

            migrationBuilder.DropTable(
                name: "RoleAccess",
                schema: "RBAC");

            migrationBuilder.DropTable(
                name: "Access",
                schema: "RBAC");

            migrationBuilder.DropTable(
                name: "Role",
                schema: "RBAC");

            migrationBuilder.DropTable(
                name: "Application",
                schema: "RBAC");

            migrationBuilder.DropTable(
                name: "TypeMaster",
                schema: "RBAC");

            migrationBuilder.DropTable(
                name: "Customer",
                schema: "RBAC");

            migrationBuilder.DropTable(
                name: "OptionsetMaster",
                schema: "RBAC");
        }
    }
}

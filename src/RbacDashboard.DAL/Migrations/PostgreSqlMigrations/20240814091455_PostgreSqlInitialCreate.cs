using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RbacDashboard.DAL.Migrations.PostgreSqlMigrations
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage(Justification = "Memory DB migration cannot be tested due to limitations.")]
    public partial class PostgreSqlInitialCreate : Migration
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
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    CreatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CustomerName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
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
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Value = table.Column<Guid>(type: "uuid", nullable: false),
                    JsonObject = table.Column<string>(type: "text", nullable: false)
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
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    CreatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    ApplicationName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false)
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
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PermissionTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true)
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
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    OptionsetMasterId = table.Column<Guid>(type: "uuid", nullable: true)
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
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    CreatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    AccessName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    OptionsetMasterId = table.Column<Guid>(type: "uuid", nullable: false),
                    ApplicationId = table.Column<Guid>(type: "uuid", nullable: false),
                    MetaData = table.Column<string>(type: "text", nullable: true),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true)
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
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    CreatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    RoleName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ApplicationId = table.Column<Guid>(type: "uuid", nullable: false),
                    TypeMasterId = table.Column<Guid>(type: "uuid", nullable: false),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true)
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
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    CreatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    AccessId = table.Column<Guid>(type: "uuid", nullable: false),
                    AccessMetaData = table.Column<string>(type: "text", nullable: true),
                    ApplicationId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsOverwrite = table.Column<bool>(type: "boolean", nullable: false)
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

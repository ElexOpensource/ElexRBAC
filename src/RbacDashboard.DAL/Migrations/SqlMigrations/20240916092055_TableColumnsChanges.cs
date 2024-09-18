using Microsoft.EntityFrameworkCore.Migrations;
using System.Diagnostics.CodeAnalysis;

#nullable disable

namespace RbacDashboard.DAL.Migrations.SqlMigrations
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage(Justification = "Memory DB migration cannot be tested due to limitations.")]
    public partial class TableColumnsChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RoleName",
                schema: "RBAC",
                table: "Role",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "CustomerName",
                schema: "RBAC",
                table: "Customer",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "ApplicationName",
                schema: "RBAC",
                table: "Application",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "AccessName",
                schema: "RBAC",
                table: "Access",
                newName: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                schema: "RBAC",
                table: "Role",
                newName: "RoleName");

            migrationBuilder.RenameColumn(
                name: "Name",
                schema: "RBAC",
                table: "Customer",
                newName: "CustomerName");

            migrationBuilder.RenameColumn(
                name: "Name",
                schema: "RBAC",
                table: "Application",
                newName: "ApplicationName");

            migrationBuilder.RenameColumn(
                name: "Name",
                schema: "RBAC",
                table: "Access",
                newName: "AccessName");
        }
    }
}

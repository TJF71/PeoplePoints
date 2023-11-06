using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace contactPro2.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateZipCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ZipCodde",
                table: "Contacts",
                newName: "ZipCode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ZipCode",
                table: "Contacts",
                newName: "ZipCodde");
        }
    }
}

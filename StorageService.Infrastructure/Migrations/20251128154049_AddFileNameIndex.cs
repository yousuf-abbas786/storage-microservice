using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StorageService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFileNameIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_StoredFiles_FileName_DeletedAt",
                table: "StoredFiles",
                columns: new[] { "FileName", "DeletedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StoredFiles_FileName_DeletedAt",
                table: "StoredFiles");
        }
    }
}

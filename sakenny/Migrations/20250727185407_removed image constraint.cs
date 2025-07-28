using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace sakenny.Migrations
{
    /// <inheritdoc />
    public partial class removedimageconstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Properties_MainImageId",
                table: "Properties");

            migrationBuilder.AlterColumn<int>(
                name: "MainImageId",
                table: "Properties",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Properties_MainImageId",
                table: "Properties",
                column: "MainImageId",
                unique: true,
                filter: "[MainImageId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Properties_MainImageId",
                table: "Properties");

            migrationBuilder.AlterColumn<int>(
                name: "MainImageId",
                table: "Properties",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Properties_MainImageId",
                table: "Properties",
                column: "MainImageId",
                unique: true);
        }
    }
}

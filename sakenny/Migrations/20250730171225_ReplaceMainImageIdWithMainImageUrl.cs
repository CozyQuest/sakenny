using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace sakenny.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceMainImageIdWithMainImageUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Properties_Images_MainImageId",
                table: "Properties");

            migrationBuilder.DropForeignKey(
                name: "FK_propertySnapshots_Images_MainImageId",
                table: "propertySnapshots");

            migrationBuilder.DropIndex(
                name: "IX_propertySnapshots_MainImageId",
                table: "propertySnapshots");

            migrationBuilder.DropIndex(
                name: "IX_Properties_MainImageId",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "MainImageId",
                table: "propertySnapshots");

            migrationBuilder.DropColumn(
                name: "MainImageId",
                table: "Properties");

            migrationBuilder.AddColumn<string>(
                name: "MainImageUrl",
                table: "propertySnapshots",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MainImageUrl",
                table: "Properties",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MainImageUrl",
                table: "propertySnapshots");

            migrationBuilder.DropColumn(
                name: "MainImageUrl",
                table: "Properties");

            migrationBuilder.AddColumn<int>(
                name: "MainImageId",
                table: "propertySnapshots",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MainImageId",
                table: "Properties",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_propertySnapshots_MainImageId",
                table: "propertySnapshots",
                column: "MainImageId");

            migrationBuilder.CreateIndex(
                name: "IX_Properties_MainImageId",
                table: "Properties",
                column: "MainImageId",
                unique: true,
                filter: "[MainImageId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Properties_Images_MainImageId",
                table: "Properties",
                column: "MainImageId",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_propertySnapshots_Images_MainImageId",
                table: "propertySnapshots",
                column: "MainImageId",
                principalTable: "Images",
                principalColumn: "Id");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace sakenny.Migrations
{
    /// <inheritdoc />
    public partial class nullableReview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PropertyPermit_AspNetUsers_AdminID",
                table: "PropertyPermit");

            migrationBuilder.DropForeignKey(
                name: "FK_PropertyPermit_Properties_PropertyID",
                table: "PropertyPermit");

            migrationBuilder.DropForeignKey(
                name: "FK_propertySnapshots_PropertyPermit_PropertyPermitId",
                table: "propertySnapshots");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PropertyPermit",
                table: "PropertyPermit");

            migrationBuilder.RenameTable(
                name: "PropertyPermit",
                newName: "PropertyPermits");

            migrationBuilder.RenameIndex(
                name: "IX_PropertyPermit_PropertyID",
                table: "PropertyPermits",
                newName: "IX_PropertyPermits_PropertyID");

            migrationBuilder.RenameIndex(
                name: "IX_PropertyPermit_AdminID",
                table: "PropertyPermits",
                newName: "IX_PropertyPermits_AdminID");

            migrationBuilder.AlterColumn<string>(
                name: "ReviewText",
                table: "Reviews",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PropertyPermits",
                table: "PropertyPermits",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_PropertyPermits_AspNetUsers_AdminID",
                table: "PropertyPermits",
                column: "AdminID",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PropertyPermits_Properties_PropertyID",
                table: "PropertyPermits",
                column: "PropertyID",
                principalTable: "Properties",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_propertySnapshots_PropertyPermits_PropertyPermitId",
                table: "propertySnapshots",
                column: "PropertyPermitId",
                principalTable: "PropertyPermits",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PropertyPermits_AspNetUsers_AdminID",
                table: "PropertyPermits");

            migrationBuilder.DropForeignKey(
                name: "FK_PropertyPermits_Properties_PropertyID",
                table: "PropertyPermits");

            migrationBuilder.DropForeignKey(
                name: "FK_propertySnapshots_PropertyPermits_PropertyPermitId",
                table: "propertySnapshots");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PropertyPermits",
                table: "PropertyPermits");

            migrationBuilder.RenameTable(
                name: "PropertyPermits",
                newName: "PropertyPermit");

            migrationBuilder.RenameIndex(
                name: "IX_PropertyPermits_PropertyID",
                table: "PropertyPermit",
                newName: "IX_PropertyPermit_PropertyID");

            migrationBuilder.RenameIndex(
                name: "IX_PropertyPermits_AdminID",
                table: "PropertyPermit",
                newName: "IX_PropertyPermit_AdminID");

            migrationBuilder.AlterColumn<string>(
                name: "ReviewText",
                table: "Reviews",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PropertyPermit",
                table: "PropertyPermit",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_PropertyPermit_AspNetUsers_AdminID",
                table: "PropertyPermit",
                column: "AdminID",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PropertyPermit_Properties_PropertyID",
                table: "PropertyPermit",
                column: "PropertyID",
                principalTable: "Properties",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_propertySnapshots_PropertyPermit_PropertyPermitId",
                table: "propertySnapshots",
                column: "PropertyPermitId",
                principalTable: "PropertyPermit",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace sakenny.Migrations
{
    /// <inheritdoc />
    public partial class Proppermit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PropertyPermit");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Rentings");

            migrationBuilder.CreateTable(
                name: "PropertyPermits",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdminID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PropertyID = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyPermits", x => x.id);
                    table.ForeignKey(
                        name: "FK_PropertyPermits_AspNetUsers_AdminID",
                        column: x => x.AdminID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PropertyPermits_Properties_PropertyID",
                        column: x => x.PropertyID,
                        principalTable: "Properties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PropertyPermits_AdminID",
                table: "PropertyPermits",
                column: "AdminID");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyPermits_PropertyID",
                table: "PropertyPermits",
                column: "PropertyID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PropertyPermits");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Rentings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "PropertyPermit",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdminID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PropertyID = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyPermit", x => x.id);
                    table.ForeignKey(
                        name: "FK_PropertyPermit_AspNetUsers_AdminID",
                        column: x => x.AdminID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PropertyPermit_Properties_PropertyID",
                        column: x => x.PropertyID,
                        principalTable: "Properties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PropertyPermit_AdminID",
                table: "PropertyPermit",
                column: "AdminID");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyPermit_PropertyID",
                table: "PropertyPermit",
                column: "PropertyID");
        }
    }
}

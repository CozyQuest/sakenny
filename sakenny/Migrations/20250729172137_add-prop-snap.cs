using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace sakenny.Migrations
{
    /// <inheritdoc />
    public partial class addpropsnap : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PropertyPermits_AspNetUsers_AdminID",
                table: "PropertyPermits");

            migrationBuilder.DropForeignKey(
                name: "FK_PropertyPermits_Properties_PropertyID",
                table: "PropertyPermits");

            migrationBuilder.AddColumn<bool>(
                name: "status",
                table: "Properties",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "propertySnapshots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PropertyId = table.Column<int>(type: "int", nullable: true),
                    PropertyPermitId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PropertyTypeId = table.Column<int>(type: "int", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    District = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BuildingNo = table.Column<int>(type: "int", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: true),
                    FlatNo = table.Column<int>(type: "int", nullable: true),
                    Longitude = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Latitude = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RoomCount = table.Column<int>(type: "int", nullable: false),
                    BathroomCount = table.Column<int>(type: "int", nullable: false),
                    Space = table.Column<double>(type: "float", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PeopleCapacity = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MainImageId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_propertySnapshots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_propertySnapshots_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_propertySnapshots_Images_MainImageId",
                        column: x => x.MainImageId,
                        principalTable: "Images",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_propertySnapshots_Properties_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "Properties",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_propertySnapshots_PropertyPermits_PropertyPermitId",
                        column: x => x.PropertyPermitId,
                        principalTable: "PropertyPermits",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_propertySnapshots_PropertyTypes_PropertyTypeId",
                        column: x => x.PropertyTypeId,
                        principalTable: "PropertyTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PropertySnapshotService",
                columns: table => new
                {
                    PropertySnapshotsId = table.Column<int>(type: "int", nullable: false),
                    ServicesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertySnapshotService", x => new { x.PropertySnapshotsId, x.ServicesId });
                    table.ForeignKey(
                        name: "FK_PropertySnapshotService_Services_ServicesId",
                        column: x => x.ServicesId,
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PropertySnapshotService_propertySnapshots_PropertySnapshotsId",
                        column: x => x.PropertySnapshotsId,
                        principalTable: "propertySnapshots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_propertySnapshots_MainImageId",
                table: "propertySnapshots",
                column: "MainImageId");

            migrationBuilder.CreateIndex(
                name: "IX_propertySnapshots_PropertyId",
                table: "propertySnapshots",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_propertySnapshots_PropertyPermitId",
                table: "propertySnapshots",
                column: "PropertyPermitId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_propertySnapshots_PropertyTypeId",
                table: "propertySnapshots",
                column: "PropertyTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_propertySnapshots_UserId",
                table: "propertySnapshots",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertySnapshotService_ServicesId",
                table: "PropertySnapshotService",
                column: "ServicesId");

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

            migrationBuilder.DropTable(
                name: "PropertySnapshotService");

            migrationBuilder.DropTable(
                name: "propertySnapshots");

            migrationBuilder.DropColumn(
                name: "status",
                table: "Properties");

            migrationBuilder.AddForeignKey(
                name: "FK_PropertyPermits_AspNetUsers_AdminID",
                table: "PropertyPermits",
                column: "AdminID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PropertyPermits_Properties_PropertyID",
                table: "PropertyPermits",
                column: "PropertyID",
                principalTable: "Properties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

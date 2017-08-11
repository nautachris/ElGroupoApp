using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ElGroupo.Domain.Migrations
{
    public partial class addEvents : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ElGroupoEventGroup",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    OwnerId = table.Column<int>(nullable: true),
                    UserCreated = table.Column<string>(nullable: true),
                    UserUpdated = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElGroupoEventGroup", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ElGroupoEventGroup_ElGroupoUser_OwnerId",
                        column: x => x.OwnerId,
                        principalSchema: "dbo",
                        principalTable: "ElGroupoUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ElGroupoEvent",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    CheckInLocationTolerance = table.Column<double>(nullable: false),
                    CheckInTimeTolerance = table.Column<int>(nullable: false),
                    CoordinateX = table.Column<double>(nullable: false),
                    CoordinateY = table.Column<double>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    EndTime = table.Column<DateTime>(nullable: false),
                    GroupId = table.Column<long>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    OrganizerId = table.Column<int>(nullable: true),
                    SavedAsDraft = table.Column<bool>(nullable: false),
                    StartTime = table.Column<DateTime>(nullable: false),
                    UserCreated = table.Column<string>(nullable: true),
                    UserUpdated = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElGroupoEvent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ElGroupoEvent_ElGroupoEventGroup_GroupId",
                        column: x => x.GroupId,
                        principalTable: "ElGroupoEventGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ElGroupoEvent_ElGroupoUser_OrganizerId",
                        column: x => x.OrganizerId,
                        principalSchema: "dbo",
                        principalTable: "ElGroupoUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ElGroupoEvent_GroupId",
                table: "ElGroupoEvent",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_ElGroupoEvent_OrganizerId",
                table: "ElGroupoEvent",
                column: "OrganizerId");

            migrationBuilder.CreateIndex(
                name: "IX_ElGroupoEventGroup_OwnerId",
                table: "ElGroupoEventGroup",
                column: "OwnerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ElGroupoEvent");

            migrationBuilder.DropTable(
                name: "ElGroupoEventGroup");
        }
    }
}

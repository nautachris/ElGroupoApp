using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ElGroupo.Domain.Migrations
{
    public partial class attendeegroups : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "AttendeeGroupId",
                schema: "dbo",
                table: "User",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AttendeeGroup",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    UserCreated = table.Column<string>(nullable: true),
                    UserId = table.Column<long>(nullable: true),
                    UserUpdated = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttendeeGroup", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttendeeGroup_User_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_User_AttendeeGroupId",
                schema: "dbo",
                table: "User",
                column: "AttendeeGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendeeGroup_UserId",
                table: "AttendeeGroup",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_User_AttendeeGroup_AttendeeGroupId",
                schema: "dbo",
                table: "User",
                column: "AttendeeGroupId",
                principalTable: "AttendeeGroup",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_AttendeeGroup_AttendeeGroupId",
                schema: "dbo",
                table: "User");

            migrationBuilder.DropTable(
                name: "AttendeeGroup");

            migrationBuilder.DropIndex(
                name: "IX_User_AttendeeGroupId",
                schema: "dbo",
                table: "User");

            migrationBuilder.DropColumn(
                name: "AttendeeGroupId",
                schema: "dbo",
                table: "User");
        }
    }
}

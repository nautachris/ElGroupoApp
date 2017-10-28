using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ElGroupo.Domain.Migrations
{
    public partial class changedattendeegroup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_AttendeeGroup_AttendeeGroupId",
                schema: "dbo",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_User_AttendeeGroupId",
                schema: "dbo",
                table: "User");

            migrationBuilder.DropColumn(
                name: "AttendeeGroupId",
                schema: "dbo",
                table: "User");

            migrationBuilder.CreateTable(
                name: "AttendeeGroupUser",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    AttendeeGroupId = table.Column<long>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: false),
                    UserCreated = table.Column<string>(nullable: true),
                    UserId = table.Column<long>(nullable: true),
                    UserUpdated = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttendeeGroupUser", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttendeeGroupUser_AttendeeGroup_AttendeeGroupId",
                        column: x => x.AttendeeGroupId,
                        principalTable: "AttendeeGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AttendeeGroupUser_User_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AttendeeGroupUser_AttendeeGroupId",
                table: "AttendeeGroupUser",
                column: "AttendeeGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendeeGroupUser_UserId",
                table: "AttendeeGroupUser",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AttendeeGroupUser");

            migrationBuilder.AddColumn<long>(
                name: "AttendeeGroupId",
                schema: "dbo",
                table: "User",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_AttendeeGroupId",
                schema: "dbo",
                table: "User",
                column: "AttendeeGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_User_AttendeeGroup_AttendeeGroupId",
                schema: "dbo",
                table: "User",
                column: "AttendeeGroupId",
                principalTable: "AttendeeGroup",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

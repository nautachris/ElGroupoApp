using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ElGroupo.Domain.Migrations
{
    public partial class linkactivitygroupdirectlytouser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "UserId",
                table: "ActivityGroups",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ActivityGroups_UserId",
                table: "ActivityGroups",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityGroups_User_UserId",
                table: "ActivityGroups",
                column: "UserId",
                principalSchema: "dbo",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActivityGroups_User_UserId",
                table: "ActivityGroups");

            migrationBuilder.DropIndex(
                name: "IX_ActivityGroups_UserId",
                table: "ActivityGroups");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ActivityGroups");
        }
    }
}

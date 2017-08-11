using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ElGroupo.Domain.Migrations
{
    public partial class bears323 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContactGroupUser_User_ElGroupoUserId1",
                schema: "dbo",
                table: "ContactGroupUser");

            migrationBuilder.DropForeignKey(
                name: "FK_ContactGroupUser_ContactGroups_GroupId",
                schema: "dbo",
                table: "ContactGroupUser");

            migrationBuilder.DropColumn(
                name: "ElGroupoUserId",
                schema: "dbo",
                table: "ContactGroupUser");

            migrationBuilder.RenameColumn(
                name: "ElGroupoUserId1",
                schema: "dbo",
                table: "ContactGroupUser",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_ContactGroupUser_ElGroupoUserId1",
                schema: "dbo",
                table: "ContactGroupUser",
                newName: "IX_ContactGroupUser_UserId");

            migrationBuilder.AlterColumn<long>(
                name: "GroupId",
                schema: "dbo",
                table: "ContactGroupUser",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AddForeignKey(
                name: "FK_ContactGroupUser_ContactGroups_GroupId",
                schema: "dbo",
                table: "ContactGroupUser",
                column: "GroupId",
                principalTable: "ContactGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ContactGroupUser_User_UserId",
                schema: "dbo",
                table: "ContactGroupUser",
                column: "UserId",
                principalSchema: "dbo",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContactGroupUser_ContactGroups_GroupId",
                schema: "dbo",
                table: "ContactGroupUser");

            migrationBuilder.DropForeignKey(
                name: "FK_ContactGroupUser_User_UserId",
                schema: "dbo",
                table: "ContactGroupUser");

            migrationBuilder.RenameColumn(
                name: "UserId",
                schema: "dbo",
                table: "ContactGroupUser",
                newName: "ElGroupoUserId1");

            migrationBuilder.RenameIndex(
                name: "IX_ContactGroupUser_UserId",
                schema: "dbo",
                table: "ContactGroupUser",
                newName: "IX_ContactGroupUser_ElGroupoUserId1");

            migrationBuilder.AlterColumn<long>(
                name: "GroupId",
                schema: "dbo",
                table: "ContactGroupUser",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ElGroupoUserId",
                schema: "dbo",
                table: "ContactGroupUser",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddForeignKey(
                name: "FK_ContactGroupUser_User_ElGroupoUserId1",
                schema: "dbo",
                table: "ContactGroupUser",
                column: "ElGroupoUserId1",
                principalSchema: "dbo",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ContactGroupUser_ContactGroups_GroupId",
                schema: "dbo",
                table: "ContactGroupUser",
                column: "GroupId",
                principalTable: "ContactGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ElGroupo.Domain.Migrations
{
    public partial class addContactGroups2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContactGroupUsers_ContactGroups_GroupId",
                table: "ContactGroupUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_ContactGroupUsers_ElGroupoUser_UserId",
                table: "ContactGroupUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ContactGroupUsers",
                table: "ContactGroupUsers");

            migrationBuilder.RenameTable(
                name: "ContactGroupUsers",
                newName: "ContactGroupUser",
                newSchema: "dbo");

            migrationBuilder.RenameColumn(
                name: "UserId",
                schema: "dbo",
                table: "ContactGroupUser",
                newName: "ElGroupoUserId");

            migrationBuilder.RenameIndex(
                name: "IX_ContactGroupUsers_UserId",
                schema: "dbo",
                table: "ContactGroupUser",
                newName: "IX_ContactGroupUser_ElGroupoUserId");

            migrationBuilder.RenameIndex(
                name: "IX_ContactGroupUsers_GroupId",
                schema: "dbo",
                table: "ContactGroupUser",
                newName: "IX_ContactGroupUser_GroupId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ContactGroupUser",
                schema: "dbo",
                table: "ContactGroupUser",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ContactGroupUser_ElGroupoUser_ElGroupoUserId",
                schema: "dbo",
                table: "ContactGroupUser",
                column: "ElGroupoUserId",
                principalSchema: "dbo",
                principalTable: "ElGroupoUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ContactGroupUser_ContactGroups_GroupId",
                schema: "dbo",
                table: "ContactGroupUser",
                column: "GroupId",
                principalTable: "ContactGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContactGroupUser_ElGroupoUser_ElGroupoUserId",
                schema: "dbo",
                table: "ContactGroupUser");

            migrationBuilder.DropForeignKey(
                name: "FK_ContactGroupUser_ContactGroups_GroupId",
                schema: "dbo",
                table: "ContactGroupUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ContactGroupUser",
                schema: "dbo",
                table: "ContactGroupUser");

            migrationBuilder.RenameTable(
                name: "ContactGroupUser",
                schema: "dbo",
                newName: "ContactGroupUsers");

            migrationBuilder.RenameColumn(
                name: "ElGroupoUserId",
                table: "ContactGroupUsers",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_ContactGroupUser_GroupId",
                table: "ContactGroupUsers",
                newName: "IX_ContactGroupUsers_GroupId");

            migrationBuilder.RenameIndex(
                name: "IX_ContactGroupUser_ElGroupoUserId",
                table: "ContactGroupUsers",
                newName: "IX_ContactGroupUsers_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ContactGroupUsers",
                table: "ContactGroupUsers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ContactGroupUsers_ContactGroups_GroupId",
                table: "ContactGroupUsers",
                column: "GroupId",
                principalTable: "ContactGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ContactGroupUsers_ElGroupoUser_UserId",
                table: "ContactGroupUsers",
                column: "UserId",
                principalSchema: "dbo",
                principalTable: "ElGroupoUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

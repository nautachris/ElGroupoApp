using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ElGroupo.Domain.Migrations
{
    public partial class test2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContactGroupUser_ContactGroups_ContactGroupId",
                schema: "dbo",
                table: "ContactGroupUser");

            migrationBuilder.DropIndex(
                name: "IX_ContactGroupUser_ContactGroupId",
                schema: "dbo",
                table: "ContactGroupUser");

            migrationBuilder.DropColumn(
                name: "ContactGroupId",
                schema: "dbo",
                table: "ContactGroupUser");

            migrationBuilder.AlterColumn<long>(
                name: "GroupId",
                schema: "dbo",
                table: "ContactGroupUser",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<int>(
                name: "ElGroupoUserId",
                schema: "dbo",
                table: "ContactGroupUser",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.CreateIndex(
                name: "IX_ContactGroupUser_ElGroupoUserId",
                schema: "dbo",
                table: "ContactGroupUser",
                column: "ElGroupoUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ContactGroupUser_GroupId",
                schema: "dbo",
                table: "ContactGroupUser",
                column: "GroupId");

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

            migrationBuilder.DropIndex(
                name: "IX_ContactGroupUser_ElGroupoUserId",
                schema: "dbo",
                table: "ContactGroupUser");

            migrationBuilder.DropIndex(
                name: "IX_ContactGroupUser_GroupId",
                schema: "dbo",
                table: "ContactGroupUser");

            migrationBuilder.AlterColumn<long>(
                name: "GroupId",
                schema: "dbo",
                table: "ContactGroupUser",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "ElGroupoUserId",
                schema: "dbo",
                table: "ContactGroupUser",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ContactGroupId",
                schema: "dbo",
                table: "ContactGroupUser",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContactGroupUser_ContactGroupId",
                schema: "dbo",
                table: "ContactGroupUser",
                column: "ContactGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_ContactGroupUser_ContactGroups_ContactGroupId",
                schema: "dbo",
                table: "ContactGroupUser",
                column: "ContactGroupId",
                principalTable: "ContactGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ElGroupo.Domain.Migrations
{
    public partial class test : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContactGroupUser_ElGroupoUser_ElGroupoUserId1",
                schema: "dbo",
                table: "ContactGroupUser");

            migrationBuilder.DropForeignKey(
                name: "FK_ContactGroupUser_ContactGroups_GroupId",
                schema: "dbo",
                table: "ContactGroupUser");

            migrationBuilder.DropIndex(
                name: "IX_ContactGroupUser_ElGroupoUserId1",
                schema: "dbo",
                table: "ContactGroupUser");

            migrationBuilder.DropIndex(
                name: "IX_ContactGroupUser_GroupId",
                schema: "dbo",
                table: "ContactGroupUser");

            migrationBuilder.DropColumn(
                name: "ElGroupoUserId1",
                schema: "dbo",
                table: "ContactGroupUser");

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

        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<int>(
                name: "ElGroupoUserId1",
                schema: "dbo",
                table: "ContactGroupUser",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContactGroupUser_ElGroupoUserId1",
                schema: "dbo",
                table: "ContactGroupUser",
                column: "ElGroupoUserId1");

            migrationBuilder.CreateIndex(
                name: "IX_ContactGroupUser_GroupId",
                schema: "dbo",
                table: "ContactGroupUser",
                column: "GroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_ContactGroupUser_ElGroupoUser_ElGroupoUserId1",
                schema: "dbo",
                table: "ContactGroupUser",
                column: "ElGroupoUserId1",
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
                onDelete: ReferentialAction.Cascade);
        }
    }
}

using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ElGroupo.Domain.Migrations
{
    public partial class changetablename : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserContacts_ContactMethods_ContactMethodId",
                table: "UserContacts");

            migrationBuilder.DropForeignKey(
                name: "FK_UserContacts_User_UserId",
                table: "UserContacts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserContacts",
                table: "UserContacts");

            migrationBuilder.RenameTable(
                name: "UserContacts",
                newName: "UserContactMethods");

            migrationBuilder.RenameIndex(
                name: "IX_UserContacts_UserId",
                table: "UserContactMethods",
                newName: "IX_UserContactMethods_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserContacts_ContactMethodId",
                table: "UserContactMethods",
                newName: "IX_UserContactMethods_ContactMethodId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserContactMethods",
                table: "UserContactMethods",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserContactMethods_ContactMethods_ContactMethodId",
                table: "UserContactMethods",
                column: "ContactMethodId",
                principalSchema: "dbo",
                principalTable: "ContactMethods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserContactMethods_User_UserId",
                table: "UserContactMethods",
                column: "UserId",
                principalSchema: "dbo",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserContactMethods_ContactMethods_ContactMethodId",
                table: "UserContactMethods");

            migrationBuilder.DropForeignKey(
                name: "FK_UserContactMethods_User_UserId",
                table: "UserContactMethods");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserContactMethods",
                table: "UserContactMethods");

            migrationBuilder.RenameTable(
                name: "UserContactMethods",
                newName: "UserContacts");

            migrationBuilder.RenameIndex(
                name: "IX_UserContactMethods_UserId",
                table: "UserContacts",
                newName: "IX_UserContacts_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserContactMethods_ContactMethodId",
                table: "UserContacts",
                newName: "IX_UserContacts_ContactMethodId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserContacts",
                table: "UserContacts",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserContacts_ContactMethods_ContactMethodId",
                table: "UserContacts",
                column: "ContactMethodId",
                principalSchema: "dbo",
                principalTable: "ContactMethods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserContacts_User_UserId",
                table: "UserContacts",
                column: "UserId",
                principalSchema: "dbo",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ElGroupo.Domain.Migrations
{
    public partial class renamed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContactGroups_ElGroupoUser_ElGroupoUserId",
                table: "ContactGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_ContactGroupUser_ElGroupoUser_ElGroupoUserId",
                schema: "dbo",
                table: "ContactGroupUser");

            migrationBuilder.DropForeignKey(
                name: "FK_ContactGroupUser_ContactGroups_GroupId",
                schema: "dbo",
                table: "ContactGroupUser");

            migrationBuilder.DropForeignKey(
                name: "FK_EventAttendees_ElGroupoUser_UserId",
                table: "EventAttendees");

            migrationBuilder.DropForeignKey(
                name: "FK_EventGroups_ElGroupoUser_OwnerId",
                table: "EventGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_EventOrganizer_ElGroupoUser_UserId",
                table: "EventOrganizer");

            migrationBuilder.DropForeignKey(
                name: "FK_MessageBoardItems_ElGroupoUser_PostedByUserId",
                table: "MessageBoardItems");

            migrationBuilder.DropForeignKey(
                name: "FK_UserContact_ElGroupoUser_UserId",
                table: "UserContact");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserClaims_ElGroupoUser_UserId",
                table: "AspNetUserClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserLogins_ElGroupoUser_UserId",
                table: "AspNetUserLogins");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_ElGroupoUser_UserId",
                table: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "ElGroupoUser",
                schema: "dbo");

            migrationBuilder.DropIndex(
                name: "IX_ContactGroupUser_ElGroupoUserId",
                schema: "dbo",
                table: "ContactGroupUser");

            migrationBuilder.RenameColumn(
                name: "ElGroupoUserId",
                table: "ContactGroups",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_ContactGroups_ElGroupoUserId",
                table: "ContactGroups",
                newName: "IX_ContactGroups_UserId");

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

            migrationBuilder.AddColumn<int>(
                name: "ElGroupoUserId1",
                schema: "dbo",
                table: "ContactGroupUser",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "User",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    EmailAddress = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    PasswordHash = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    SecurityStamp = table.Column<string>(nullable: true),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContactGroupUser_ElGroupoUserId1",
                schema: "dbo",
                table: "ContactGroupUser",
                column: "ElGroupoUserId1");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                schema: "dbo",
                table: "User",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                schema: "dbo",
                table: "User",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ContactGroups_User_UserId",
                table: "ContactGroups",
                column: "UserId",
                principalSchema: "dbo",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

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

            migrationBuilder.AddForeignKey(
                name: "FK_EventAttendees_User_UserId",
                table: "EventAttendees",
                column: "UserId",
                principalSchema: "dbo",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EventGroups_User_OwnerId",
                table: "EventGroups",
                column: "OwnerId",
                principalSchema: "dbo",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EventOrganizer_User_UserId",
                table: "EventOrganizer",
                column: "UserId",
                principalSchema: "dbo",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MessageBoardItems_User_PostedByUserId",
                table: "MessageBoardItems",
                column: "PostedByUserId",
                principalSchema: "dbo",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserContact_User_UserId",
                table: "UserContact",
                column: "UserId",
                principalSchema: "dbo",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserClaims_User_UserId",
                table: "AspNetUserClaims",
                column: "UserId",
                principalSchema: "dbo",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserLogins_User_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalSchema: "dbo",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_User_UserId",
                table: "AspNetUserRoles",
                column: "UserId",
                principalSchema: "dbo",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContactGroups_User_UserId",
                table: "ContactGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_ContactGroupUser_User_ElGroupoUserId1",
                schema: "dbo",
                table: "ContactGroupUser");

            migrationBuilder.DropForeignKey(
                name: "FK_ContactGroupUser_ContactGroups_GroupId",
                schema: "dbo",
                table: "ContactGroupUser");

            migrationBuilder.DropForeignKey(
                name: "FK_EventAttendees_User_UserId",
                table: "EventAttendees");

            migrationBuilder.DropForeignKey(
                name: "FK_EventGroups_User_OwnerId",
                table: "EventGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_EventOrganizer_User_UserId",
                table: "EventOrganizer");

            migrationBuilder.DropForeignKey(
                name: "FK_MessageBoardItems_User_PostedByUserId",
                table: "MessageBoardItems");

            migrationBuilder.DropForeignKey(
                name: "FK_UserContact_User_UserId",
                table: "UserContact");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserClaims_User_UserId",
                table: "AspNetUserClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserLogins_User_UserId",
                table: "AspNetUserLogins");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_User_UserId",
                table: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "User",
                schema: "dbo");

            migrationBuilder.DropIndex(
                name: "IX_ContactGroupUser_ElGroupoUserId1",
                schema: "dbo",
                table: "ContactGroupUser");

            migrationBuilder.DropColumn(
                name: "ElGroupoUserId1",
                schema: "dbo",
                table: "ContactGroupUser");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "ContactGroups",
                newName: "ElGroupoUserId");

            migrationBuilder.RenameIndex(
                name: "IX_ContactGroups_UserId",
                table: "ContactGroups",
                newName: "IX_ContactGroups_ElGroupoUserId");

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

            migrationBuilder.CreateTable(
                name: "ElGroupoUser",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    EmailAddress = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    PasswordHash = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    SecurityStamp = table.Column<string>(nullable: true),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElGroupoUser", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContactGroupUser_ElGroupoUserId",
                schema: "dbo",
                table: "ContactGroupUser",
                column: "ElGroupoUserId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                schema: "dbo",
                table: "ElGroupoUser",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                schema: "dbo",
                table: "ElGroupoUser",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ContactGroups_ElGroupoUser_ElGroupoUserId",
                table: "ContactGroups",
                column: "ElGroupoUserId",
                principalSchema: "dbo",
                principalTable: "ElGroupoUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

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

            migrationBuilder.AddForeignKey(
                name: "FK_EventAttendees_ElGroupoUser_UserId",
                table: "EventAttendees",
                column: "UserId",
                principalSchema: "dbo",
                principalTable: "ElGroupoUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EventGroups_ElGroupoUser_OwnerId",
                table: "EventGroups",
                column: "OwnerId",
                principalSchema: "dbo",
                principalTable: "ElGroupoUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EventOrganizer_ElGroupoUser_UserId",
                table: "EventOrganizer",
                column: "UserId",
                principalSchema: "dbo",
                principalTable: "ElGroupoUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MessageBoardItems_ElGroupoUser_PostedByUserId",
                table: "MessageBoardItems",
                column: "PostedByUserId",
                principalSchema: "dbo",
                principalTable: "ElGroupoUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserContact_ElGroupoUser_UserId",
                table: "UserContact",
                column: "UserId",
                principalSchema: "dbo",
                principalTable: "ElGroupoUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserClaims_ElGroupoUser_UserId",
                table: "AspNetUserClaims",
                column: "UserId",
                principalSchema: "dbo",
                principalTable: "ElGroupoUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserLogins_ElGroupoUser_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalSchema: "dbo",
                principalTable: "ElGroupoUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_ElGroupoUser_UserId",
                table: "AspNetUserRoles",
                column: "UserId",
                principalSchema: "dbo",
                principalTable: "ElGroupoUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

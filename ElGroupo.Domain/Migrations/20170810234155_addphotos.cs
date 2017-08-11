using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ElGroupo.Domain.Migrations
{
    public partial class addphotos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventAttendeeNotification_EventAttendees_AttendeeId",
                table: "EventAttendeeNotification");

            migrationBuilder.DropForeignKey(
                name: "FK_EventAttendeeNotification_EventNotification_NotificationId",
                table: "EventAttendeeNotification");

            migrationBuilder.DropForeignKey(
                name: "FK_EventNotification_Events_EventId",
                table: "EventNotification");

            migrationBuilder.DropForeignKey(
                name: "FK_EventNotification_EventOrganizer_EventOrganizerId",
                table: "EventNotification");

            migrationBuilder.DropForeignKey(
                name: "FK_EventOrganizer_Events_EventId",
                table: "EventOrganizer");

            migrationBuilder.DropForeignKey(
                name: "FK_EventOrganizer_User_UserId",
                table: "EventOrganizer");

            migrationBuilder.DropForeignKey(
                name: "FK_MessageBoardItemAttendee_EventAttendees_AttendeeId",
                table: "MessageBoardItemAttendee");

            migrationBuilder.DropForeignKey(
                name: "FK_MessageBoardItemAttendee_MessageBoardItems_MessageBoardItemId",
                table: "MessageBoardItemAttendee");

            migrationBuilder.DropForeignKey(
                name: "FK_UserContact_ContactTypes_ContactTypeId",
                table: "UserContact");

            migrationBuilder.DropForeignKey(
                name: "FK_UserContact_User_UserId",
                table: "UserContact");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserContact",
                table: "UserContact");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MessageBoardItemAttendee",
                table: "MessageBoardItemAttendee");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EventOrganizer",
                table: "EventOrganizer");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EventNotification",
                table: "EventNotification");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EventAttendeeNotification",
                table: "EventAttendeeNotification");

            migrationBuilder.RenameTable(
                name: "UserContact",
                newName: "UserContacts");

            migrationBuilder.RenameTable(
                name: "MessageBoardItemAttendee",
                newName: "MessageBoardItemAttendees");

            migrationBuilder.RenameTable(
                name: "EventOrganizer",
                newName: "EventOrganizers");

            migrationBuilder.RenameTable(
                name: "EventNotification",
                newName: "EventNotifications");

            migrationBuilder.RenameTable(
                name: "EventAttendeeNotification",
                newName: "EventAttendeeNotifications");

            migrationBuilder.RenameIndex(
                name: "IX_UserContact_UserId",
                table: "UserContacts",
                newName: "IX_UserContacts_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserContact_ContactTypeId",
                table: "UserContacts",
                newName: "IX_UserContacts_ContactTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_MessageBoardItemAttendee_MessageBoardItemId",
                table: "MessageBoardItemAttendees",
                newName: "IX_MessageBoardItemAttendees_MessageBoardItemId");

            migrationBuilder.RenameIndex(
                name: "IX_MessageBoardItemAttendee_AttendeeId",
                table: "MessageBoardItemAttendees",
                newName: "IX_MessageBoardItemAttendees_AttendeeId");

            migrationBuilder.RenameIndex(
                name: "IX_EventOrganizer_UserId",
                table: "EventOrganizers",
                newName: "IX_EventOrganizers_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_EventOrganizer_EventId",
                table: "EventOrganizers",
                newName: "IX_EventOrganizers_EventId");

            migrationBuilder.RenameIndex(
                name: "IX_EventNotification_EventOrganizerId",
                table: "EventNotifications",
                newName: "IX_EventNotifications_EventOrganizerId");

            migrationBuilder.RenameIndex(
                name: "IX_EventNotification_EventId",
                table: "EventNotifications",
                newName: "IX_EventNotifications_EventId");

            migrationBuilder.RenameIndex(
                name: "IX_EventAttendeeNotification_NotificationId",
                table: "EventAttendeeNotifications",
                newName: "IX_EventAttendeeNotifications_NotificationId");

            migrationBuilder.RenameIndex(
                name: "IX_EventAttendeeNotification_AttendeeId",
                table: "EventAttendeeNotifications",
                newName: "IX_EventAttendeeNotifications_AttendeeId");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                schema: "dbo",
                table: "User",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "PhotoId",
                schema: "dbo",
                table: "User",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ZipCode",
                schema: "dbo",
                table: "User",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserContacts",
                table: "UserContacts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MessageBoardItemAttendees",
                table: "MessageBoardItemAttendees",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventOrganizers",
                table: "EventOrganizers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventNotifications",
                table: "EventNotifications",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventAttendeeNotifications",
                table: "EventAttendeeNotifications",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "UserPhoto",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    ContentType = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: false),
                    ImageData = table.Column<byte[]>(nullable: true),
                    UserCreated = table.Column<string>(nullable: true),
                    UserUpdated = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPhoto", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_User_PhotoId",
                schema: "dbo",
                table: "User",
                column: "PhotoId");

            migrationBuilder.AddForeignKey(
                name: "FK_EventAttendeeNotifications_EventAttendees_AttendeeId",
                table: "EventAttendeeNotifications",
                column: "AttendeeId",
                principalTable: "EventAttendees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EventAttendeeNotifications_EventNotifications_NotificationId",
                table: "EventAttendeeNotifications",
                column: "NotificationId",
                principalTable: "EventNotifications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EventNotifications_Events_EventId",
                table: "EventNotifications",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EventNotifications_EventOrganizers_EventOrganizerId",
                table: "EventNotifications",
                column: "EventOrganizerId",
                principalTable: "EventOrganizers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EventOrganizers_Events_EventId",
                table: "EventOrganizers",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EventOrganizers_User_UserId",
                table: "EventOrganizers",
                column: "UserId",
                principalSchema: "dbo",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MessageBoardItemAttendees_EventAttendees_AttendeeId",
                table: "MessageBoardItemAttendees",
                column: "AttendeeId",
                principalTable: "EventAttendees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MessageBoardItemAttendees_MessageBoardItems_MessageBoardItemId",
                table: "MessageBoardItemAttendees",
                column: "MessageBoardItemId",
                principalTable: "MessageBoardItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_User_UserPhoto_PhotoId",
                schema: "dbo",
                table: "User",
                column: "PhotoId",
                principalTable: "UserPhoto",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserContacts_ContactTypes_ContactTypeId",
                table: "UserContacts",
                column: "ContactTypeId",
                principalSchema: "dbo",
                principalTable: "ContactTypes",
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventAttendeeNotifications_EventAttendees_AttendeeId",
                table: "EventAttendeeNotifications");

            migrationBuilder.DropForeignKey(
                name: "FK_EventAttendeeNotifications_EventNotifications_NotificationId",
                table: "EventAttendeeNotifications");

            migrationBuilder.DropForeignKey(
                name: "FK_EventNotifications_Events_EventId",
                table: "EventNotifications");

            migrationBuilder.DropForeignKey(
                name: "FK_EventNotifications_EventOrganizers_EventOrganizerId",
                table: "EventNotifications");

            migrationBuilder.DropForeignKey(
                name: "FK_EventOrganizers_Events_EventId",
                table: "EventOrganizers");

            migrationBuilder.DropForeignKey(
                name: "FK_EventOrganizers_User_UserId",
                table: "EventOrganizers");

            migrationBuilder.DropForeignKey(
                name: "FK_MessageBoardItemAttendees_EventAttendees_AttendeeId",
                table: "MessageBoardItemAttendees");

            migrationBuilder.DropForeignKey(
                name: "FK_MessageBoardItemAttendees_MessageBoardItems_MessageBoardItemId",
                table: "MessageBoardItemAttendees");

            migrationBuilder.DropForeignKey(
                name: "FK_User_UserPhoto_PhotoId",
                schema: "dbo",
                table: "User");

            migrationBuilder.DropForeignKey(
                name: "FK_UserContacts_ContactTypes_ContactTypeId",
                table: "UserContacts");

            migrationBuilder.DropForeignKey(
                name: "FK_UserContacts_User_UserId",
                table: "UserContacts");

            migrationBuilder.DropTable(
                name: "UserPhoto");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserContacts",
                table: "UserContacts");

            migrationBuilder.DropIndex(
                name: "IX_User_PhotoId",
                schema: "dbo",
                table: "User");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MessageBoardItemAttendees",
                table: "MessageBoardItemAttendees");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EventOrganizers",
                table: "EventOrganizers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EventNotifications",
                table: "EventNotifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EventAttendeeNotifications",
                table: "EventAttendeeNotifications");

            migrationBuilder.DropColumn(
                name: "Name",
                schema: "dbo",
                table: "User");

            migrationBuilder.DropColumn(
                name: "PhotoId",
                schema: "dbo",
                table: "User");

            migrationBuilder.DropColumn(
                name: "ZipCode",
                schema: "dbo",
                table: "User");

            migrationBuilder.RenameTable(
                name: "UserContacts",
                newName: "UserContact");

            migrationBuilder.RenameTable(
                name: "MessageBoardItemAttendees",
                newName: "MessageBoardItemAttendee");

            migrationBuilder.RenameTable(
                name: "EventOrganizers",
                newName: "EventOrganizer");

            migrationBuilder.RenameTable(
                name: "EventNotifications",
                newName: "EventNotification");

            migrationBuilder.RenameTable(
                name: "EventAttendeeNotifications",
                newName: "EventAttendeeNotification");

            migrationBuilder.RenameIndex(
                name: "IX_UserContacts_UserId",
                table: "UserContact",
                newName: "IX_UserContact_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserContacts_ContactTypeId",
                table: "UserContact",
                newName: "IX_UserContact_ContactTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_MessageBoardItemAttendees_MessageBoardItemId",
                table: "MessageBoardItemAttendee",
                newName: "IX_MessageBoardItemAttendee_MessageBoardItemId");

            migrationBuilder.RenameIndex(
                name: "IX_MessageBoardItemAttendees_AttendeeId",
                table: "MessageBoardItemAttendee",
                newName: "IX_MessageBoardItemAttendee_AttendeeId");

            migrationBuilder.RenameIndex(
                name: "IX_EventOrganizers_UserId",
                table: "EventOrganizer",
                newName: "IX_EventOrganizer_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_EventOrganizers_EventId",
                table: "EventOrganizer",
                newName: "IX_EventOrganizer_EventId");

            migrationBuilder.RenameIndex(
                name: "IX_EventNotifications_EventOrganizerId",
                table: "EventNotification",
                newName: "IX_EventNotification_EventOrganizerId");

            migrationBuilder.RenameIndex(
                name: "IX_EventNotifications_EventId",
                table: "EventNotification",
                newName: "IX_EventNotification_EventId");

            migrationBuilder.RenameIndex(
                name: "IX_EventAttendeeNotifications_NotificationId",
                table: "EventAttendeeNotification",
                newName: "IX_EventAttendeeNotification_NotificationId");

            migrationBuilder.RenameIndex(
                name: "IX_EventAttendeeNotifications_AttendeeId",
                table: "EventAttendeeNotification",
                newName: "IX_EventAttendeeNotification_AttendeeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserContact",
                table: "UserContact",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MessageBoardItemAttendee",
                table: "MessageBoardItemAttendee",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventOrganizer",
                table: "EventOrganizer",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventNotification",
                table: "EventNotification",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventAttendeeNotification",
                table: "EventAttendeeNotification",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EventAttendeeNotification_EventAttendees_AttendeeId",
                table: "EventAttendeeNotification",
                column: "AttendeeId",
                principalTable: "EventAttendees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EventAttendeeNotification_EventNotification_NotificationId",
                table: "EventAttendeeNotification",
                column: "NotificationId",
                principalTable: "EventNotification",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EventNotification_Events_EventId",
                table: "EventNotification",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EventNotification_EventOrganizer_EventOrganizerId",
                table: "EventNotification",
                column: "EventOrganizerId",
                principalTable: "EventOrganizer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EventOrganizer_Events_EventId",
                table: "EventOrganizer",
                column: "EventId",
                principalTable: "Events",
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
                name: "FK_MessageBoardItemAttendee_EventAttendees_AttendeeId",
                table: "MessageBoardItemAttendee",
                column: "AttendeeId",
                principalTable: "EventAttendees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MessageBoardItemAttendee_MessageBoardItems_MessageBoardItemId",
                table: "MessageBoardItemAttendee",
                column: "MessageBoardItemId",
                principalTable: "MessageBoardItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserContact_ContactTypes_ContactTypeId",
                table: "UserContact",
                column: "ContactTypeId",
                principalSchema: "dbo",
                principalTable: "ContactTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserContact_User_UserId",
                table: "UserContact",
                column: "UserId",
                principalSchema: "dbo",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

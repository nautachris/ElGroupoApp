using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ElGroupo.Domain.Migrations
{
    public partial class renamedlotsmore : Migration
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
                name: "FK_EventGroups_User_OwnerId",
                table: "EventGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_EventNotification_EventOrganizer_PostedById",
                table: "EventNotification");

            migrationBuilder.DropForeignKey(
                name: "FK_MessageBoardItems_Events_EventId",
                table: "MessageBoardItems");

            migrationBuilder.DropForeignKey(
                name: "FK_MessageBoardItems_User_PostedByUserId",
                table: "MessageBoardItems");

            migrationBuilder.DropForeignKey(
                name: "FK_MessageBoardItemAttendee_EventAttendees_AttendeeId",
                table: "MessageBoardItemAttendee");

            migrationBuilder.DropForeignKey(
                name: "FK_MessageBoardItemAttendee_MessageBoardItems_ItemId",
                table: "MessageBoardItemAttendee");

            migrationBuilder.DropForeignKey(
                name: "FK_UserContact_ContactTypes_ContactTypeId",
                table: "UserContact");

            migrationBuilder.DropForeignKey(
                name: "FK_UserContact_User_UserId",
                table: "UserContact");

            migrationBuilder.DropIndex(
                name: "IX_MessageBoardItemAttendee_ItemId",
                table: "MessageBoardItemAttendee");

            migrationBuilder.DropIndex(
                name: "IX_MessageBoardItems_PostedByUserId",
                table: "MessageBoardItems");

            migrationBuilder.DropIndex(
                name: "IX_EventNotification_PostedById",
                table: "EventNotification");

            migrationBuilder.DropColumn(
                name: "ItemId",
                table: "MessageBoardItemAttendee");

            migrationBuilder.DropColumn(
                name: "PostedByUserId",
                table: "MessageBoardItems");

            migrationBuilder.DropColumn(
                name: "PostedById",
                table: "EventNotification");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "UserContact",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "ContactTypeId",
                table: "UserContact",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "AttendeeId",
                table: "MessageBoardItemAttendee",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AddColumn<long>(
                name: "MessageBoardItemId",
                table: "MessageBoardItemAttendee",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AlterColumn<string>(
                name: "MessageText",
                table: "MessageBoardItems",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "EventId",
                table: "MessageBoardItems",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "MessageBoardItems",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "EventOrganizer",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "EventId",
                table: "EventOrganizer",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "EventId",
                table: "EventNotification",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AddColumn<long>(
                name: "EventOrganizerId",
                table: "EventNotification",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AlterColumn<int>(
                name: "OwnerId",
                table: "EventGroups",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "EventGroups",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "NotificationId",
                table: "EventAttendeeNotification",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "AttendeeId",
                table: "EventAttendeeNotification",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Events",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MessageBoardItemAttendee_MessageBoardItemId",
                table: "MessageBoardItemAttendee",
                column: "MessageBoardItemId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageBoardItems_UserId",
                table: "MessageBoardItems",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_EventNotification_EventOrganizerId",
                table: "EventNotification",
                column: "EventOrganizerId");

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
                name: "FK_EventGroups_User_OwnerId",
                table: "EventGroups",
                column: "OwnerId",
                principalSchema: "dbo",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EventNotification_EventOrganizer_EventOrganizerId",
                table: "EventNotification",
                column: "EventOrganizerId",
                principalTable: "EventOrganizer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MessageBoardItems_Events_EventId",
                table: "MessageBoardItems",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MessageBoardItems_User_UserId",
                table: "MessageBoardItems",
                column: "UserId",
                principalSchema: "dbo",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventAttendeeNotification_EventAttendees_AttendeeId",
                table: "EventAttendeeNotification");

            migrationBuilder.DropForeignKey(
                name: "FK_EventAttendeeNotification_EventNotification_NotificationId",
                table: "EventAttendeeNotification");

            migrationBuilder.DropForeignKey(
                name: "FK_EventGroups_User_OwnerId",
                table: "EventGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_EventNotification_EventOrganizer_EventOrganizerId",
                table: "EventNotification");

            migrationBuilder.DropForeignKey(
                name: "FK_MessageBoardItems_Events_EventId",
                table: "MessageBoardItems");

            migrationBuilder.DropForeignKey(
                name: "FK_MessageBoardItems_User_UserId",
                table: "MessageBoardItems");

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

            migrationBuilder.DropIndex(
                name: "IX_MessageBoardItemAttendee_MessageBoardItemId",
                table: "MessageBoardItemAttendee");

            migrationBuilder.DropIndex(
                name: "IX_MessageBoardItems_UserId",
                table: "MessageBoardItems");

            migrationBuilder.DropIndex(
                name: "IX_EventNotification_EventOrganizerId",
                table: "EventNotification");

            migrationBuilder.DropColumn(
                name: "MessageBoardItemId",
                table: "MessageBoardItemAttendee");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "MessageBoardItems");

            migrationBuilder.DropColumn(
                name: "EventOrganizerId",
                table: "EventNotification");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "Events");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "UserContact",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<long>(
                name: "ContactTypeId",
                table: "UserContact",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<long>(
                name: "AttendeeId",
                table: "MessageBoardItemAttendee",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AddColumn<long>(
                name: "ItemId",
                table: "MessageBoardItemAttendee",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MessageText",
                table: "MessageBoardItems",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<long>(
                name: "EventId",
                table: "MessageBoardItems",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AddColumn<int>(
                name: "PostedByUserId",
                table: "MessageBoardItems",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "EventOrganizer",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<long>(
                name: "EventId",
                table: "EventOrganizer",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<long>(
                name: "EventId",
                table: "EventNotification",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AddColumn<long>(
                name: "PostedById",
                table: "EventNotification",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "OwnerId",
                table: "EventGroups",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "EventGroups",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<long>(
                name: "NotificationId",
                table: "EventAttendeeNotification",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<long>(
                name: "AttendeeId",
                table: "EventAttendeeNotification",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.CreateIndex(
                name: "IX_MessageBoardItemAttendee_ItemId",
                table: "MessageBoardItemAttendee",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageBoardItems_PostedByUserId",
                table: "MessageBoardItems",
                column: "PostedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_EventNotification_PostedById",
                table: "EventNotification",
                column: "PostedById");

            migrationBuilder.AddForeignKey(
                name: "FK_EventAttendeeNotification_EventAttendees_AttendeeId",
                table: "EventAttendeeNotification",
                column: "AttendeeId",
                principalTable: "EventAttendees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EventAttendeeNotification_EventNotification_NotificationId",
                table: "EventAttendeeNotification",
                column: "NotificationId",
                principalTable: "EventNotification",
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
                name: "FK_EventNotification_EventOrganizer_PostedById",
                table: "EventNotification",
                column: "PostedById",
                principalTable: "EventOrganizer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MessageBoardItems_Events_EventId",
                table: "MessageBoardItems",
                column: "EventId",
                principalTable: "Events",
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
                name: "FK_MessageBoardItemAttendee_EventAttendees_AttendeeId",
                table: "MessageBoardItemAttendee",
                column: "AttendeeId",
                principalTable: "EventAttendees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MessageBoardItemAttendee_MessageBoardItems_ItemId",
                table: "MessageBoardItemAttendee",
                column: "ItemId",
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
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserContact_User_UserId",
                table: "UserContact",
                column: "UserId",
                principalSchema: "dbo",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

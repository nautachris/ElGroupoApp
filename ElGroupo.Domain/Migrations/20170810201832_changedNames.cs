using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ElGroupo.Domain.Migrations
{
    public partial class changedNames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContactGroupUser_ElGroupoUser_ElGroupoUserId",
                schema: "dbo",
                table: "ContactGroupUser");

            migrationBuilder.DropForeignKey(
                name: "FK_ContactGroupUser_ContactGroups_GroupId",
                schema: "dbo",
                table: "ContactGroupUser");

            migrationBuilder.DropTable(
                name: "ElGroupoEventAttendeeNotification");

            migrationBuilder.DropTable(
                name: "ElGroupoMessageBoardItemAttendee");

            migrationBuilder.DropTable(
                name: "ElGroupoEventNotification");

            migrationBuilder.DropTable(
                name: "ElGroupoEventOrganizer");

            migrationBuilder.DropIndex(
                name: "IX_ContactGroupUser_ElGroupoUserId",
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

            migrationBuilder.AddColumn<int>(
                name: "ElGroupoUserId1",
                schema: "dbo",
                table: "ContactGroupUser",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EventOrganizer",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: false),
                    EventId = table.Column<long>(nullable: true),
                    Owner = table.Column<bool>(nullable: false),
                    UserCreated = table.Column<string>(nullable: true),
                    UserId = table.Column<int>(nullable: true),
                    UserUpdated = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventOrganizer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventOrganizer_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventOrganizer_ElGroupoUser_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "ElGroupoUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MessageBoardItemAttendee",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    AttendeeId = table.Column<long>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: false),
                    ItemId = table.Column<long>(nullable: true),
                    UserCreated = table.Column<string>(nullable: true),
                    UserUpdated = table.Column<string>(nullable: true),
                    Viewed = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageBoardItemAttendee", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MessageBoardItemAttendee_EventAttendees_AttendeeId",
                        column: x => x.AttendeeId,
                        principalTable: "EventAttendees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MessageBoardItemAttendee_MessageBoardItems_ItemId",
                        column: x => x.ItemId,
                        principalTable: "MessageBoardItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EventNotification",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: false),
                    EventId = table.Column<long>(nullable: true),
                    Importance = table.Column<int>(nullable: false),
                    MessageText = table.Column<string>(nullable: true),
                    PostedById = table.Column<long>(nullable: true),
                    PostedDate = table.Column<DateTime>(nullable: false),
                    UserCreated = table.Column<string>(nullable: true),
                    UserUpdated = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventNotification", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventNotification_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventNotification_EventOrganizer_PostedById",
                        column: x => x.PostedById,
                        principalTable: "EventOrganizer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EventAttendeeNotification",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    AttendeeId = table.Column<long>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: false),
                    NotificationId = table.Column<long>(nullable: true),
                    UserCreated = table.Column<string>(nullable: true),
                    UserUpdated = table.Column<string>(nullable: true),
                    Viewed = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventAttendeeNotification", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventAttendeeNotification_EventAttendees_AttendeeId",
                        column: x => x.AttendeeId,
                        principalTable: "EventAttendees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventAttendeeNotification_EventNotification_NotificationId",
                        column: x => x.NotificationId,
                        principalTable: "EventNotification",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContactGroupUser_ElGroupoUserId1",
                schema: "dbo",
                table: "ContactGroupUser",
                column: "ElGroupoUserId1");

            migrationBuilder.CreateIndex(
                name: "IX_EventAttendeeNotification_AttendeeId",
                table: "EventAttendeeNotification",
                column: "AttendeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EventAttendeeNotification_NotificationId",
                table: "EventAttendeeNotification",
                column: "NotificationId");

            migrationBuilder.CreateIndex(
                name: "IX_EventNotification_EventId",
                table: "EventNotification",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_EventNotification_PostedById",
                table: "EventNotification",
                column: "PostedById");

            migrationBuilder.CreateIndex(
                name: "IX_EventOrganizer_EventId",
                table: "EventOrganizer",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_EventOrganizer_UserId",
                table: "EventOrganizer",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageBoardItemAttendee_AttendeeId",
                table: "MessageBoardItemAttendee",
                column: "AttendeeId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageBoardItemAttendee_ItemId",
                table: "MessageBoardItemAttendee",
                column: "ItemId");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContactGroupUser_ElGroupoUser_ElGroupoUserId1",
                schema: "dbo",
                table: "ContactGroupUser");

            migrationBuilder.DropForeignKey(
                name: "FK_ContactGroupUser_ContactGroups_GroupId",
                schema: "dbo",
                table: "ContactGroupUser");

            migrationBuilder.DropTable(
                name: "EventAttendeeNotification");

            migrationBuilder.DropTable(
                name: "MessageBoardItemAttendee");

            migrationBuilder.DropTable(
                name: "EventNotification");

            migrationBuilder.DropTable(
                name: "EventOrganizer");

            migrationBuilder.DropIndex(
                name: "IX_ContactGroupUser_ElGroupoUserId1",
                schema: "dbo",
                table: "ContactGroupUser");

            migrationBuilder.DropColumn(
                name: "ElGroupoUserId1",
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

            migrationBuilder.CreateTable(
                name: "ElGroupoEventOrganizer",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: false),
                    EventId = table.Column<long>(nullable: true),
                    Owner = table.Column<bool>(nullable: false),
                    UserCreated = table.Column<string>(nullable: true),
                    UserId = table.Column<int>(nullable: true),
                    UserUpdated = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElGroupoEventOrganizer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ElGroupoEventOrganizer_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ElGroupoEventOrganizer_ElGroupoUser_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "ElGroupoUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ElGroupoMessageBoardItemAttendee",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    AttendeeId = table.Column<long>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: false),
                    ItemId = table.Column<long>(nullable: true),
                    UserCreated = table.Column<string>(nullable: true),
                    UserUpdated = table.Column<string>(nullable: true),
                    Viewed = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElGroupoMessageBoardItemAttendee", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ElGroupoMessageBoardItemAttendee_EventAttendees_AttendeeId",
                        column: x => x.AttendeeId,
                        principalTable: "EventAttendees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ElGroupoMessageBoardItemAttendee_MessageBoardItems_ItemId",
                        column: x => x.ItemId,
                        principalTable: "MessageBoardItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ElGroupoEventNotification",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: false),
                    EventId = table.Column<long>(nullable: true),
                    Importance = table.Column<int>(nullable: false),
                    MessageText = table.Column<string>(nullable: true),
                    PostedById = table.Column<long>(nullable: true),
                    PostedDate = table.Column<DateTime>(nullable: false),
                    UserCreated = table.Column<string>(nullable: true),
                    UserUpdated = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElGroupoEventNotification", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ElGroupoEventNotification_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ElGroupoEventNotification_ElGroupoEventOrganizer_PostedById",
                        column: x => x.PostedById,
                        principalTable: "ElGroupoEventOrganizer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ElGroupoEventAttendeeNotification",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    AttendeeId = table.Column<long>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: false),
                    NotificationId = table.Column<long>(nullable: true),
                    UserCreated = table.Column<string>(nullable: true),
                    UserUpdated = table.Column<string>(nullable: true),
                    Viewed = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElGroupoEventAttendeeNotification", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ElGroupoEventAttendeeNotification_EventAttendees_AttendeeId",
                        column: x => x.AttendeeId,
                        principalTable: "EventAttendees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ElGroupoEventAttendeeNotification_ElGroupoEventNotification_NotificationId",
                        column: x => x.NotificationId,
                        principalTable: "ElGroupoEventNotification",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContactGroupUser_ElGroupoUserId",
                schema: "dbo",
                table: "ContactGroupUser",
                column: "ElGroupoUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ElGroupoEventAttendeeNotification_AttendeeId",
                table: "ElGroupoEventAttendeeNotification",
                column: "AttendeeId");

            migrationBuilder.CreateIndex(
                name: "IX_ElGroupoEventAttendeeNotification_NotificationId",
                table: "ElGroupoEventAttendeeNotification",
                column: "NotificationId");

            migrationBuilder.CreateIndex(
                name: "IX_ElGroupoEventNotification_EventId",
                table: "ElGroupoEventNotification",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_ElGroupoEventNotification_PostedById",
                table: "ElGroupoEventNotification",
                column: "PostedById");

            migrationBuilder.CreateIndex(
                name: "IX_ElGroupoEventOrganizer_EventId",
                table: "ElGroupoEventOrganizer",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_ElGroupoEventOrganizer_UserId",
                table: "ElGroupoEventOrganizer",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ElGroupoMessageBoardItemAttendee_AttendeeId",
                table: "ElGroupoMessageBoardItemAttendee",
                column: "AttendeeId");

            migrationBuilder.CreateIndex(
                name: "IX_ElGroupoMessageBoardItemAttendee_ItemId",
                table: "ElGroupoMessageBoardItemAttendee",
                column: "ItemId");

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
    }
}

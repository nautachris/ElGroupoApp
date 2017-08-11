using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ElGroupo.Domain.Migrations
{
    public partial class changedEventToOrganizerRelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ElGroupoEvent_ElGroupoEventGroup_GroupId",
                table: "ElGroupoEvent");

            migrationBuilder.DropForeignKey(
                name: "FK_ElGroupoEvent_ElGroupoUser_OrganizerId",
                table: "ElGroupoEvent");

            migrationBuilder.DropForeignKey(
                name: "FK_ElGroupoEventAttendee_ElGroupoEvent_EventId",
                table: "ElGroupoEventAttendee");

            migrationBuilder.DropForeignKey(
                name: "FK_ElGroupoEventAttendee_ElGroupoUser_UserId",
                table: "ElGroupoEventAttendee");

            migrationBuilder.DropForeignKey(
                name: "FK_ElGroupoEventGroup_ElGroupoUser_OwnerId",
                table: "ElGroupoEventGroup");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ElGroupoEventGroup",
                table: "ElGroupoEventGroup");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ElGroupoEventAttendee",
                table: "ElGroupoEventAttendee");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ElGroupoEvent",
                table: "ElGroupoEvent");

            migrationBuilder.DropIndex(
                name: "IX_ElGroupoEvent_OrganizerId",
                table: "ElGroupoEvent");

            migrationBuilder.DropColumn(
                name: "OrganizerId",
                table: "ElGroupoEvent");

            migrationBuilder.RenameTable(
                name: "ElGroupoEventGroup",
                newName: "EventGroups");

            migrationBuilder.RenameTable(
                name: "ElGroupoEventAttendee",
                newName: "EventAttendees");

            migrationBuilder.RenameTable(
                name: "ElGroupoEvent",
                newName: "Events");

            migrationBuilder.RenameIndex(
                name: "IX_ElGroupoEventGroup_OwnerId",
                table: "EventGroups",
                newName: "IX_EventGroups_OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_ElGroupoEventAttendee_UserId",
                table: "EventAttendees",
                newName: "IX_EventAttendees_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_ElGroupoEventAttendee_EventId",
                table: "EventAttendees",
                newName: "IX_EventAttendees_EventId");

            migrationBuilder.RenameIndex(
                name: "IX_ElGroupoEvent_GroupId",
                table: "Events",
                newName: "IX_Events_GroupId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventGroups",
                table: "EventGroups",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventAttendees",
                table: "EventAttendees",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Events",
                table: "Events",
                column: "Id");

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
                name: "MessageBoardItems",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: false),
                    EventId = table.Column<long>(nullable: true),
                    MessageText = table.Column<string>(nullable: true),
                    PostedByUserId = table.Column<int>(nullable: true),
                    PostedDate = table.Column<DateTime>(nullable: false),
                    UserCreated = table.Column<string>(nullable: true),
                    UserUpdated = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageBoardItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MessageBoardItems_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MessageBoardItems_ElGroupoUser_PostedByUserId",
                        column: x => x.PostedByUserId,
                        principalSchema: "dbo",
                        principalTable: "ElGroupoUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ElGroupoEventOrganizer_EventId",
                table: "ElGroupoEventOrganizer",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_ElGroupoEventOrganizer_UserId",
                table: "ElGroupoEventOrganizer",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageBoardItems_EventId",
                table: "MessageBoardItems",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageBoardItems_PostedByUserId",
                table: "MessageBoardItems",
                column: "PostedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_EventGroups_GroupId",
                table: "Events",
                column: "GroupId",
                principalTable: "EventGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EventAttendees_Events_EventId",
                table: "EventAttendees",
                column: "EventId",
                principalTable: "Events",
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_EventGroups_GroupId",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_EventAttendees_Events_EventId",
                table: "EventAttendees");

            migrationBuilder.DropForeignKey(
                name: "FK_EventAttendees_ElGroupoUser_UserId",
                table: "EventAttendees");

            migrationBuilder.DropForeignKey(
                name: "FK_EventGroups_ElGroupoUser_OwnerId",
                table: "EventGroups");

            migrationBuilder.DropTable(
                name: "ElGroupoEventOrganizer");

            migrationBuilder.DropTable(
                name: "MessageBoardItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EventGroups",
                table: "EventGroups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EventAttendees",
                table: "EventAttendees");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Events",
                table: "Events");

            migrationBuilder.RenameTable(
                name: "EventGroups",
                newName: "ElGroupoEventGroup");

            migrationBuilder.RenameTable(
                name: "EventAttendees",
                newName: "ElGroupoEventAttendee");

            migrationBuilder.RenameTable(
                name: "Events",
                newName: "ElGroupoEvent");

            migrationBuilder.RenameIndex(
                name: "IX_EventGroups_OwnerId",
                table: "ElGroupoEventGroup",
                newName: "IX_ElGroupoEventGroup_OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_EventAttendees_UserId",
                table: "ElGroupoEventAttendee",
                newName: "IX_ElGroupoEventAttendee_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_EventAttendees_EventId",
                table: "ElGroupoEventAttendee",
                newName: "IX_ElGroupoEventAttendee_EventId");

            migrationBuilder.RenameIndex(
                name: "IX_Events_GroupId",
                table: "ElGroupoEvent",
                newName: "IX_ElGroupoEvent_GroupId");

            migrationBuilder.AddColumn<int>(
                name: "OrganizerId",
                table: "ElGroupoEvent",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ElGroupoEventGroup",
                table: "ElGroupoEventGroup",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ElGroupoEventAttendee",
                table: "ElGroupoEventAttendee",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ElGroupoEvent",
                table: "ElGroupoEvent",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ElGroupoEvent_OrganizerId",
                table: "ElGroupoEvent",
                column: "OrganizerId");

            migrationBuilder.AddForeignKey(
                name: "FK_ElGroupoEvent_ElGroupoEventGroup_GroupId",
                table: "ElGroupoEvent",
                column: "GroupId",
                principalTable: "ElGroupoEventGroup",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ElGroupoEvent_ElGroupoUser_OrganizerId",
                table: "ElGroupoEvent",
                column: "OrganizerId",
                principalSchema: "dbo",
                principalTable: "ElGroupoUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ElGroupoEventAttendee_ElGroupoEvent_EventId",
                table: "ElGroupoEventAttendee",
                column: "EventId",
                principalTable: "ElGroupoEvent",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ElGroupoEventAttendee_ElGroupoUser_UserId",
                table: "ElGroupoEventAttendee",
                column: "UserId",
                principalSchema: "dbo",
                principalTable: "ElGroupoUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ElGroupoEventGroup_ElGroupoUser_OwnerId",
                table: "ElGroupoEventGroup",
                column: "OwnerId",
                principalSchema: "dbo",
                principalTable: "ElGroupoUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

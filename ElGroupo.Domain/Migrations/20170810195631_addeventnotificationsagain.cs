using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ElGroupo.Domain.Migrations
{
    public partial class addeventnotificationsagain : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ElGroupoEventAttendeeNotification_Events_EventId",
                table: "ElGroupoEventAttendeeNotification");

            migrationBuilder.RenameColumn(
                name: "EventId",
                table: "ElGroupoEventAttendeeNotification",
                newName: "NotificationId");

            migrationBuilder.RenameIndex(
                name: "IX_ElGroupoEventAttendeeNotification_EventId",
                table: "ElGroupoEventAttendeeNotification",
                newName: "IX_ElGroupoEventAttendeeNotification_NotificationId");

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

            migrationBuilder.CreateIndex(
                name: "IX_ElGroupoEventNotification_EventId",
                table: "ElGroupoEventNotification",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_ElGroupoEventNotification_PostedById",
                table: "ElGroupoEventNotification",
                column: "PostedById");

            migrationBuilder.AddForeignKey(
                name: "FK_ElGroupoEventAttendeeNotification_ElGroupoEventNotification_NotificationId",
                table: "ElGroupoEventAttendeeNotification",
                column: "NotificationId",
                principalTable: "ElGroupoEventNotification",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ElGroupoEventAttendeeNotification_ElGroupoEventNotification_NotificationId",
                table: "ElGroupoEventAttendeeNotification");

            migrationBuilder.DropTable(
                name: "ElGroupoEventNotification");

            migrationBuilder.RenameColumn(
                name: "NotificationId",
                table: "ElGroupoEventAttendeeNotification",
                newName: "EventId");

            migrationBuilder.RenameIndex(
                name: "IX_ElGroupoEventAttendeeNotification_NotificationId",
                table: "ElGroupoEventAttendeeNotification",
                newName: "IX_ElGroupoEventAttendeeNotification_EventId");

            migrationBuilder.AddForeignKey(
                name: "FK_ElGroupoEventAttendeeNotification_Events_EventId",
                table: "ElGroupoEventAttendeeNotification",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

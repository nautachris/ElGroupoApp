using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ElGroupo.Domain.Migrations
{
    public partial class addeventnotifications : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ElGroupoEventAttendeeNotification",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    AttendeeId = table.Column<long>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: false),
                    EventId = table.Column<long>(nullable: true),
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
                        name: "FK_ElGroupoEventAttendeeNotification_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ElGroupoEventAttendeeNotification_AttendeeId",
                table: "ElGroupoEventAttendeeNotification",
                column: "AttendeeId");

            migrationBuilder.CreateIndex(
                name: "IX_ElGroupoEventAttendeeNotification_EventId",
                table: "ElGroupoEventAttendeeNotification",
                column: "EventId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ElGroupoEventAttendeeNotification");
        }
    }
}

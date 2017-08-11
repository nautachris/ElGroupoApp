using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ElGroupo.Domain.Migrations
{
    public partial class addEventToAttendeeRelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ElGroupoEventAttendee",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    AllowEventUpdates = table.Column<bool>(nullable: true),
                    CheckInCoordinateX = table.Column<double>(nullable: true),
                    CheckInCoordinateY = table.Column<double>(nullable: true),
                    CheckInTime = table.Column<DateTime>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: false),
                    EventId = table.Column<long>(nullable: true),
                    ResponseDate = table.Column<DateTime>(nullable: true),
                    ResponseStatus = table.Column<int>(nullable: false),
                    ResponseText = table.Column<string>(nullable: true),
                    UserCreated = table.Column<string>(nullable: true),
                    UserId = table.Column<int>(nullable: true),
                    UserUpdated = table.Column<string>(nullable: true),
                    Viewed = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElGroupoEventAttendee", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ElGroupoEventAttendee_ElGroupoEvent_EventId",
                        column: x => x.EventId,
                        principalTable: "ElGroupoEvent",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ElGroupoEventAttendee_ElGroupoUser_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "ElGroupoUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ElGroupoEventAttendee_EventId",
                table: "ElGroupoEventAttendee",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_ElGroupoEventAttendee_UserId",
                table: "ElGroupoEventAttendee",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ElGroupoEventAttendee");
        }
    }
}

using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ElGroupo.Domain.Migrations
{
    public partial class addMessageBoardItemUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateIndex(
                name: "IX_ElGroupoMessageBoardItemAttendee_AttendeeId",
                table: "ElGroupoMessageBoardItemAttendee",
                column: "AttendeeId");

            migrationBuilder.CreateIndex(
                name: "IX_ElGroupoMessageBoardItemAttendee_ItemId",
                table: "ElGroupoMessageBoardItemAttendee",
                column: "ItemId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ElGroupoMessageBoardItemAttendee");
        }
    }
}

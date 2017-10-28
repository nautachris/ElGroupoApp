using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ElGroupo.Domain.Migrations
{
    public partial class eventrecurrence : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_EventGroups_GroupId",
                table: "Events");

            migrationBuilder.DropTable(
                name: "EventGroups");

            migrationBuilder.DropIndex(
                name: "IX_Events_GroupId",
                table: "Events");

            migrationBuilder.AddColumn<long>(
                name: "RecurrenceId",
                table: "Events",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RecurringEvent",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: false),
                    Pattern = table.Column<int>(nullable: false),
                    RecurrenceDays = table.Column<int>(nullable: false),
                    RecurrenceInterval = table.Column<int>(nullable: false),
                    RecurrenceLimit = table.Column<int>(nullable: false),
                    UserCreated = table.Column<string>(nullable: true),
                    UserUpdated = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecurringEvent", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Events_RecurrenceId",
                table: "Events",
                column: "RecurrenceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_RecurringEvent_RecurrenceId",
                table: "Events",
                column: "RecurrenceId",
                principalTable: "RecurringEvent",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_RecurringEvent_RecurrenceId",
                table: "Events");

            migrationBuilder.DropTable(
                name: "RecurringEvent");

            migrationBuilder.DropIndex(
                name: "IX_Events_RecurrenceId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "RecurrenceId",
                table: "Events");

            migrationBuilder.CreateTable(
                name: "EventGroups",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    OwnerId = table.Column<int>(nullable: false),
                    OwnerId1 = table.Column<long>(nullable: false),
                    UserCreated = table.Column<string>(nullable: true),
                    UserUpdated = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventGroups_User_OwnerId1",
                        column: x => x.OwnerId1,
                        principalSchema: "dbo",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Events_GroupId",
                table: "Events",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_EventGroups_OwnerId1",
                table: "EventGroups",
                column: "OwnerId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_EventGroups_GroupId",
                table: "Events",
                column: "GroupId",
                principalTable: "EventGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

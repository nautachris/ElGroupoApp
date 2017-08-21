using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ElGroupo.Domain.Migrations
{
    public partial class unregisteredeventattendee : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UnregisteredEventAttendees",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: false),
                    Email = table.Column<string>(nullable: true),
                    EventId = table.Column<long>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    RegisterToken = table.Column<Guid>(nullable: false),
                    UserCreated = table.Column<string>(nullable: true),
                    UserUpdated = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnregisteredEventAttendees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UnregisteredEventAttendees_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UnregisteredEventAttendees_EventId",
                schema: "dbo",
                table: "UnregisteredEventAttendees",
                column: "EventId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UnregisteredEventAttendees",
                schema: "dbo");
        }
    }
}

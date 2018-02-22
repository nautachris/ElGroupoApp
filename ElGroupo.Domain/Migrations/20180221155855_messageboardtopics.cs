using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ElGroupo.Domain.Migrations
{
    public partial class messageboardtopics : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MessageBoardItems_Events_EventId",
                table: "MessageBoardItems");

            migrationBuilder.DropColumn(
                name: "Subject",
                table: "MessageBoardItems");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "MessageBoardItems");

            migrationBuilder.RenameColumn(
                name: "EventId",
                table: "MessageBoardItems",
                newName: "TopicId");

            migrationBuilder.RenameIndex(
                name: "IX_MessageBoardItems_EventId",
                table: "MessageBoardItems",
                newName: "IX_MessageBoardItems_TopicId");

            migrationBuilder.CreateTable(
                name: "MessageBoardTopics",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: false),
                    EventId = table.Column<long>(nullable: true),
                    StartedById = table.Column<long>(nullable: true),
                    StartedDate = table.Column<DateTime>(nullable: false),
                    Subject = table.Column<string>(nullable: true),
                    UserCreated = table.Column<string>(nullable: true),
                    UserUpdated = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageBoardTopics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MessageBoardTopics_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MessageBoardTopics_User_StartedById",
                        column: x => x.StartedById,
                        principalSchema: "dbo",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MessageBoardTopics_EventId",
                table: "MessageBoardTopics",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageBoardTopics_StartedById",
                table: "MessageBoardTopics",
                column: "StartedById");

            migrationBuilder.AddForeignKey(
                name: "FK_MessageBoardItems_MessageBoardTopics_TopicId",
                table: "MessageBoardItems",
                column: "TopicId",
                principalTable: "MessageBoardTopics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MessageBoardItems_MessageBoardTopics_TopicId",
                table: "MessageBoardItems");

            migrationBuilder.DropTable(
                name: "MessageBoardTopics");

            migrationBuilder.RenameColumn(
                name: "TopicId",
                table: "MessageBoardItems",
                newName: "EventId");

            migrationBuilder.RenameIndex(
                name: "IX_MessageBoardItems_TopicId",
                table: "MessageBoardItems",
                newName: "IX_MessageBoardItems_EventId");

            migrationBuilder.AddColumn<string>(
                name: "Subject",
                table: "MessageBoardItems",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "MessageBoardItems",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_MessageBoardItems_Events_EventId",
                table: "MessageBoardItems",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

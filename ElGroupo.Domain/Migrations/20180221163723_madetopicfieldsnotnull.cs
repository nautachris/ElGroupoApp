using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ElGroupo.Domain.Migrations
{
    public partial class madetopicfieldsnotnull : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MessageBoardTopics_Events_EventId",
                table: "MessageBoardTopics");

            migrationBuilder.DropForeignKey(
                name: "FK_MessageBoardTopics_User_StartedById",
                table: "MessageBoardTopics");

            migrationBuilder.AlterColumn<long>(
                name: "StartedById",
                table: "MessageBoardTopics",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "EventId",
                table: "MessageBoardTopics",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_MessageBoardTopics_Events_EventId",
                table: "MessageBoardTopics",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MessageBoardTopics_User_StartedById",
                table: "MessageBoardTopics",
                column: "StartedById",
                principalSchema: "dbo",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MessageBoardTopics_Events_EventId",
                table: "MessageBoardTopics");

            migrationBuilder.DropForeignKey(
                name: "FK_MessageBoardTopics_User_StartedById",
                table: "MessageBoardTopics");

            migrationBuilder.AlterColumn<long>(
                name: "StartedById",
                table: "MessageBoardTopics",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<long>(
                name: "EventId",
                table: "MessageBoardTopics",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AddForeignKey(
                name: "FK_MessageBoardTopics_Events_EventId",
                table: "MessageBoardTopics",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MessageBoardTopics_User_StartedById",
                table: "MessageBoardTopics",
                column: "StartedById",
                principalSchema: "dbo",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

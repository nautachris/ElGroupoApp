using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ElGroupo.Domain.Migrations
{
    public partial class addMessageSubject : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Subject",
                table: "MessageBoardItems",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Subject",
                table: "EventNotifications",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Subject",
                table: "MessageBoardItems");

            migrationBuilder.DropColumn(
                name: "Subject",
                table: "EventNotifications");
        }
    }
}

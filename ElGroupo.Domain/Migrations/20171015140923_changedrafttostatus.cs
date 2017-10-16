using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ElGroupo.Domain.Migrations
{
    public partial class changedrafttostatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SavedAsDraft",
                table: "Events");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Events",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Events");

            migrationBuilder.AddColumn<bool>(
                name: "SavedAsDraft",
                table: "Events",
                nullable: false,
                defaultValue: false);
        }
    }
}

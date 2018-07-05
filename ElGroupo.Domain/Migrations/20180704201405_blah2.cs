using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ElGroupo.Domain.Migrations
{
    public partial class blah2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ItemDescriptionColumnHeader",
                table: "RecordSubCategories",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ItemValueColumnHeader",
                table: "RecordSubCategories",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "RecordElements",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ItemDescriptionColumnHeader",
                table: "RecordSubCategories");

            migrationBuilder.DropColumn(
                name: "ItemValueColumnHeader",
                table: "RecordSubCategories");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "RecordElements");
        }
    }
}

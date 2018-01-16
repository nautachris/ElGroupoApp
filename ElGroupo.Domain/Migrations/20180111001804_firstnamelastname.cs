using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ElGroupo.Domain.Migrations
{
    public partial class firstnamelastname : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                schema: "dbo",
                table: "User",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                schema: "dbo",
                table: "User",
                maxLength: 100,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstName",
                schema: "dbo",
                table: "User");

            migrationBuilder.DropColumn(
                name: "LastName",
                schema: "dbo",
                table: "User");
        }
    }
}

using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ElGroupo.Domain.Migrations
{
    public partial class changeEventAddressFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Events");

            migrationBuilder.AddColumn<string>(
                name: "Address1",
                table: "Events",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address2",
                table: "Events",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Events",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "Events",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Zip",
                table: "Events",
                maxLength: 10,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address1",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "Address2",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "City",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "State",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "Zip",
                table: "Events");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Events",
                nullable: true);
        }
    }
}

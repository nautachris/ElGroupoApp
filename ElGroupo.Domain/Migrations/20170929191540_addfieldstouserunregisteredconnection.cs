using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ElGroupo.Domain.Migrations
{
    public partial class addfieldstouserunregisteredconnection : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RegisterToken",
                table: "UnregisteredUserConnections");

            migrationBuilder.RenameColumn(
                name: "Phone",
                table: "UnregisteredUserConnections",
                newName: "Phone2Value");

            migrationBuilder.AddColumn<string>(
                name: "Phone1Type",
                table: "UnregisteredUserConnections",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone1Value",
                table: "UnregisteredUserConnections",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone2Type",
                table: "UnregisteredUserConnections",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Phone1Type",
                table: "UnregisteredUserConnections");

            migrationBuilder.DropColumn(
                name: "Phone1Value",
                table: "UnregisteredUserConnections");

            migrationBuilder.DropColumn(
                name: "Phone2Type",
                table: "UnregisteredUserConnections");

            migrationBuilder.RenameColumn(
                name: "Phone2Value",
                table: "UnregisteredUserConnections",
                newName: "Phone");

            migrationBuilder.AddColumn<Guid>(
                name: "RegisterToken",
                table: "UnregisteredUserConnections",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}

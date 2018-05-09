using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ElGroupo.Domain.Migrations
{
    public partial class activityrequiresactivitygroup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Activities_ActivityGroups_ActivityGroupId",
                table: "Activities");

            migrationBuilder.AlterColumn<long>(
                name: "ActivityGroupId",
                table: "Activities",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Activities_ActivityGroups_ActivityGroupId",
                table: "Activities",
                column: "ActivityGroupId",
                principalTable: "ActivityGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Activities_ActivityGroups_ActivityGroupId",
                table: "Activities");

            migrationBuilder.AlterColumn<long>(
                name: "ActivityGroupId",
                table: "Activities",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AddForeignKey(
                name: "FK_Activities_ActivityGroups_ActivityGroupId",
                table: "Activities",
                column: "ActivityGroupId",
                principalTable: "ActivityGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

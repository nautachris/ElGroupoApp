using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ElGroupo.Domain.Migrations
{
    public partial class changedmoreschema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserActivities_ActivityAttendanceTypes_AttendanceTypeId",
                table: "UserActivities");

            migrationBuilder.DropTable(
                name: "ActivityAttendanceTypes");

            migrationBuilder.DropIndex(
                name: "IX_UserActivities_AttendanceTypeId",
                table: "UserActivities");

            migrationBuilder.AddColumn<bool>(
                name: "IsPresenting",
                table: "UserActivities",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "UserActivities",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PresentationName",
                table: "UserActivities",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Active",
                table: "CreditTypeCategories",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Activities",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPresenting",
                table: "UserActivities");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "UserActivities");

            migrationBuilder.DropColumn(
                name: "PresentationName",
                table: "UserActivities");

            migrationBuilder.DropColumn(
                name: "Active",
                table: "CreditTypeCategories");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Activities");

            migrationBuilder.CreateTable(
                name: "ActivityAttendanceTypes",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(maxLength: 255, nullable: false),
                    UserCreated = table.Column<string>(nullable: true),
                    UserUpdated = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityAttendanceTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserActivities_AttendanceTypeId",
                table: "UserActivities",
                column: "AttendanceTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserActivities_ActivityAttendanceTypes_AttendanceTypeId",
                table: "UserActivities",
                column: "AttendanceTypeId",
                principalTable: "ActivityAttendanceTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ElGroupo.Domain.Migrations
{
    public partial class useractivitydepartmentoptional : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActivityGroups_Departments_DepartmentId",
                table: "ActivityGroups");

            migrationBuilder.AlterColumn<long>(
                name: "DepartmentId",
                table: "ActivityGroups",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityGroups_Departments_DepartmentId",
                table: "ActivityGroups",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActivityGroups_Departments_DepartmentId",
                table: "ActivityGroups");

            migrationBuilder.AlterColumn<long>(
                name: "DepartmentId",
                table: "ActivityGroups",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityGroups_Departments_DepartmentId",
                table: "ActivityGroups",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

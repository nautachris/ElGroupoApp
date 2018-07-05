using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ElGroupo.Domain.Migrations
{
    public partial class recordelementdatatypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataType",
                table: "RecordElements");

            migrationBuilder.AddColumn<long>(
                name: "DataTypeId",
                table: "RecordElements",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "LookupTableId",
                table: "RecordElements",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RecordElementDataTypes",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    UserCreated = table.Column<string>(nullable: true),
                    UserUpdated = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecordElementDataTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RecordElementLookupTables",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    TableName = table.Column<string>(nullable: true),
                    UserCreated = table.Column<string>(nullable: true),
                    UserUpdated = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecordElementLookupTables", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RecordElements_DataTypeId",
                table: "RecordElements",
                column: "DataTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_RecordElements_LookupTableId",
                table: "RecordElements",
                column: "LookupTableId");

            migrationBuilder.AddForeignKey(
                name: "FK_RecordElements_RecordElementDataTypes_DataTypeId",
                table: "RecordElements",
                column: "DataTypeId",
                principalTable: "RecordElementDataTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RecordElements_RecordElementLookupTables_LookupTableId",
                table: "RecordElements",
                column: "LookupTableId",
                principalTable: "RecordElementLookupTables",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RecordElements_RecordElementDataTypes_DataTypeId",
                table: "RecordElements");

            migrationBuilder.DropForeignKey(
                name: "FK_RecordElements_RecordElementLookupTables_LookupTableId",
                table: "RecordElements");

            migrationBuilder.DropTable(
                name: "RecordElementDataTypes");

            migrationBuilder.DropTable(
                name: "RecordElementLookupTables");

            migrationBuilder.DropIndex(
                name: "IX_RecordElements_DataTypeId",
                table: "RecordElements");

            migrationBuilder.DropIndex(
                name: "IX_RecordElements_LookupTableId",
                table: "RecordElements");

            migrationBuilder.DropColumn(
                name: "DataTypeId",
                table: "RecordElements");

            migrationBuilder.DropColumn(
                name: "LookupTableId",
                table: "RecordElements");

            migrationBuilder.AddColumn<string>(
                name: "DataType",
                table: "RecordElements",
                nullable: true);
        }
    }
}

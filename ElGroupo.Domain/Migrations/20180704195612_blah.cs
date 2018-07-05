using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ElGroupo.Domain.Migrations
{
    public partial class blah : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Visible",
                table: "RecordItemUsers",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<long>(
                name: "UserId",
                table: "RecordItems",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "InputTypeId",
                table: "RecordElements",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "LookupTableFieldTypeId",
                table: "RecordElements",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ItemDescriptionColumnHeader",
                table: "RecordCategories",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ItemValueColumnHeader",
                table: "RecordCategories",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RecordDefaultElements",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    CategoryId = table.Column<long>(nullable: true),
                    ElementId = table.Column<long>(nullable: false),
                    SubCategoryId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecordDefaultElements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecordDefaultElements_RecordCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "RecordCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecordDefaultElements_RecordElements_ElementId",
                        column: x => x.ElementId,
                        principalTable: "RecordElements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecordDefaultElements_RecordSubCategories_SubCategoryId",
                        column: x => x.SubCategoryId,
                        principalTable: "RecordSubCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RecordElementInputTypes",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecordElementInputTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RecordElementLookupTableFieldTypes",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecordElementLookupTableFieldTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RecordElementDataTypeInputTypes",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    DataTypeId = table.Column<long>(nullable: false),
                    InputTypeId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecordElementDataTypeInputTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecordElementDataTypeInputTypes_RecordElementDataTypes_DataTypeId",
                        column: x => x.DataTypeId,
                        principalTable: "RecordElementDataTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecordElementDataTypeInputTypes_RecordElementInputTypes_InputTypeId",
                        column: x => x.InputTypeId,
                        principalTable: "RecordElementInputTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RecordItems_UserId",
                table: "RecordItems",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RecordElements_InputTypeId",
                table: "RecordElements",
                column: "InputTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_RecordElements_LookupTableFieldTypeId",
                table: "RecordElements",
                column: "LookupTableFieldTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_RecordDefaultElements_CategoryId",
                table: "RecordDefaultElements",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_RecordDefaultElements_ElementId",
                table: "RecordDefaultElements",
                column: "ElementId");

            migrationBuilder.CreateIndex(
                name: "IX_RecordDefaultElements_SubCategoryId",
                table: "RecordDefaultElements",
                column: "SubCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_RecordElementDataTypeInputTypes_DataTypeId",
                table: "RecordElementDataTypeInputTypes",
                column: "DataTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_RecordElementDataTypeInputTypes_InputTypeId",
                table: "RecordElementDataTypeInputTypes",
                column: "InputTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_RecordElements_RecordElementInputTypes_InputTypeId",
                table: "RecordElements",
                column: "InputTypeId",
                principalTable: "RecordElementInputTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RecordElements_RecordElementLookupTableFieldTypes_LookupTableFieldTypeId",
                table: "RecordElements",
                column: "LookupTableFieldTypeId",
                principalTable: "RecordElementLookupTableFieldTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RecordItems_User_UserId",
                table: "RecordItems",
                column: "UserId",
                principalSchema: "dbo",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RecordElements_RecordElementInputTypes_InputTypeId",
                table: "RecordElements");

            migrationBuilder.DropForeignKey(
                name: "FK_RecordElements_RecordElementLookupTableFieldTypes_LookupTableFieldTypeId",
                table: "RecordElements");

            migrationBuilder.DropForeignKey(
                name: "FK_RecordItems_User_UserId",
                table: "RecordItems");

            migrationBuilder.DropTable(
                name: "RecordDefaultElements");

            migrationBuilder.DropTable(
                name: "RecordElementDataTypeInputTypes");

            migrationBuilder.DropTable(
                name: "RecordElementLookupTableFieldTypes");

            migrationBuilder.DropTable(
                name: "RecordElementInputTypes");

            migrationBuilder.DropIndex(
                name: "IX_RecordItems_UserId",
                table: "RecordItems");

            migrationBuilder.DropIndex(
                name: "IX_RecordElements_InputTypeId",
                table: "RecordElements");

            migrationBuilder.DropIndex(
                name: "IX_RecordElements_LookupTableFieldTypeId",
                table: "RecordElements");

            migrationBuilder.DropColumn(
                name: "Visible",
                table: "RecordItemUsers");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "RecordItems");

            migrationBuilder.DropColumn(
                name: "InputTypeId",
                table: "RecordElements");

            migrationBuilder.DropColumn(
                name: "LookupTableFieldTypeId",
                table: "RecordElements");

            migrationBuilder.DropColumn(
                name: "ItemDescriptionColumnHeader",
                table: "RecordCategories");

            migrationBuilder.DropColumn(
                name: "ItemValueColumnHeader",
                table: "RecordCategories");
        }
    }
}

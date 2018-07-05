using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ElGroupo.Domain.Migrations
{
    public partial class addsubcategories : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RecordItems_RecordCategories_CategoryId",
                table: "RecordItems");

            migrationBuilder.AlterColumn<long>(
                name: "CategoryId",
                table: "RecordItems",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AddColumn<long>(
                name: "SubCategoryId",
                table: "RecordItems",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RecordSubCategories",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    ParentCategoryId = table.Column<long>(nullable: false),
                    UserCreated = table.Column<string>(nullable: true),
                    UserUpdated = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecordSubCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecordSubCategories_RecordCategories_ParentCategoryId",
                        column: x => x.ParentCategoryId,
                        principalTable: "RecordCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RecordItems_SubCategoryId",
                table: "RecordItems",
                column: "SubCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_RecordSubCategories_ParentCategoryId",
                table: "RecordSubCategories",
                column: "ParentCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_RecordItems_RecordCategories_CategoryId",
                table: "RecordItems",
                column: "CategoryId",
                principalTable: "RecordCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RecordItems_RecordSubCategories_SubCategoryId",
                table: "RecordItems",
                column: "SubCategoryId",
                principalTable: "RecordSubCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RecordItems_RecordCategories_CategoryId",
                table: "RecordItems");

            migrationBuilder.DropForeignKey(
                name: "FK_RecordItems_RecordSubCategories_SubCategoryId",
                table: "RecordItems");

            migrationBuilder.DropTable(
                name: "RecordSubCategories");

            migrationBuilder.DropIndex(
                name: "IX_RecordItems_SubCategoryId",
                table: "RecordItems");

            migrationBuilder.DropColumn(
                name: "SubCategoryId",
                table: "RecordItems");

            migrationBuilder.AlterColumn<long>(
                name: "CategoryId",
                table: "RecordItems",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_RecordItems_RecordCategories_CategoryId",
                table: "RecordItems",
                column: "CategoryId",
                principalTable: "RecordCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

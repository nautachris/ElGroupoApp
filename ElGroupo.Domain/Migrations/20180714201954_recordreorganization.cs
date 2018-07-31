using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ElGroupo.Domain.Migrations
{
    public partial class recordreorganization : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RecordItems_User_UserId",
                table: "RecordItems");

            migrationBuilder.DropTable(
                name: "RecordItemUserData");

            migrationBuilder.DropTable(
                name: "RecordItemUserDocuments");

            migrationBuilder.DropTable(
                name: "RecordItemUsers");

            migrationBuilder.AddColumn<string>(
                name: "Value",
                table: "RecordItemElements",
                nullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "UserId",
                table: "RecordItems",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ItemTypeId",
                table: "RecordItems",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Visible",
                table: "RecordItems",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "LabelOnSameRow",
                table: "RecordElements",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "RecordItemDocuments",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    ContentType = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    FileName = table.Column<string>(nullable: true),
                    ImageData = table.Column<byte[]>(nullable: true),
                    ItemId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecordItemDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecordItemDocuments_RecordItems_ItemId",
                        column: x => x.ItemId,
                        principalTable: "RecordItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RecordItemTypes",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    CategoryId = table.Column<long>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    SubCategoryId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecordItemTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecordItemTypes_RecordCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "RecordCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecordItemTypes_RecordSubCategories_SubCategoryId",
                        column: x => x.SubCategoryId,
                        principalTable: "RecordSubCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RecordItems_ItemTypeId",
                table: "RecordItems",
                column: "ItemTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_RecordItemDocuments_ItemId",
                table: "RecordItemDocuments",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_RecordItemTypes_CategoryId",
                table: "RecordItemTypes",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_RecordItemTypes_SubCategoryId",
                table: "RecordItemTypes",
                column: "SubCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_RecordItems_RecordItemTypes_ItemTypeId",
                table: "RecordItems",
                column: "ItemTypeId",
                principalTable: "RecordItemTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RecordItems_User_UserId",
                table: "RecordItems",
                column: "UserId",
                principalSchema: "dbo",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RecordItems_RecordItemTypes_ItemTypeId",
                table: "RecordItems");

            migrationBuilder.DropForeignKey(
                name: "FK_RecordItems_User_UserId",
                table: "RecordItems");

            migrationBuilder.DropTable(
                name: "RecordItemDocuments");

            migrationBuilder.DropTable(
                name: "RecordItemTypes");

            migrationBuilder.DropIndex(
                name: "IX_RecordItems_ItemTypeId",
                table: "RecordItems");

            migrationBuilder.DropColumn(
                name: "Value",
                table: "RecordItemElements");

            migrationBuilder.DropColumn(
                name: "ItemTypeId",
                table: "RecordItems");

            migrationBuilder.DropColumn(
                name: "Visible",
                table: "RecordItems");

            migrationBuilder.AlterColumn<long>(
                name: "UserId",
                table: "RecordItems",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<bool>(
                name: "LabelOnSameRow",
                table: "RecordElements",
                nullable: true,
                oldClrType: typeof(bool));

            migrationBuilder.CreateTable(
                name: "RecordItemUsers",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    ItemId = table.Column<long>(nullable: false),
                    UserId = table.Column<long>(nullable: false),
                    Visible = table.Column<bool>(nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecordItemUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecordItemUsers_RecordItems_ItemId",
                        column: x => x.ItemId,
                        principalTable: "RecordItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecordItemUsers_User_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RecordItemUserData",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    ElementId = table.Column<long>(nullable: false),
                    ItemUserId = table.Column<long>(nullable: false),
                    Value = table.Column<string>(maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecordItemUserData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecordItemUserData_RecordItemElements_ElementId",
                        column: x => x.ElementId,
                        principalTable: "RecordItemElements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecordItemUserData_RecordItemUsers_ItemUserId",
                        column: x => x.ItemUserId,
                        principalTable: "RecordItemUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RecordItemUserDocuments",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    ContentType = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    FileName = table.Column<string>(nullable: true),
                    ImageData = table.Column<byte[]>(nullable: true),
                    ItemUserId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecordItemUserDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecordItemUserDocuments_RecordItemUsers_ItemUserId",
                        column: x => x.ItemUserId,
                        principalTable: "RecordItemUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RecordItemUsers_ItemId",
                table: "RecordItemUsers",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_RecordItemUsers_UserId",
                table: "RecordItemUsers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RecordItemUserData_ElementId",
                table: "RecordItemUserData",
                column: "ElementId");

            migrationBuilder.CreateIndex(
                name: "IX_RecordItemUserData_ItemUserId",
                table: "RecordItemUserData",
                column: "ItemUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RecordItemUserDocuments_ItemUserId",
                table: "RecordItemUserDocuments",
                column: "ItemUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_RecordItems_User_UserId",
                table: "RecordItems",
                column: "UserId",
                principalSchema: "dbo",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

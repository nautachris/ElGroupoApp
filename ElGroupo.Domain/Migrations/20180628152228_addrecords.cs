using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ElGroupo.Domain.Migrations
{
    public partial class addrecords : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RecordCategories",
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
                    table.PrimaryKey("PK_RecordCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RecordElements",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    DataType = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    UserCreated = table.Column<string>(nullable: true),
                    UserUpdated = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecordElements", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RecordItems",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    CategoryId = table.Column<long>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    UserCreated = table.Column<string>(nullable: true),
                    UserUpdated = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecordItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecordItems_RecordCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "RecordCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RecordItemElements",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: false),
                    ElementId = table.Column<long>(nullable: false),
                    ItemId = table.Column<long>(nullable: false),
                    PrimaryDisplay = table.Column<bool>(nullable: false),
                    UserCreated = table.Column<string>(nullable: true),
                    UserUpdated = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecordItemElements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecordItemElements_RecordElements_ElementId",
                        column: x => x.ElementId,
                        principalTable: "RecordElements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecordItemElements_RecordItems_ItemId",
                        column: x => x.ItemId,
                        principalTable: "RecordItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RecordItemUsers",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: false),
                    ItemId = table.Column<long>(nullable: false),
                    UserCreated = table.Column<string>(nullable: true),
                    UserId = table.Column<long>(nullable: false),
                    UserUpdated = table.Column<string>(nullable: true)
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
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: false),
                    ElementId = table.Column<long>(nullable: false),
                    ItemUserId = table.Column<long>(nullable: false),
                    UserCreated = table.Column<string>(nullable: true),
                    UserUpdated = table.Column<string>(nullable: true),
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
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    FileName = table.Column<string>(nullable: true),
                    ImageData = table.Column<byte[]>(nullable: true),
                    ItemUserId = table.Column<long>(nullable: false),
                    UserCreated = table.Column<string>(nullable: true),
                    UserUpdated = table.Column<string>(nullable: true)
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
                name: "IX_RecordItems_CategoryId",
                table: "RecordItems",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_RecordItemElements_ElementId",
                table: "RecordItemElements",
                column: "ElementId");

            migrationBuilder.CreateIndex(
                name: "IX_RecordItemElements_ItemId",
                table: "RecordItemElements",
                column: "ItemId");

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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecordItemUserData");

            migrationBuilder.DropTable(
                name: "RecordItemUserDocuments");

            migrationBuilder.DropTable(
                name: "RecordItemElements");

            migrationBuilder.DropTable(
                name: "RecordItemUsers");

            migrationBuilder.DropTable(
                name: "RecordElements");

            migrationBuilder.DropTable(
                name: "RecordItems");

            migrationBuilder.DropTable(
                name: "RecordCategories");
        }
    }
}

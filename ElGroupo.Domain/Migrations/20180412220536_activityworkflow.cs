using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ElGroupo.Domain.Migrations
{
    public partial class activityworkflow : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "Active",
                table: "EventAttendees",
                nullable: true,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldDefaultValue: true);

            migrationBuilder.CreateTable(
                name: "AccreditationDocuments",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    ContentType = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: false),
                    ImageData = table.Column<byte[]>(nullable: true),
                    UserCreated = table.Column<string>(nullable: true),
                    UserUpdated = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccreditationDocuments", x => x.Id);
                });

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

            migrationBuilder.CreateTable(
                name: "CreditTypes",
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
                    table.PrimaryKey("PK_CreditTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Organizations",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    Address1 = table.Column<string>(nullable: true),
                    Address2 = table.Column<string>(nullable: true),
                    Address3 = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    State = table.Column<string>(nullable: true),
                    UserCreated = table.Column<string>(nullable: true),
                    UserUpdated = table.Column<string>(nullable: true),
                    Zip = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organizations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CreditTypeCategories",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    CreditTypeId = table.Column<long>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(maxLength: 255, nullable: false),
                    UserCreated = table.Column<string>(nullable: true),
                    UserUpdated = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditTypeCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CreditTypeCategories_CreditTypes_CreditTypeId",
                        column: x => x.CreditTypeId,
                        principalTable: "CreditTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(maxLength: 255, nullable: false),
                    OrganizationId = table.Column<long>(nullable: false),
                    UserCreated = table.Column<string>(nullable: true),
                    UserUpdated = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Departments_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationAccreditations",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    AuthorityId = table.Column<string>(nullable: true),
                    CreditTypeId = table.Column<long>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: false),
                    OrganizationId = table.Column<long>(nullable: false),
                    UserCreated = table.Column<string>(nullable: true),
                    UserUpdated = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationAccreditations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganizationAccreditations_CreditTypes_CreditTypeId",
                        column: x => x.CreditTypeId,
                        principalTable: "CreditTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganizationAccreditations_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ActivityGroups",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    DateCreated = table.Column<DateTime>(maxLength: 255, nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: false),
                    DepartmentId = table.Column<long>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    UserCreated = table.Column<string>(nullable: true),
                    UserUpdated = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActivityGroups_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DepartmentUsers",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: false),
                    DepartmentId = table.Column<long>(nullable: false),
                    UserCreated = table.Column<string>(nullable: true),
                    UserId = table.Column<long>(nullable: false),
                    UserUpdated = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepartmentUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DepartmentUsers_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DepartmentUsers_User_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DepartmentUserGroups",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: false),
                    DepartmentId = table.Column<long>(nullable: false),
                    Name = table.Column<string>(maxLength: 255, nullable: false),
                    UserCreated = table.Column<string>(nullable: true),
                    UserUpdated = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepartmentUserGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DepartmentUserGroups_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Activities",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    ActivityGroupId = table.Column<long>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    EndDate = table.Column<DateTime>(nullable: true),
                    Location = table.Column<string>(nullable: true),
                    StartDate = table.Column<DateTime>(nullable: true),
                    UserCreated = table.Column<string>(nullable: true),
                    UserUpdated = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Activities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Activities_ActivityGroups_ActivityGroupId",
                        column: x => x.ActivityGroupId,
                        principalTable: "ActivityGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ActivityGroupOrganizers",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    ActivityGroupId = table.Column<long>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: false),
                    UserCreated = table.Column<string>(nullable: true),
                    UserId = table.Column<long>(nullable: false),
                    UserUpdated = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityGroupOrganizers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActivityGroupOrganizers_ActivityGroups_ActivityGroupId",
                        column: x => x.ActivityGroupId,
                        principalTable: "ActivityGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActivityGroupOrganizers_User_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DepartmentUserGroupActivityGroups",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    ActivityGroupId = table.Column<long>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: false),
                    UserCreated = table.Column<string>(nullable: true),
                    UserGroupId = table.Column<long>(nullable: false),
                    UserUpdated = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepartmentUserGroupActivityGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DepartmentUserGroupActivityGroups_ActivityGroups_ActivityGroupId",
                        column: x => x.ActivityGroupId,
                        principalTable: "ActivityGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DepartmentUserGroupActivityGroups_DepartmentUserGroups_UserGroupId",
                        column: x => x.UserGroupId,
                        principalTable: "DepartmentUserGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DepartmentUserGroupUsers",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: false),
                    IsOwner = table.Column<bool>(nullable: false),
                    UserCreated = table.Column<string>(nullable: true),
                    UserGroupId = table.Column<long>(nullable: false),
                    UserId = table.Column<long>(nullable: false),
                    UserUpdated = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepartmentUserGroupUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DepartmentUserGroupUsers_DepartmentUserGroups_UserGroupId",
                        column: x => x.UserGroupId,
                        principalTable: "DepartmentUserGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DepartmentUserGroupUsers_DepartmentUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "DepartmentUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ActivityCredits",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    ActivityId = table.Column<long>(nullable: false),
                    CreditTypeCategoryId = table.Column<long>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: false),
                    UserCreated = table.Column<string>(nullable: true),
                    UserUpdated = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityCredits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActivityCredits_Activities_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "Activities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActivityCredits_CreditTypeCategories_CreditTypeCategoryId",
                        column: x => x.CreditTypeCategoryId,
                        principalTable: "CreditTypeCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserActivities",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    ActivityId = table.Column<long>(nullable: false),
                    AttendanceTypeId = table.Column<long>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: false),
                    UserCreated = table.Column<string>(nullable: true),
                    UserId = table.Column<long>(nullable: false),
                    UserUpdated = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserActivities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserActivities_Activities_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "Activities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserActivities_ActivityAttendanceTypes_AttendanceTypeId",
                        column: x => x.AttendanceTypeId,
                        principalTable: "ActivityAttendanceTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserActivities_User_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserActivityCredits",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    CreditHours = table.Column<double>(nullable: false, defaultValue: 0.0),
                    CreditTypeCategoryId = table.Column<long>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: false),
                    UserActivityId = table.Column<long>(nullable: false),
                    UserCreated = table.Column<string>(nullable: true),
                    UserUpdated = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserActivityCredits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserActivityCredits_CreditTypeCategories_CreditTypeCategoryId",
                        column: x => x.CreditTypeCategoryId,
                        principalTable: "CreditTypeCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserActivityCredits_UserActivities_UserActivityId",
                        column: x => x.UserActivityId,
                        principalTable: "UserActivities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserActivityDocuments",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DateUpdated = table.Column<DateTime>(nullable: false),
                    DocumentId = table.Column<long>(nullable: false),
                    UserActivityId = table.Column<long>(nullable: false),
                    UserCreated = table.Column<string>(nullable: true),
                    UserUpdated = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserActivityDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserActivityDocuments_AccreditationDocuments_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "AccreditationDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserActivityDocuments_UserActivities_UserActivityId",
                        column: x => x.UserActivityId,
                        principalTable: "UserActivities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Activities_ActivityGroupId",
                table: "Activities",
                column: "ActivityGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityCredits_ActivityId",
                table: "ActivityCredits",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityCredits_CreditTypeCategoryId",
                table: "ActivityCredits",
                column: "CreditTypeCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityGroups_DepartmentId",
                table: "ActivityGroups",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityGroupOrganizers_ActivityGroupId",
                table: "ActivityGroupOrganizers",
                column: "ActivityGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityGroupOrganizers_UserId",
                table: "ActivityGroupOrganizers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditTypeCategories_CreditTypeId",
                table: "CreditTypeCategories",
                column: "CreditTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_OrganizationId",
                table: "Departments",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentUsers_DepartmentId",
                table: "DepartmentUsers",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentUsers_UserId",
                table: "DepartmentUsers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentUserGroups_DepartmentId",
                table: "DepartmentUserGroups",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentUserGroupActivityGroups_ActivityGroupId",
                table: "DepartmentUserGroupActivityGroups",
                column: "ActivityGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentUserGroupActivityGroups_UserGroupId",
                table: "DepartmentUserGroupActivityGroups",
                column: "UserGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentUserGroupUsers_UserGroupId",
                table: "DepartmentUserGroupUsers",
                column: "UserGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentUserGroupUsers_UserId",
                table: "DepartmentUserGroupUsers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationAccreditations_CreditTypeId",
                table: "OrganizationAccreditations",
                column: "CreditTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationAccreditations_OrganizationId",
                table: "OrganizationAccreditations",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_UserActivities_ActivityId",
                table: "UserActivities",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_UserActivities_AttendanceTypeId",
                table: "UserActivities",
                column: "AttendanceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_UserActivities_UserId",
                table: "UserActivities",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserActivityCredits_CreditTypeCategoryId",
                table: "UserActivityCredits",
                column: "CreditTypeCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_UserActivityCredits_UserActivityId",
                table: "UserActivityCredits",
                column: "UserActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_UserActivityDocuments_DocumentId",
                table: "UserActivityDocuments",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_UserActivityDocuments_UserActivityId",
                table: "UserActivityDocuments",
                column: "UserActivityId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivityCredits");

            migrationBuilder.DropTable(
                name: "ActivityGroupOrganizers");

            migrationBuilder.DropTable(
                name: "DepartmentUserGroupActivityGroups");

            migrationBuilder.DropTable(
                name: "DepartmentUserGroupUsers");

            migrationBuilder.DropTable(
                name: "OrganizationAccreditations");

            migrationBuilder.DropTable(
                name: "UserActivityCredits");

            migrationBuilder.DropTable(
                name: "UserActivityDocuments");

            migrationBuilder.DropTable(
                name: "DepartmentUserGroups");

            migrationBuilder.DropTable(
                name: "DepartmentUsers");

            migrationBuilder.DropTable(
                name: "CreditTypeCategories");

            migrationBuilder.DropTable(
                name: "AccreditationDocuments");

            migrationBuilder.DropTable(
                name: "UserActivities");

            migrationBuilder.DropTable(
                name: "CreditTypes");

            migrationBuilder.DropTable(
                name: "Activities");

            migrationBuilder.DropTable(
                name: "ActivityAttendanceTypes");

            migrationBuilder.DropTable(
                name: "ActivityGroups");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropTable(
                name: "Organizations");

            migrationBuilder.AlterColumn<bool>(
                name: "Active",
                table: "EventAttendees",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldNullable: true,
                oldDefaultValue: true);
        }
    }
}

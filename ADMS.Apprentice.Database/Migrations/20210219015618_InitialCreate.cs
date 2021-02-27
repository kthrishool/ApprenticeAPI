#region Not using migration scripts
//using System;
//using Microsoft.EntityFrameworkCore.Migrations;

//namespace ADMS.Apprentice.Database.Migrations
//{
//    public partial class InitialCreate : Migration
//    {
//        protected override void Up(MigrationBuilder migrationBuilder)
//        {
//            migrationBuilder.EnsureSchema(
//                name: "dbo");

//            migrationBuilder.CreateTable(
//                name: "ClaimSubmission",
//                schema: "dbo",
//                columns: table => new
//                {
//                    Id = table.Column<int>(type: "int", nullable: false)
//                        .Annotation("SqlServer:Identity", "1, 1"),
//                    SubmissionStatus = table.Column<int>(type: "int", nullable: false),
//                    Type = table.Column<int>(type: "int", nullable: false),
//                    Category = table.Column<int>(type: "int", nullable: false),
//                    ApprenticeId = table.Column<int>(type: "int", nullable: false),
//                    ApprenticeName = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
//                    EmployerId = table.Column<int>(type: "int", nullable: false),
//                    EmployerName = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
//                    NetworkProviderId = table.Column<int>(type: "int", nullable: false),
//                    NetworkProviderName = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
//                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
//                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
//                },
//                constraints: table =>
//                {
//                    table.PrimaryKey("PK_ClaimSubmission", x => x.Id);
//                });
//        }

//        protected override void Down(MigrationBuilder migrationBuilder)
//        {
//            migrationBuilder.DropTable(
//                name: "ClaimSubmission",
//                schema: "dbo");
//        }
//    }
//}
#endregion
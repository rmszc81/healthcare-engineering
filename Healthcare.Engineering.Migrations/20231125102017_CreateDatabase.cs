using System;

using Microsoft.EntityFrameworkCore.Migrations;

namespace Healthcare.Engineering.Migrations;

public partial class CreateDatabase : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "customer",
            columns: table => new
            {
                id = table.Column<Guid>(type: "TEXT", nullable: false),
                first_name = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                last_name = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                email = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                phone_number = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false)
            },
            constraints: table => { table.PrimaryKey("customer_id_pk", x => x.id); });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "customer");
    }
}
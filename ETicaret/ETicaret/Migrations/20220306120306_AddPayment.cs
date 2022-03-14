using Microsoft.EntityFrameworkCore.Migrations;

namespace ETicaret.Migrations
{
    public partial class AddPayment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CartName",
                table: "OrderHeaders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CartNumber",
                table: "OrderHeaders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Cvc",
                table: "OrderHeaders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ExprationMonth",
                table: "OrderHeaders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ExprationYear",
                table: "OrderHeaders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CartName",
                table: "OrderHeaders");

            migrationBuilder.DropColumn(
                name: "CartNumber",
                table: "OrderHeaders");

            migrationBuilder.DropColumn(
                name: "Cvc",
                table: "OrderHeaders");

            migrationBuilder.DropColumn(
                name: "ExprationMonth",
                table: "OrderHeaders");

            migrationBuilder.DropColumn(
                name: "ExprationYear",
                table: "OrderHeaders");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BsdFinalProject.Migrations
{
    /// <inheritdoc />
    public partial class Createing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Card_Gift_GiftId",
                table: "Card");

            migrationBuilder.DropForeignKey(
                name: "FK_Gift_Donor_DonorId",
                table: "Gift");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Gift_Cost",
                table: "Gift");

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "User",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "CategoryId1",
                table: "Gift",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DonorId1",
                table: "Gift",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Gift_CategoryId1",
                table: "Gift",
                column: "CategoryId1");

            migrationBuilder.CreateIndex(
                name: "IX_Gift_DonorId1",
                table: "Gift",
                column: "DonorId1");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Gift_Cost",
                table: "Gift",
                sql: "Cost >= 10 AND Cost <= 100");

            migrationBuilder.CreateIndex(
                name: "IX_Basket_GiftId",
                table: "Basket",
                column: "GiftId");

            migrationBuilder.AddForeignKey(
                name: "FK_Basket_Gift_GiftId",
                table: "Basket",
                column: "GiftId",
                principalTable: "Gift",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Card_Gift_GiftId",
                table: "Card",
                column: "GiftId",
                principalTable: "Gift",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Gift_Category_CategoryId1",
                table: "Gift",
                column: "CategoryId1",
                principalTable: "Category",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Gift_Donor_DonorId",
                table: "Gift",
                column: "DonorId",
                principalTable: "Donor",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Gift_Donor_DonorId1",
                table: "Gift",
                column: "DonorId1",
                principalTable: "Donor",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Basket_Gift_GiftId",
                table: "Basket");

            migrationBuilder.DropForeignKey(
                name: "FK_Card_Gift_GiftId",
                table: "Card");

            migrationBuilder.DropForeignKey(
                name: "FK_Gift_Category_CategoryId1",
                table: "Gift");

            migrationBuilder.DropForeignKey(
                name: "FK_Gift_Donor_DonorId",
                table: "Gift");

            migrationBuilder.DropForeignKey(
                name: "FK_Gift_Donor_DonorId1",
                table: "Gift");

            migrationBuilder.DropIndex(
                name: "IX_Gift_CategoryId1",
                table: "Gift");

            migrationBuilder.DropIndex(
                name: "IX_Gift_DonorId1",
                table: "Gift");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Gift_Cost",
                table: "Gift");

            migrationBuilder.DropIndex(
                name: "IX_Basket_GiftId",
                table: "Basket");

            migrationBuilder.DropColumn(
                name: "CategoryId1",
                table: "Gift");

            migrationBuilder.DropColumn(
                name: "DonorId1",
                table: "Gift");

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "User",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Gift_Cost",
                table: "Gift",
                sql: "Cost > 10 AND Cost < 100");

            migrationBuilder.AddForeignKey(
                name: "FK_Card_Gift_GiftId",
                table: "Card",
                column: "GiftId",
                principalTable: "Gift",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Gift_Donor_DonorId",
                table: "Gift",
                column: "DonorId",
                principalTable: "Donor",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

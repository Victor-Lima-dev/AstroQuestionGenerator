using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class tagnobancodedados : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PerguntaTAG_TAG_TAGsId",
                table: "PerguntaTAG");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TAG",
                table: "TAG");

            migrationBuilder.RenameTable(
                name: "TAG",
                newName: "TAGs");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TAGs",
                table: "TAGs",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PerguntaTAG_TAGs_TAGsId",
                table: "PerguntaTAG",
                column: "TAGsId",
                principalTable: "TAGs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PerguntaTAG_TAGs_TAGsId",
                table: "PerguntaTAG");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TAGs",
                table: "TAGs");

            migrationBuilder.RenameTable(
                name: "TAGs",
                newName: "TAG");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TAG",
                table: "TAG",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PerguntaTAG_TAG_TAGsId",
                table: "PerguntaTAG",
                column: "TAGsId",
                principalTable: "TAG",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

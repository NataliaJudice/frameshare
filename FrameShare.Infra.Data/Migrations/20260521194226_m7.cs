using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FrameShare.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class m7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Foto_UserId",
                table: "Foto",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Foto_Usuario_UserId",
                table: "Foto",
                column: "UserId",
                principalTable: "Usuario",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Foto_Usuario_UserId",
                table: "Foto");

            migrationBuilder.DropIndex(
                name: "IX_Foto_UserId",
                table: "Foto");
        }
    }
}

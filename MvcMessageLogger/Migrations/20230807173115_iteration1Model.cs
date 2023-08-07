using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MvcMessageLogger.Migrations
{
    /// <inheritdoc />
    public partial class iteration1Model : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_messages_users_user_id",
                table: "messages");

            migrationBuilder.DropIndex(
                name: "ix_messages_user_id",
                table: "messages");

            migrationBuilder.DropColumn(
                name: "user_id",
                table: "messages");

            migrationBuilder.AddColumn<string>(
                name: "email",
                table: "users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "password",
                table: "users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "author_id",
                table: "messages",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "ix_messages_author_id",
                table: "messages",
                column: "author_id");

            migrationBuilder.AddForeignKey(
                name: "fk_messages_users_author_id",
                table: "messages",
                column: "author_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_messages_users_author_id",
                table: "messages");

            migrationBuilder.DropIndex(
                name: "ix_messages_author_id",
                table: "messages");

            migrationBuilder.DropColumn(
                name: "email",
                table: "users");

            migrationBuilder.DropColumn(
                name: "password",
                table: "users");

            migrationBuilder.DropColumn(
                name: "author_id",
                table: "messages");

            migrationBuilder.AddColumn<int>(
                name: "user_id",
                table: "messages",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_messages_user_id",
                table: "messages",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_messages_users_user_id",
                table: "messages",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id");
        }
    }
}

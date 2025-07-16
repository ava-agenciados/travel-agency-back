using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace travel_agency_back.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PackageMedia_Packages_PackageId",
                table: "PackageMedia");

            migrationBuilder.DropForeignKey(
                name: "FK_Rating_Packages_PackageId",
                table: "Rating");

            migrationBuilder.DropForeignKey(
                name: "FK_Rating_Usuarios_UserId",
                table: "Rating");

            migrationBuilder.DropForeignKey(
                name: "FK_UserDocument_Usuarios_UserId",
                table: "UserDocument");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserDocument",
                table: "UserDocument");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Rating",
                table: "Rating");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PackageMedia",
                table: "PackageMedia");

            migrationBuilder.RenameTable(
                name: "UserDocument",
                newName: "UserDocuments");

            migrationBuilder.RenameTable(
                name: "Rating",
                newName: "Ratings");

            migrationBuilder.RenameTable(
                name: "PackageMedia",
                newName: "PackageMedias");

            migrationBuilder.RenameIndex(
                name: "IX_UserDocument_UserId",
                table: "UserDocuments",
                newName: "IX_UserDocuments_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Rating_UserId",
                table: "Ratings",
                newName: "IX_Ratings_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Rating_PackageId",
                table: "Ratings",
                newName: "IX_Ratings_PackageId");

            migrationBuilder.RenameIndex(
                name: "IX_PackageMedia_PackageId",
                table: "PackageMedias",
                newName: "IX_PackageMedias_PackageId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserDocuments",
                table: "UserDocuments",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Ratings",
                table: "Ratings",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PackageMedias",
                table: "PackageMedias",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PackageMedias_Packages_PackageId",
                table: "PackageMedias",
                column: "PackageId",
                principalTable: "Packages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Ratings_Packages_PackageId",
                table: "Ratings",
                column: "PackageId",
                principalTable: "Packages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Ratings_Usuarios_UserId",
                table: "Ratings",
                column: "UserId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserDocuments_Usuarios_UserId",
                table: "UserDocuments",
                column: "UserId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PackageMedias_Packages_PackageId",
                table: "PackageMedias");

            migrationBuilder.DropForeignKey(
                name: "FK_Ratings_Packages_PackageId",
                table: "Ratings");

            migrationBuilder.DropForeignKey(
                name: "FK_Ratings_Usuarios_UserId",
                table: "Ratings");

            migrationBuilder.DropForeignKey(
                name: "FK_UserDocuments_Usuarios_UserId",
                table: "UserDocuments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserDocuments",
                table: "UserDocuments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Ratings",
                table: "Ratings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PackageMedias",
                table: "PackageMedias");

            migrationBuilder.RenameTable(
                name: "UserDocuments",
                newName: "UserDocument");

            migrationBuilder.RenameTable(
                name: "Ratings",
                newName: "Rating");

            migrationBuilder.RenameTable(
                name: "PackageMedias",
                newName: "PackageMedia");

            migrationBuilder.RenameIndex(
                name: "IX_UserDocuments_UserId",
                table: "UserDocument",
                newName: "IX_UserDocument_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Ratings_UserId",
                table: "Rating",
                newName: "IX_Rating_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Ratings_PackageId",
                table: "Rating",
                newName: "IX_Rating_PackageId");

            migrationBuilder.RenameIndex(
                name: "IX_PackageMedias_PackageId",
                table: "PackageMedia",
                newName: "IX_PackageMedia_PackageId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserDocument",
                table: "UserDocument",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Rating",
                table: "Rating",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PackageMedia",
                table: "PackageMedia",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PackageMedia_Packages_PackageId",
                table: "PackageMedia",
                column: "PackageId",
                principalTable: "Packages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Rating_Packages_PackageId",
                table: "Rating",
                column: "PackageId",
                principalTable: "Packages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Rating_Usuarios_UserId",
                table: "Rating",
                column: "UserId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserDocument_Usuarios_UserId",
                table: "UserDocument",
                column: "UserId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

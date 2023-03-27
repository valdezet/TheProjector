using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheProjector.Migrations
{
    /// <inheritdoc />
    public partial class AddSuperAdministrator : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(128 / 8);
            string defaultPassword = "Pa$$w0rd";

            byte[] derivedKey = KeyDerivation.Pbkdf2(
                defaultPassword,
                salt,
                KeyDerivationPrf.HMACSHA512,
                600000,
                256 / 8
            );

            byte[] password = new byte[salt.Length + derivedKey.Length];
            Buffer.BlockCopy(salt, 0, password, 0, salt.Length);
            Buffer.BlockCopy(derivedKey, 0, password, salt.Length, derivedKey.Length);
            string passwordHashBase64 = Convert.ToBase64String(password);
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "EmailNormalized", "PasswordHash" },
                values: new object[,] {
                    {
                        1L,
                        "superadmin@example.local",
                        "SUPERADMIN@EXAMPLE.LOCAL",
                        passwordHashBase64
                    }
                }
            );

            migrationBuilder.InsertData(
                table: "People",
                columns: new[] { "FirstName", "LastName", "UserId" },
                values: new object[,] {
                    {"Super", "Administrator", 1}
                }
            );

            migrationBuilder.Sql("INSERT INTO PersonRole (PeopleId, RolesId) VALUES (SCOPE_IDENTITY(),1)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}

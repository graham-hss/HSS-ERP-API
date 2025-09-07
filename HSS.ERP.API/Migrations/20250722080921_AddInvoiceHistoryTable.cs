using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HSS.ERP.API.Migrations
{
    /// <inheritdoc />
    public partial class AddInvoiceHistoryTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "invoice_history",
                schema: "tms",
                columns: table => new
                {
                    history_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    invoice_id = table.Column<int>(type: "integer", nullable: false),
                    history_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    history_title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    history_content = table.Column<string>(type: "text", nullable: false),
                    created_by = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_invoice_history", x => x.history_id);
                    table.ForeignKey(
                        name: "FK_invoice_history_hss_invoice_invoice_id",
                        column: x => x.invoice_id,
                        principalSchema: "tms",
                        principalTable: "hss_invoice",
                        principalColumn: "invoice_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceHistory_InvoiceId",
                schema: "tms",
                table: "invoice_history",
                column: "invoice_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "invoice_history",
                schema: "tms");
        }
    }
}

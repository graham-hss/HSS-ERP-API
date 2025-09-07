using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HSS.ERP.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreatePostgreSQL : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "invoices",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    invoice_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    invoice_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    due_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    customer_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    customer_email = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    billing_address = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    subtotal = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    tax_amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    total_amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    refund_amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_modified_by = table.Column<string>(type: "text", nullable: true),
                    last_modified_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_invoices", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "invoice_lines",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    invoice_id = table.Column<int>(type: "integer", nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    unit_price = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    line_total = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    refund_amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    product_code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    unit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    tax_rate = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    tax_amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_modified_by = table.Column<string>(type: "text", nullable: true),
                    last_modified_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_invoice_lines", x => x.id);
                    table.ForeignKey(
                        name: "FK_invoice_lines_invoices_invoice_id",
                        column: x => x.invoice_id,
                        principalTable: "invoices",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "invoices",
                columns: new[] { "id", "billing_address", "created_at", "created_by", "customer_email", "customer_name", "due_date", "invoice_date", "invoice_number", "last_modified_at", "last_modified_by", "notes", "refund_amount", "status", "subtotal", "tax_amount", "total_amount" },
                values: new object[,]
                {
                    { 1, "123 Business Ave, Seattle, WA 98101", new DateTime(2025, 1, 10, 10, 0, 0, 0, DateTimeKind.Utc), "user1@company.com", "billing@contoso.com", "Contoso Corporation", new DateTime(2025, 2, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "INV-2025-001", null, null, "Software licensing for Q1 2025", 0.00m, "Paid", 5000.00m, 500.00m, 5500.00m },
                    { 2, "456 Commerce St, Portland, OR 97201", new DateTime(2025, 1, 28, 14, 30, 0, 0, DateTimeKind.Utc), "user2@company.com", "accounts@adventureworks.com", "Adventure Works", new DateTime(2025, 3, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "INV-2025-002", null, null, "Consulting services - partial refund processed", 150.00m, "Sent", 2500.00m, 250.00m, 2750.00m }
                });

            migrationBuilder.InsertData(
                table: "invoice_lines",
                columns: new[] { "id", "created_at", "description", "invoice_id", "last_modified_at", "last_modified_by", "line_total", "product_code", "quantity", "refund_amount", "tax_amount", "tax_rate", "unit", "unit_price" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 10, 10, 0, 0, 0, DateTimeKind.Utc), "Microsoft Office 365 Business Premium - 50 licenses", 1, null, null, 1100.00m, "O365-BP", 50, 0.00m, 110.00m, 10.00m, "license", 22.00m },
                    { 2, new DateTime(2025, 1, 10, 10, 0, 0, 0, DateTimeKind.Utc), "Azure Cloud Services - Monthly subscription", 1, null, null, 3900.00m, "AZURE-MONTHLY", 1, 0.00m, 390.00m, 10.00m, "month", 3900.00m },
                    { 3, new DateTime(2025, 1, 28, 14, 30, 0, 0, DateTimeKind.Utc), "Software Development Consulting", 2, null, null, 2500.00m, "CONSULT-DEV", 20, 150.00m, 250.00m, 10.00m, "hour", 125.00m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_invoice_lines_invoice_id",
                table: "invoice_lines",
                column: "invoice_id");

            migrationBuilder.CreateIndex(
                name: "IX_invoice_lines_product_code",
                table: "invoice_lines",
                column: "product_code");

            migrationBuilder.CreateIndex(
                name: "IX_invoices_created_at",
                table: "invoices",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "IX_invoices_customer_email",
                table: "invoices",
                column: "customer_email");

            migrationBuilder.CreateIndex(
                name: "IX_invoices_invoice_number",
                table: "invoices",
                column: "invoice_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_invoices_status",
                table: "invoices",
                column: "status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "invoice_lines");

            migrationBuilder.DropTable(
                name: "invoices");
        }
    }
}

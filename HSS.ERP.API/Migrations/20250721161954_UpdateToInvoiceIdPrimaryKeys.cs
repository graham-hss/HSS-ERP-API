using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HSS.ERP.API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateToInvoiceIdPrimaryKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_invoice_lines_invoices_invoice_id",
                table: "invoice_lines");

            migrationBuilder.DropPrimaryKey(
                name: "PK_invoices",
                table: "invoices");

            migrationBuilder.DropIndex(
                name: "IX_invoices_created_at",
                table: "invoices");

            migrationBuilder.DropIndex(
                name: "IX_invoices_customer_email",
                table: "invoices");

            migrationBuilder.DropIndex(
                name: "IX_invoices_invoice_number",
                table: "invoices");

            migrationBuilder.DropIndex(
                name: "IX_invoices_status",
                table: "invoices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_invoice_lines",
                table: "invoice_lines");

            migrationBuilder.DropIndex(
                name: "IX_invoice_lines_product_code",
                table: "invoice_lines");

            migrationBuilder.DeleteData(
                table: "invoice_lines",
                keyColumn: "id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "invoice_lines",
                keyColumn: "id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "invoice_lines",
                keyColumn: "id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "invoices",
                keyColumn: "id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "invoices",
                keyColumn: "id",
                keyValue: 2);

            migrationBuilder.DropColumn(
                name: "billing_address",
                table: "invoices");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "invoices");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "invoices");

            migrationBuilder.DropColumn(
                name: "customer_email",
                table: "invoices");

            migrationBuilder.DropColumn(
                name: "customer_name",
                table: "invoices");

            migrationBuilder.DropColumn(
                name: "due_date",
                table: "invoices");

            migrationBuilder.DropColumn(
                name: "invoice_date",
                table: "invoices");

            migrationBuilder.DropColumn(
                name: "invoice_number",
                table: "invoices");

            migrationBuilder.DropColumn(
                name: "last_modified_by",
                table: "invoices");

            migrationBuilder.DropColumn(
                name: "notes",
                table: "invoices");

            migrationBuilder.DropColumn(
                name: "refund_amount",
                table: "invoices");

            migrationBuilder.DropColumn(
                name: "status",
                table: "invoices");

            migrationBuilder.DropColumn(
                name: "subtotal",
                table: "invoices");

            migrationBuilder.DropColumn(
                name: "tax_amount",
                table: "invoices");

            migrationBuilder.DropColumn(
                name: "total_amount",
                table: "invoices");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "invoice_lines");

            migrationBuilder.DropColumn(
                name: "description",
                table: "invoice_lines");

            migrationBuilder.DropColumn(
                name: "last_modified_by",
                table: "invoice_lines");

            migrationBuilder.DropColumn(
                name: "line_total",
                table: "invoice_lines");

            migrationBuilder.DropColumn(
                name: "product_code",
                table: "invoice_lines");

            migrationBuilder.DropColumn(
                name: "refund_amount",
                table: "invoice_lines");

            migrationBuilder.DropColumn(
                name: "tax_amount",
                table: "invoice_lines");

            migrationBuilder.DropColumn(
                name: "tax_rate",
                table: "invoice_lines");

            migrationBuilder.DropColumn(
                name: "unit",
                table: "invoice_lines");

            migrationBuilder.DropColumn(
                name: "unit_price",
                table: "invoice_lines");

            migrationBuilder.EnsureSchema(
                name: "tms");

            migrationBuilder.RenameTable(
                name: "invoices",
                newName: "hss_invoice",
                newSchema: "tms");

            migrationBuilder.RenameTable(
                name: "invoice_lines",
                newName: "hss_invoiceline",
                newSchema: "tms");

            migrationBuilder.RenameColumn(
                name: "last_modified_at",
                schema: "tms",
                table: "hss_invoice",
                newName: "processed_date");

            migrationBuilder.RenameColumn(
                name: "id",
                schema: "tms",
                table: "hss_invoice",
                newName: "pr_no");

            migrationBuilder.RenameColumn(
                name: "quantity",
                schema: "tms",
                table: "hss_invoiceline",
                newName: "stock_no");

            migrationBuilder.RenameColumn(
                name: "last_modified_at",
                schema: "tms",
                table: "hss_invoiceline",
                newName: "processed_date");

            migrationBuilder.RenameColumn(
                name: "id",
                schema: "tms",
                table: "hss_invoiceline",
                newName: "invoice_line_id");

            migrationBuilder.RenameIndex(
                name: "IX_invoice_lines_invoice_id",
                schema: "tms",
                table: "hss_invoiceline",
                newName: "IX_InvoiceLine_InvoiceId");

            migrationBuilder.AlterColumn<int>(
                name: "pr_no",
                schema: "tms",
                table: "hss_invoice",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<string>(
                name: "invoice_id",
                schema: "tms",
                table: "hss_invoice",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "azure_function_run_id",
                schema: "tms",
                table: "hss_invoice",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "cnrteam_flag",
                schema: "tms",
                table: "hss_invoice",
                type: "character varying(1)",
                maxLength: 1,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "contract_code",
                schema: "tms",
                table: "hss_invoice",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "contract_no",
                schema: "tms",
                table: "hss_invoice",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "cpcomplaint_code",
                schema: "tms",
                table: "hss_invoice",
                type: "character varying(8)",
                maxLength: 8,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "cust_code",
                schema: "tms",
                table: "hss_invoice",
                type: "character varying(8)",
                maxLength: 8,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<float>(
                name: "cust_discount",
                schema: "tms",
                table: "hss_invoice",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "dw_percent",
                schema: "tms",
                table: "hss_invoice",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<DateTime>(
                name: "invoice_createdate",
                schema: "tms",
                table: "hss_invoice",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "invoice_enddate",
                schema: "tms",
                table: "hss_invoice",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "invoice_offhire",
                schema: "tms",
                table: "hss_invoice",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<string>(
                name: "invoice_order",
                schema: "tms",
                table: "hss_invoice",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<short>(
                name: "invoice_seqno",
                schema: "tms",
                table: "hss_invoice",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<string>(
                name: "invoice_slcode",
                schema: "tms",
                table: "hss_invoice",
                type: "character varying(8)",
                maxLength: 8,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "invoice_sldate",
                schema: "tms",
                table: "hss_invoice",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "invoice_startdate",
                schema: "tms",
                table: "hss_invoice",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "invoice_value",
                schema: "tms",
                table: "hss_invoice",
                type: "numeric(14,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "invoice_vat",
                schema: "tms",
                table: "hss_invoice",
                type: "numeric(14,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "invoicestatus_code",
                schema: "tms",
                table: "hss_invoice",
                type: "character varying(1)",
                maxLength: 1,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "invoicetype_code",
                schema: "tms",
                table: "hss_invoice",
                type: "character varying(1)",
                maxLength: 1,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<short>(
                name: "period_no",
                schema: "tms",
                table: "hss_invoice",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<string>(
                name: "processed_by",
                schema: "tms",
                table: "hss_invoice",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "tmsbooking_no",
                schema: "tms",
                table: "hss_invoice",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "unit_no_control",
                schema: "tms",
                table: "hss_invoice",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "unit_no_income",
                schema: "tms",
                table: "hss_invoice",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AlterColumn<string>(
                name: "invoice_id",
                schema: "tms",
                table: "hss_invoiceline",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<string>(
                name: "contract_code",
                schema: "tms",
                table: "hss_invoiceline",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<short>(
                name: "contractline_no",
                schema: "tms",
                table: "hss_invoiceline",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<int>(
                name: "duration_no_current",
                schema: "tms",
                table: "hss_invoiceline",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "duration_no_full",
                schema: "tms",
                table: "hss_invoiceline",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "invoice_finalflag",
                schema: "tms",
                table: "hss_invoiceline",
                type: "character varying(1)",
                maxLength: 1,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<short>(
                name: "invoice_seqno",
                schema: "tms",
                table: "hss_invoiceline",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<decimal>(
                name: "invoiceline_autocharge",
                schema: "tms",
                table: "hss_invoiceline",
                type: "numeric(14,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "invoiceline_basiccharge",
                schema: "tms",
                table: "hss_invoiceline",
                type: "numeric(14,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "invoiceline_charge",
                schema: "tms",
                table: "hss_invoiceline",
                type: "numeric(14,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "invoiceline_createflag",
                schema: "tms",
                table: "hss_invoiceline",
                type: "character varying(1)",
                maxLength: 1,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "invoiceline_deliverydate",
                schema: "tms",
                table: "hss_invoiceline",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "invoiceline_deliveryno",
                schema: "tms",
                table: "hss_invoiceline",
                type: "character varying(15)",
                maxLength: 15,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<float>(
                name: "invoiceline_discount",
                schema: "tms",
                table: "hss_invoiceline",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<string>(
                name: "invoiceline_discountflag",
                schema: "tms",
                table: "hss_invoiceline",
                type: "character varying(1)",
                maxLength: 1,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<short>(
                name: "invoiceline_lineno",
                schema: "tms",
                table: "hss_invoiceline",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<decimal>(
                name: "invoiceline_offhirediscount",
                schema: "tms",
                table: "hss_invoiceline",
                type: "numeric(14,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "invoiceline_offhireexclude",
                schema: "tms",
                table: "hss_invoiceline",
                type: "character varying(1)",
                maxLength: 1,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "invoiceline_periodflag",
                schema: "tms",
                table: "hss_invoiceline",
                type: "character varying(1)",
                maxLength: 1,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "invoiceline_preeditcharge",
                schema: "tms",
                table: "hss_invoiceline",
                type: "numeric(14,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "invoiceline_price",
                schema: "tms",
                table: "hss_invoiceline",
                type: "numeric(14,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "invoiceline_qty",
                schema: "tms",
                table: "hss_invoiceline",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "nomtype_code",
                schema: "tms",
                table: "hss_invoiceline",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "stocktype_code",
                schema: "tms",
                table: "hss_invoiceline",
                type: "character varying(1)",
                maxLength: 1,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "tmsbookingline_no",
                schema: "tms",
                table: "hss_invoiceline",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "vat_percent",
                schema: "tms",
                table: "hss_invoiceline",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "vatreverse_percent",
                schema: "tms",
                table: "hss_invoiceline",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddPrimaryKey(
                name: "PK_hss_invoice",
                schema: "tms",
                table: "hss_invoice",
                column: "invoice_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_hss_invoiceline",
                schema: "tms",
                table: "hss_invoiceline",
                column: "invoice_line_id");

            migrationBuilder.AddForeignKey(
                name: "FK_hss_invoiceline_hss_invoice_invoice_id",
                schema: "tms",
                table: "hss_invoiceline",
                column: "invoice_id",
                principalSchema: "tms",
                principalTable: "hss_invoice",
                principalColumn: "invoice_id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_hss_invoiceline_hss_invoice_invoice_id",
                schema: "tms",
                table: "hss_invoiceline");

            migrationBuilder.DropPrimaryKey(
                name: "PK_hss_invoiceline",
                schema: "tms",
                table: "hss_invoiceline");

            migrationBuilder.DropPrimaryKey(
                name: "PK_hss_invoice",
                schema: "tms",
                table: "hss_invoice");

            migrationBuilder.DropColumn(
                name: "contract_code",
                schema: "tms",
                table: "hss_invoiceline");

            migrationBuilder.DropColumn(
                name: "contractline_no",
                schema: "tms",
                table: "hss_invoiceline");

            migrationBuilder.DropColumn(
                name: "duration_no_current",
                schema: "tms",
                table: "hss_invoiceline");

            migrationBuilder.DropColumn(
                name: "duration_no_full",
                schema: "tms",
                table: "hss_invoiceline");

            migrationBuilder.DropColumn(
                name: "invoice_finalflag",
                schema: "tms",
                table: "hss_invoiceline");

            migrationBuilder.DropColumn(
                name: "invoice_seqno",
                schema: "tms",
                table: "hss_invoiceline");

            migrationBuilder.DropColumn(
                name: "invoiceline_autocharge",
                schema: "tms",
                table: "hss_invoiceline");

            migrationBuilder.DropColumn(
                name: "invoiceline_basiccharge",
                schema: "tms",
                table: "hss_invoiceline");

            migrationBuilder.DropColumn(
                name: "invoiceline_charge",
                schema: "tms",
                table: "hss_invoiceline");

            migrationBuilder.DropColumn(
                name: "invoiceline_createflag",
                schema: "tms",
                table: "hss_invoiceline");

            migrationBuilder.DropColumn(
                name: "invoiceline_deliverydate",
                schema: "tms",
                table: "hss_invoiceline");

            migrationBuilder.DropColumn(
                name: "invoiceline_deliveryno",
                schema: "tms",
                table: "hss_invoiceline");

            migrationBuilder.DropColumn(
                name: "invoiceline_discount",
                schema: "tms",
                table: "hss_invoiceline");

            migrationBuilder.DropColumn(
                name: "invoiceline_discountflag",
                schema: "tms",
                table: "hss_invoiceline");

            migrationBuilder.DropColumn(
                name: "invoiceline_lineno",
                schema: "tms",
                table: "hss_invoiceline");

            migrationBuilder.DropColumn(
                name: "invoiceline_offhirediscount",
                schema: "tms",
                table: "hss_invoiceline");

            migrationBuilder.DropColumn(
                name: "invoiceline_offhireexclude",
                schema: "tms",
                table: "hss_invoiceline");

            migrationBuilder.DropColumn(
                name: "invoiceline_periodflag",
                schema: "tms",
                table: "hss_invoiceline");

            migrationBuilder.DropColumn(
                name: "invoiceline_preeditcharge",
                schema: "tms",
                table: "hss_invoiceline");

            migrationBuilder.DropColumn(
                name: "invoiceline_price",
                schema: "tms",
                table: "hss_invoiceline");

            migrationBuilder.DropColumn(
                name: "invoiceline_qty",
                schema: "tms",
                table: "hss_invoiceline");

            migrationBuilder.DropColumn(
                name: "nomtype_code",
                schema: "tms",
                table: "hss_invoiceline");

            migrationBuilder.DropColumn(
                name: "stocktype_code",
                schema: "tms",
                table: "hss_invoiceline");

            migrationBuilder.DropColumn(
                name: "tmsbookingline_no",
                schema: "tms",
                table: "hss_invoiceline");

            migrationBuilder.DropColumn(
                name: "vat_percent",
                schema: "tms",
                table: "hss_invoiceline");

            migrationBuilder.DropColumn(
                name: "vatreverse_percent",
                schema: "tms",
                table: "hss_invoiceline");

            migrationBuilder.DropColumn(
                name: "invoice_id",
                schema: "tms",
                table: "hss_invoice");

            migrationBuilder.DropColumn(
                name: "azure_function_run_id",
                schema: "tms",
                table: "hss_invoice");

            migrationBuilder.DropColumn(
                name: "cnrteam_flag",
                schema: "tms",
                table: "hss_invoice");

            migrationBuilder.DropColumn(
                name: "contract_code",
                schema: "tms",
                table: "hss_invoice");

            migrationBuilder.DropColumn(
                name: "contract_no",
                schema: "tms",
                table: "hss_invoice");

            migrationBuilder.DropColumn(
                name: "cpcomplaint_code",
                schema: "tms",
                table: "hss_invoice");

            migrationBuilder.DropColumn(
                name: "cust_code",
                schema: "tms",
                table: "hss_invoice");

            migrationBuilder.DropColumn(
                name: "cust_discount",
                schema: "tms",
                table: "hss_invoice");

            migrationBuilder.DropColumn(
                name: "dw_percent",
                schema: "tms",
                table: "hss_invoice");

            migrationBuilder.DropColumn(
                name: "invoice_createdate",
                schema: "tms",
                table: "hss_invoice");

            migrationBuilder.DropColumn(
                name: "invoice_enddate",
                schema: "tms",
                table: "hss_invoice");

            migrationBuilder.DropColumn(
                name: "invoice_offhire",
                schema: "tms",
                table: "hss_invoice");

            migrationBuilder.DropColumn(
                name: "invoice_order",
                schema: "tms",
                table: "hss_invoice");

            migrationBuilder.DropColumn(
                name: "invoice_seqno",
                schema: "tms",
                table: "hss_invoice");

            migrationBuilder.DropColumn(
                name: "invoice_slcode",
                schema: "tms",
                table: "hss_invoice");

            migrationBuilder.DropColumn(
                name: "invoice_sldate",
                schema: "tms",
                table: "hss_invoice");

            migrationBuilder.DropColumn(
                name: "invoice_startdate",
                schema: "tms",
                table: "hss_invoice");

            migrationBuilder.DropColumn(
                name: "invoice_value",
                schema: "tms",
                table: "hss_invoice");

            migrationBuilder.DropColumn(
                name: "invoice_vat",
                schema: "tms",
                table: "hss_invoice");

            migrationBuilder.DropColumn(
                name: "invoicestatus_code",
                schema: "tms",
                table: "hss_invoice");

            migrationBuilder.DropColumn(
                name: "invoicetype_code",
                schema: "tms",
                table: "hss_invoice");

            migrationBuilder.DropColumn(
                name: "period_no",
                schema: "tms",
                table: "hss_invoice");

            migrationBuilder.DropColumn(
                name: "processed_by",
                schema: "tms",
                table: "hss_invoice");

            migrationBuilder.DropColumn(
                name: "tmsbooking_no",
                schema: "tms",
                table: "hss_invoice");

            migrationBuilder.DropColumn(
                name: "unit_no_control",
                schema: "tms",
                table: "hss_invoice");

            migrationBuilder.DropColumn(
                name: "unit_no_income",
                schema: "tms",
                table: "hss_invoice");

            migrationBuilder.RenameTable(
                name: "hss_invoiceline",
                schema: "tms",
                newName: "invoice_lines");

            migrationBuilder.RenameTable(
                name: "hss_invoice",
                schema: "tms",
                newName: "invoices");

            migrationBuilder.RenameColumn(
                name: "stock_no",
                table: "invoice_lines",
                newName: "quantity");

            migrationBuilder.RenameColumn(
                name: "processed_date",
                table: "invoice_lines",
                newName: "last_modified_at");

            migrationBuilder.RenameColumn(
                name: "invoice_line_id",
                table: "invoice_lines",
                newName: "id");

            migrationBuilder.RenameIndex(
                name: "IX_InvoiceLine_InvoiceId",
                table: "invoice_lines",
                newName: "IX_invoice_lines_invoice_id");

            migrationBuilder.RenameColumn(
                name: "processed_date",
                table: "invoices",
                newName: "last_modified_at");

            migrationBuilder.RenameColumn(
                name: "pr_no",
                table: "invoices",
                newName: "id");

            migrationBuilder.AlterColumn<int>(
                name: "invoice_id",
                table: "invoice_lines",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "invoice_lines",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "invoice_lines",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "last_modified_by",
                table: "invoice_lines",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "line_total",
                table: "invoice_lines",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "product_code",
                table: "invoice_lines",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "refund_amount",
                table: "invoice_lines",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "tax_amount",
                table: "invoice_lines",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "tax_rate",
                table: "invoice_lines",
                type: "numeric(5,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "unit",
                table: "invoice_lines",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "unit_price",
                table: "invoice_lines",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "invoices",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<string>(
                name: "billing_address",
                table: "invoices",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "invoices",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "created_by",
                table: "invoices",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "customer_email",
                table: "invoices",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "customer_name",
                table: "invoices",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "due_date",
                table: "invoices",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "invoice_date",
                table: "invoices",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "invoice_number",
                table: "invoices",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "last_modified_by",
                table: "invoices",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "notes",
                table: "invoices",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "refund_amount",
                table: "invoices",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "status",
                table: "invoices",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "subtotal",
                table: "invoices",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "tax_amount",
                table: "invoices",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "total_amount",
                table: "invoices",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddPrimaryKey(
                name: "PK_invoice_lines",
                table: "invoice_lines",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_invoices",
                table: "invoices",
                column: "id");

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

            migrationBuilder.AddForeignKey(
                name: "FK_invoice_lines_invoices_invoice_id",
                table: "invoice_lines",
                column: "invoice_id",
                principalTable: "invoices",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HamamPos.Api.Migrations
{
    /// <inheritdoc />
    public partial class Initial_Pos_Tables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OpenedByUserId",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "Capacity",
                table: "ServiceUnits");

            migrationBuilder.DropColumn(
                name: "RefNo",
                table: "Payments");

            migrationBuilder.RenameColumn(
                name: "OpenedAt",
                table: "Tickets",
                newName: "OpenedBy");

            migrationBuilder.RenameColumn(
                name: "ClosedAt",
                table: "Tickets",
                newName: "ClosedAtUtc");

            migrationBuilder.RenameColumn(
                name: "Qty",
                table: "TicketItems",
                newName: "Quantity");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "ServiceUnits",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "PaidAt",
                table: "Payments",
                newName: "CollectedBy");

            migrationBuilder.RenameColumn(
                name: "Method",
                table: "Payments",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "OpenedAt",
                table: "DaySessions",
                newName: "OpenedBy");

            migrationBuilder.RenameColumn(
                name: "ClosedAt",
                table: "DaySessions",
                newName: "ClosedBy");

            migrationBuilder.AddColumn<DateTime>(
                name: "OpenedAtUtc",
                table: "Tickets",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "TicketItems",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "Products",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CollectedAtUtc",
                table: "Payments",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ClosedAtUtc",
                table: "DaySessions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "OpenedAtUtc",
                table: "DaySessions",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_Payments_TicketId",
                table: "Payments",
                column: "TicketId");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Tickets_TicketId",
                table: "Payments",
                column: "TicketId",
                principalTable: "Tickets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Tickets_TicketId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_TicketId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "OpenedAtUtc",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "TicketItems");

            migrationBuilder.DropColumn(
                name: "CollectedAtUtc",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "ClosedAtUtc",
                table: "DaySessions");

            migrationBuilder.DropColumn(
                name: "OpenedAtUtc",
                table: "DaySessions");

            migrationBuilder.RenameColumn(
                name: "OpenedBy",
                table: "Tickets",
                newName: "OpenedAt");

            migrationBuilder.RenameColumn(
                name: "ClosedAtUtc",
                table: "Tickets",
                newName: "ClosedAt");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "TicketItems",
                newName: "Qty");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "ServiceUnits",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Payments",
                newName: "Method");

            migrationBuilder.RenameColumn(
                name: "CollectedBy",
                table: "Payments",
                newName: "PaidAt");

            migrationBuilder.RenameColumn(
                name: "OpenedBy",
                table: "DaySessions",
                newName: "OpenedAt");

            migrationBuilder.RenameColumn(
                name: "ClosedBy",
                table: "DaySessions",
                newName: "ClosedAt");

            migrationBuilder.AddColumn<int>(
                name: "OpenedByUserId",
                table: "Tickets",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Tickets",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Capacity",
                table: "ServiceUnits",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "Products",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddColumn<string>(
                name: "RefNo",
                table: "Payments",
                type: "TEXT",
                nullable: true);
        }
    }
}

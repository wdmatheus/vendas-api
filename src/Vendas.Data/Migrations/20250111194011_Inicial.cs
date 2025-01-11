using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Vendas.Core.Entidades;

#nullable disable

namespace Vendas.Data.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:status_venda", "cancelada,criada");

            migrationBuilder.CreateTable(
                name: "vendas",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    id_externo = table.Column<Guid>(type: "uuid", nullable: false),
                    numero = table.Column<string>(type: "varchar", nullable: false),
                    valor_total_sem_desconto = table.Column<decimal>(type: "numeric(14,2)", nullable: false),
                    valor_total = table.Column<decimal>(type: "numeric(14,2)", nullable: false),
                    desconto = table.Column<decimal>(type: "numeric(14,2)", nullable: false),
                    status = table.Column<StatusVenda>(type: "status_venda", nullable: false),
                    data = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    criada_em = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    alterada_em = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    cliente_id_externo = table.Column<Guid>(type: "uuid", nullable: false),
                    cliente_nome = table.Column<string>(type: "varchar", nullable: false),
                    cliente_cnpj_numero = table.Column<string>(type: "varchar(14)", nullable: false),
                    filial_id_externo = table.Column<Guid>(type: "uuid", nullable: false),
                    filial_nome = table.Column<string>(type: "varchar", nullable: false),
                    filial_numero = table.Column<string>(type: "varchar", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_vendas", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "items_venda",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    quantidade = table.Column<decimal>(type: "numeric(14,2)", nullable: false),
                    valor_unitario = table.Column<decimal>(type: "numeric(14,2)", nullable: false),
                    valor_total_sem_desconto = table.Column<decimal>(type: "numeric(14,2)", nullable: false),
                    valor_total = table.Column<decimal>(type: "numeric(14,2)", nullable: false),
                    desconto = table.Column<decimal>(type: "numeric(14,2)", nullable: false),
                    venda_id = table.Column<Guid>(type: "uuid", nullable: false),
                    produto_codigo = table.Column<string>(type: "varchar", nullable: false),
                    produto_descricao = table.Column<string>(type: "varchar", nullable: false),
                    produto_ean = table.Column<string>(type: "varchar", nullable: false),
                    produto_id_externo = table.Column<Guid>(type: "uuid", nullable: false),
                    produto_nome = table.Column<string>(type: "varchar", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_items_venda", x => x.id);
                    table.ForeignKey(
                        name: "fk_items_venda_vendas_venda_id",
                        column: x => x.venda_id,
                        principalSchema: "public",
                        principalTable: "vendas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_items_venda_venda_id",
                schema: "public",
                table: "items_venda",
                column: "venda_id");

            migrationBuilder.CreateIndex(
                name: "ix_vendas_criada_em",
                schema: "public",
                table: "vendas",
                column: "criada_em");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "items_venda",
                schema: "public");

            migrationBuilder.DropTable(
                name: "vendas",
                schema: "public");
        }
    }
}

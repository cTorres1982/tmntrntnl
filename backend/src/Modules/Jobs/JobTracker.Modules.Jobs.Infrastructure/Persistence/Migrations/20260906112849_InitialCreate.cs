using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobTracker.Modules.Jobs.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "jobs");

            migrationBuilder.CreateTable(
                name: "assignees",
                schema: "jobs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_assignees", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "customers",
                schema: "jobs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_customers", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "outbox_messages",
                schema: "jobs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    content = table.Column<string>(type: "jsonb", nullable: false),
                    occurred_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    processed_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    error = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_outbox_messages", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "jobs",
                schema: "jobs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    street = table.Column<string>(type: "text", nullable: false),
                    city = table.Column<string>(type: "text", nullable: false),
                    state = table.Column<string>(type: "text", nullable: false),
                    zip_code = table.Column<string>(type: "text", nullable: false),
                    latitude = table.Column<double>(type: "double precision", nullable: false),
                    longitude = table.Column<double>(type: "double precision", nullable: false),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    scheduled_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    assignee_id = table.Column<Guid>(type: "uuid", nullable: true),
                    customer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    started_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    completed_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    cancelled_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    cancellation_reason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    signature_url = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_jobs", x => x.id);
                    table.ForeignKey(
                        name: "fk_jobs_assignees_assignee_id",
                        column: x => x.assignee_id,
                        principalSchema: "jobs",
                        principalTable: "assignees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_jobs_customers_customer_id",
                        column: x => x.customer_id,
                        principalSchema: "jobs",
                        principalTable: "customers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "job_photos",
                schema: "jobs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    url = table.Column<string>(type: "text", nullable: false),
                    captured_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    caption = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    job_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_job_photos", x => x.id);
                    table.ForeignKey(
                        name: "fk_job_photos_jobs_job_id",
                        column: x => x.job_id,
                        principalSchema: "jobs",
                        principalTable: "jobs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_assignees_organization_id",
                schema: "jobs",
                table: "assignees",
                column: "organization_id");

            migrationBuilder.CreateIndex(
                name: "ix_customers_organization_id",
                schema: "jobs",
                table: "customers",
                column: "organization_id");

            migrationBuilder.CreateIndex(
                name: "ix_job_photos_job_id",
                schema: "jobs",
                table: "job_photos",
                column: "job_id");

            migrationBuilder.CreateIndex(
                name: "ix_jobs_assignee_id",
                schema: "jobs",
                table: "jobs",
                column: "assignee_id");

            migrationBuilder.CreateIndex(
                name: "ix_jobs_customer_id",
                schema: "jobs",
                table: "jobs",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "ix_jobs_organization_id",
                schema: "jobs",
                table: "jobs",
                column: "organization_id");

            migrationBuilder.CreateIndex(
                name: "ix_jobs_organization_id_scheduled_date",
                schema: "jobs",
                table: "jobs",
                columns: new[] { "organization_id", "scheduled_date" });

            migrationBuilder.CreateIndex(
                name: "ix_jobs_organization_id_status",
                schema: "jobs",
                table: "jobs",
                columns: new[] { "organization_id", "status" });

            migrationBuilder.CreateIndex(
                name: "ix_outbox_messages_processed_on_utc",
                schema: "jobs",
                table: "outbox_messages",
                column: "processed_on_utc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "job_photos",
                schema: "jobs");

            migrationBuilder.DropTable(
                name: "outbox_messages",
                schema: "jobs");

            migrationBuilder.DropTable(
                name: "jobs",
                schema: "jobs");

            migrationBuilder.DropTable(
                name: "assignees",
                schema: "jobs");

            migrationBuilder.DropTable(
                name: "customers",
                schema: "jobs");
        }
    }
}

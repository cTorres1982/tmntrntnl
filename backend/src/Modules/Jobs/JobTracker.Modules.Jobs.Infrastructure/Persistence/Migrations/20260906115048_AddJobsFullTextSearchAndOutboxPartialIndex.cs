using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobTracker.Modules.Jobs.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddJobsFullTextSearchAndOutboxPartialIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_outbox_messages_processed_on_utc",
                schema: "jobs",
                table: "outbox_messages");

            migrationBuilder.CreateIndex(
                name: "ix_outbox_messages_unprocessed",
                schema: "jobs",
                table: "outbox_messages",
                column: "occurred_on_utc",
                filter: "processed_on_utc IS NULL");

            // A generated tsvector column isn't expressible as a mapped C# property
            // without leaking a Postgres-specific concern into the Job aggregate, so
            // it's hand-written here as raw SQL rather than via the Domain model.
            migrationBuilder.Sql(
                """
                ALTER TABLE jobs.jobs
                    ADD COLUMN search_vector tsvector
                    GENERATED ALWAYS AS (
                        setweight(to_tsvector('english', coalesce(title, '')), 'A') ||
                        setweight(to_tsvector('english', coalesce(description, '')), 'B')
                    ) STORED;
                """);

            migrationBuilder.Sql(
                "CREATE INDEX ix_jobs_search_vector ON jobs.jobs USING GIN (search_vector);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP INDEX IF EXISTS jobs.ix_jobs_search_vector;");
            migrationBuilder.Sql("ALTER TABLE jobs.jobs DROP COLUMN IF EXISTS search_vector;");

            migrationBuilder.DropIndex(
                name: "ix_outbox_messages_unprocessed",
                schema: "jobs",
                table: "outbox_messages");

            migrationBuilder.CreateIndex(
                name: "ix_outbox_messages_processed_on_utc",
                schema: "jobs",
                table: "outbox_messages",
                column: "processed_on_utc");
        }
    }
}

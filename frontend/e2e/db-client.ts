import { Client, type ClientConfig } from "pg";

/**
 * Same environment variable the backend's `dotnet ef` commands use throughout
 * this project (see backend/src/Modules/Jobs/.../JobsDbContextFactory.cs) —
 * no hardcoded fallback, for the same reason: a missing connection string
 * should fail loudly, not silently guess a password. The value is an
 * Npgsql-style string ("Host=...;Port=...;Database=...;Username=...;
 * Password=..."), which `pg` doesn't parse natively (it expects a
 * postgres:// URI or a config object) — parsed into a ClientConfig below.
 */
export async function connectToTestDatabase(): Promise<Client> {
  const connectionString = process.env.JOBTRACKER_CONNECTION_STRING;
  if (!connectionString) {
    throw new Error(
      "Set the JOBTRACKER_CONNECTION_STRING environment variable before running the e2e suite " +
        "(same connection string used for `dotnet ef` commands against the backend).",
    );
  }

  const client = new Client(parseNpgsqlConnectionString(connectionString));
  await client.connect();
  return client;
}

function parseNpgsqlConnectionString(connectionString: string): ClientConfig {
  const pairs = Object.fromEntries(
    connectionString
      .split(";")
      .filter(Boolean)
      .map((pair) => {
        const [key, ...rest] = pair.split("=");
        return [key.trim().toLowerCase(), rest.join("=").trim()];
      }),
  );

  return {
    host: pairs.host,
    port: pairs.port ? Number(pairs.port) : undefined,
    database: pairs.database,
    user: pairs.username ?? pairs["user id"],
    password: pairs.password,
  };
}

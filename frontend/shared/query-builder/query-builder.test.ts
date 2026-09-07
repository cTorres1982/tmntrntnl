import { expect, expectTypeOf, test } from "vitest";
import { QueryBuilder } from "./query-builder";

interface Job {
  id: string;
  title: string;
  status: "draft" | "scheduled" | "in_progress" | "completed" | "cancelled";
  photoCount: number;
}

test("matches the AC example exactly: select -> where -> orderBy -> limit -> build", () => {
  const result = QueryBuilder.create<Job>()
    .select("id", "title", "status")
    .where("status", "eq", "completed")
    .orderBy("title", "asc")
    .limit(10)
    .build();

  expectTypeOf(result.query).toEqualTypeOf<"SELECT id, title, status WHERE status = completed ORDER BY title ASC LIMIT 10">();
  expect(result.query).toBe("SELECT id, title, status WHERE status = completed ORDER BY title ASC LIMIT 10");
  expect(result.params).toEqual(["completed"]);
});

test("where's value must be assignable to the field's own type", () => {
  QueryBuilder.create<Job>()
    .select("id", "photoCount")
    // @ts-expect-error photoCount is a number, not a Job status string
    .where("photoCount", "eq", "completed");
});

test("where/orderBy only accept fields that survived select's narrowing", () => {
  QueryBuilder.create<Job>()
    .select("id", "title")
    // @ts-expect-error "status" was not selected, so it isn't a valid field here
    .where("status", "eq", "completed");
});

test("orderBy rejects a field that was not selected", () => {
  QueryBuilder.create<Job>()
    .select("id", "title")
    // @ts-expect-error "status" was not selected
    .orderBy("status", "asc");
});

test("build() before select still narrows the query string to whatever was chained", () => {
  const result = QueryBuilder.create<Job>().limit(5).build();

  expectTypeOf(result.query).toEqualTypeOf<" LIMIT 5">();
  expect(result.query).toBe(" LIMIT 5");
});

import { expectTypeOf, test } from "vitest";
import type { PathKeys } from "./path-keys.type";

test("PathKeys matches the AC example exactly", () => {
  expectTypeOf<PathKeys<{ a: { b: string; c: { d: number } } }>>().toEqualTypeOf<"a.b" | "a.c.d">();
});

test("PathKeys treats a top-level primitive property as its own leaf path", () => {
  expectTypeOf<PathKeys<{ id: string; nested: { value: number } }>>().toEqualTypeOf<"id" | "nested.value">();
});

test("PathKeys treats arrays and Dates as leaves rather than recursing into them", () => {
  expectTypeOf<PathKeys<{ tags: string[]; createdAt: Date }>>().toEqualTypeOf<"tags" | "createdAt">();
});

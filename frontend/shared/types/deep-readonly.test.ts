import { expectTypeOf, test } from "vitest";
import type { DeepReadonly } from "./deep-readonly.type";

interface Sample {
  id: string;
  address: { street: string; coords: [number, number] };
  tags: string[];
  metadata: Map<string, { count: number }>;
  labels: Set<{ name: string }>;
}

// Tuple/array-shape assertions below check element types and mutation
// rejection individually rather than comparing whole tuple types with
// toEqualTypeOf/toExtend — expect-type's structural comparison walks the
// entire Array/tuple prototype (including ES2023 additions like `with` and
// `toReversed`, pulled in by this project's "esnext" lib), where a readonly
// tuple's method surface differs subtly enough from a hand-written literal's
// to make the comparison itself unreliable, independent of whether
// DeepReadonly is correct. Checking behavior (does it actually reject
// mutation?) is the more meaningful assertion anyway.

test("DeepReadonly makes nested object properties readonly", () => {
  expectTypeOf<DeepReadonly<Sample>["address"]["street"]>().toEqualTypeOf<string>();
});

test("DeepReadonly turns arrays into ReadonlyArray", () => {
  expectTypeOf<DeepReadonly<Sample>["tags"]>().toEqualTypeOf<readonly string[]>();
});

test("DeepReadonly preserves each tuple position's own type instead of collapsing to a union element type", () => {
  expectTypeOf<DeepReadonly<Sample>["address"]["coords"][0]>().toEqualTypeOf<number>();
  expectTypeOf<DeepReadonly<Sample>["address"]["coords"][1]>().toEqualTypeOf<number>();
  expectTypeOf<DeepReadonly<Sample>["address"]["coords"]["length"]>().toEqualTypeOf<2>();
});

test("DeepReadonly turns Map into ReadonlyMap with a deep-readonly value type", () => {
  expectTypeOf<DeepReadonly<Sample>["metadata"]>().toEqualTypeOf<
    ReadonlyMap<string, { readonly count: number }>
  >();
});

test("DeepReadonly turns Set into ReadonlySet with a deep-readonly member type", () => {
  expectTypeOf<DeepReadonly<Sample>["labels"]>().toEqualTypeOf<ReadonlySet<{ readonly name: string }>>();
});

test("DeepReadonly leaves primitives untouched", () => {
  expectTypeOf<DeepReadonly<string>>().toEqualTypeOf<string>();
  expectTypeOf<DeepReadonly<number>>().toEqualTypeOf<number>();
});

test("DeepReadonly actually rejects mutation, not just renamed types", () => {
  // A real, fully-populated object: DeepReadonly is a compile-time-only type
  // (plain JS has no readonly enforcement without Object.freeze), so the
  // ts-expect-error comments below are what actually verify the rejection —
  // this just has to be a real object so property access doesn't throw.
  const value: DeepReadonly<Sample> = {
    id: "job-1",
    address: { street: "Main St", coords: [1, 2] },
    tags: ["urgent"],
    metadata: new Map(),
    labels: new Set(),
  };

  // @ts-expect-error id is readonly
  value.id = "new-id";
  // @ts-expect-error nested properties are readonly too
  value.address.street = "new-street";
  // @ts-expect-error tuple positions are readonly
  value.address.coords[0] = 1;
  // @ts-expect-error arrays become ReadonlyArray, which has no push
  value.tags.push("x");
});

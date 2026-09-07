/**
 * Recursively makes every property readonly, including nested objects, arrays,
 * Maps, and Sets. Primitives (and functions) are returned as-is — there is
 * nothing to make readonly about a value with no mutable members.
 */
export type DeepReadonly<T> = T extends (...args: never[]) => unknown
  ? T
  : T extends ReadonlyMap<infer K, infer V>
    ? ReadonlyMap<DeepReadonly<K>, DeepReadonly<V>>
    : T extends ReadonlySet<infer V>
      ? ReadonlySet<DeepReadonly<V>>
      : // Fixed-length tuples are matched before general arrays so their exact
        // shape (and each position's own type) survives instead of collapsing
        // into a single unioned ReadonlyArray element type. The empty-tuple
        // case must resolve to a literal `readonly []`, not fall through to
        // the general array branch below (`ReadonlyArray<never>`) — a fixed
        // empty tuple spread into a rest position terminates the recursion at
        // a fixed length; a rest array spread into one keeps it variadic,
        // which would make every tuple's `.length` widen to `number`.
        T extends readonly []
        ? readonly []
        : T extends readonly [infer Head, ...infer Tail]
          ? readonly [DeepReadonly<Head>, ...DeepReadonly<Tail>]
          : T extends readonly (infer Item)[]
            ? ReadonlyArray<DeepReadonly<Item>>
            : T extends object
              ? { readonly [K in keyof T]: DeepReadonly<T[K]> }
              : T;

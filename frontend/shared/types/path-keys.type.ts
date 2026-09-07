/**
 * Union of dot-notation paths to every leaf property of an object type.
 * A property is a "leaf" when it is not itself a plain object — arrays,
 * Dates, and other class instances count as leaves, not containers to
 * recurse into, since dotting into `list.0` or `date.getTime` is not a
 * meaningful "data path" the way a plain nested object's fields are.
 */
export type PathKeys<T> = T extends object
  ? {
      [K in keyof T & string]: T[K] extends Date | readonly unknown[]
        ? K
        : T[K] extends object
          ? `${K}.${PathKeys<T[K]>}`
          : K;
    }[keyof T & string]
  : never;

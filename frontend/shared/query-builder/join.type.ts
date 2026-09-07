/** Joins a tuple of string literals with `Sep`, entirely at the type level — e.g. `Join<["id", "title"], ", ">` is `"id, title"`. */
export type Join<Parts extends readonly string[], Sep extends string> = Parts extends readonly [
  infer Head extends string,
  ...infer Rest extends string[],
]
  ? Rest extends []
    ? Head
    : `${Head}${Sep}${Join<Rest, Sep>}`
  : "";

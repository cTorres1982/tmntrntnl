import type { ComparisonOperator } from "./comparison-operator.type";
import type { Join } from "./join.type";
import { SQL_OPERATOR, type SqlOperator } from "./sql-operator.type";

/**
 * Type-safe query builder: `Selected` narrows after `.select()` so `.where()`/
 * `.orderBy()` only accept fields that were actually selected, and `Query`
 * accumulates the literal SQL text as a template literal type — `build().query`
 * is typed as the exact string the chain produces, not a generic `string`.
 *
 * Note on `.where()`: a real implementation would inline a `$1`-style
 * placeholder into the query text and rely solely on `params` for the value
 * (protecting against SQL injection). Here the value is embedded directly in
 * the query text too so the template literal type stays a fully concrete,
 * inspectable string end-to-end, which is the point of this exercise — `params`
 * still carries the value, exactly matching the shape `build()` must return.
 */
export class QueryBuilder<T, Selected extends keyof T = keyof T, Query extends string = ""> {
  private constructor(
    private readonly queryText: Query,
    private readonly params: readonly unknown[],
  ) {}

  static create<T>(): QueryBuilder<T, keyof T, ""> {
    return new QueryBuilder("", []);
  }

  select<const Fields extends readonly (keyof T & string)[]>(
    ...fields: Fields
  ): QueryBuilder<T, Fields[number], `SELECT ${Join<Fields, ", ">}`> {
    const query = `SELECT ${fields.join(", ")}` as `SELECT ${Join<Fields, ", ">}`;
    return new QueryBuilder(query, this.params);
  }

  where<Field extends Selected & string, const Op extends ComparisonOperator, const Value extends T[Field]>(
    field: Field,
    operator: Op,
    value: Value,
  ): QueryBuilder<
    T,
    Selected,
    `${Query} WHERE ${Field} ${SqlOperator<Op>} ${Value & (string | number | boolean | bigint | null | undefined)}`
  > {
    type Clause =
      `${Query} WHERE ${Field} ${SqlOperator<Op>} ${Value & (string | number | boolean | bigint | null | undefined)}`;
    const query = `${this.queryText} WHERE ${field} ${SQL_OPERATOR[operator]} ${String(value)}` as Clause;
    return new QueryBuilder(query, [...this.params, value]);
  }

  orderBy<Field extends Selected & string, const Direction extends "asc" | "desc">(
    field: Field,
    direction: Direction,
  ): QueryBuilder<T, Selected, `${Query} ORDER BY ${Field} ${Uppercase<Direction>}`> {
    const query =
      `${this.queryText} ORDER BY ${field} ${direction.toUpperCase()}` as `${Query} ORDER BY ${Field} ${Uppercase<Direction>}`;
    return new QueryBuilder(query, this.params);
  }

  limit<const N extends number>(count: N): QueryBuilder<T, Selected, `${Query} LIMIT ${N}`> {
    const query = `${this.queryText} LIMIT ${count}` as `${Query} LIMIT ${N}`;
    return new QueryBuilder(query, this.params);
  }

  build(): { query: Query; params: unknown[] } {
    return { query: this.queryText, params: [...this.params] };
  }
}

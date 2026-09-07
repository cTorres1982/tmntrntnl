import type { ComparisonOperator } from "./comparison-operator.type";

/** Maps each comparison operator to its literal SQL symbol, at the type level too — this is what lets the generated query string type show `=` instead of `eq`. */
export type SqlOperator<Op extends ComparisonOperator> = {
  eq: "=";
  neq: "<>";
  gt: ">";
  gte: ">=";
  lt: "<";
  lte: "<=";
}[Op];

export const SQL_OPERATOR: { [Op in ComparisonOperator]: SqlOperator<Op> } = {
  eq: "=",
  neq: "<>",
  gt: ">",
  gte: ">=",
  lt: "<",
  lte: "<=",
};

import { defineConfig } from "vitest/config";

export default defineConfig({
  test: {
    environment: "jsdom",
    globals: false,
    include: ["**/*.test.ts", "**/*.test.tsx"],
    exclude: ["node_modules", ".next", "e2e"],
    coverage: {
      provider: "v8",
      reportsDirectory: "./vitest-coverage",
    },
  },
});

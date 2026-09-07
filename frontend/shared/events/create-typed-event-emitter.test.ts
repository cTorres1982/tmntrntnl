import { describe, expect, expectTypeOf, test, vi } from "vitest";
import { createTypedEventEmitter } from "./create-typed-event-emitter";

interface AppEvents {
  "job:created": { jobId: string };
  "job:completed": { jobId: string; completedAt: Date };
}

describe("createTypedEventEmitter", () => {
  test("invokes the handler registered for the emitted event with its payload", () => {
    const emitter = createTypedEventEmitter<AppEvents>();
    const handler = vi.fn();

    emitter.on("job:created", handler);
    emitter.emit("job:created", { jobId: "job-1" });

    expect(handler).toHaveBeenCalledExactlyOnceWith({ jobId: "job-1" });
  });

  test("does not invoke a handler registered for a different event", () => {
    const emitter = createTypedEventEmitter<AppEvents>();
    const createdHandler = vi.fn();
    const completedHandler = vi.fn();

    emitter.on("job:created", createdHandler);
    emitter.on("job:completed", completedHandler);
    emitter.emit("job:created", { jobId: "job-1" });

    expect(completedHandler).not.toHaveBeenCalled();
  });

  test("off removes the exact handler reference so it no longer fires", () => {
    const emitter = createTypedEventEmitter<AppEvents>();
    const handler = vi.fn();

    emitter.on("job:created", handler);
    emitter.off("job:created", handler);
    emitter.emit("job:created", { jobId: "job-1" });

    expect(handler).not.toHaveBeenCalled();
  });

  test("infers the handler's payload type from the event name", () => {
    const emitter = createTypedEventEmitter<AppEvents>();

    emitter.on("job:completed", (payload) => {
      expectTypeOf(payload).toEqualTypeOf<{ jobId: string; completedAt: Date }>();
    });
  });
});

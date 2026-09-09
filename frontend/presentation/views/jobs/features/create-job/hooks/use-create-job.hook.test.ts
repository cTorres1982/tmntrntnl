import { act, renderHook } from "@testing-library/react";
import { beforeEach, describe, expect, test, vi } from "vitest";
import { createJobAction } from "@/app/jobs/actions";
import { useCreateJob } from "./use-create-job.hook";

vi.mock("@/app/jobs/actions", () => ({
  createJobAction: vi.fn(),
}));

const createJobActionMock = vi.mocked(createJobAction);
const VALID_CUSTOMER_ID = "11111111-1111-1111-1111-111111111111";

function submitEvent() {
  return { preventDefault: vi.fn() } as unknown as React.FormEvent<HTMLFormElement>;
}

/** Latitude/longitude/customerId are all validated before submit — most tests aren't about that validation, so this fills in values that pass it. */
function fillRequiredValidFields(result: { current: ReturnType<typeof useCreateJob> }) {
  act(() => result.current.onFieldChange("latitude", "30.27"));
  act(() => result.current.onFieldChange("longitude", "-97.74"));
  act(() => result.current.onFieldChange("customerId", VALID_CUSTOMER_ID));
}

describe("useCreateJob", () => {
  beforeEach(() => {
    createJobActionMock.mockReset();
  });

  test("the reducer updates only the changed field, leaving the rest of the form intact", () => {
    const { result } = renderHook(() => useCreateJob(vi.fn()));

    act(() => result.current.onFieldChange("title", "Roof repair"));
    act(() => result.current.onFieldChange("city", "Austin"));

    expect(result.current.fields.title).toBe("Roof repair");
    expect(result.current.fields.city).toBe("Austin");
    expect(result.current.fields.description).toBe("");
  });

  test("submitting calls createJobAction with the current field values, coercing lat/lng to numbers", async () => {
    createJobActionMock.mockResolvedValue({ success: true });
    const onCreated = vi.fn();
    const { result } = renderHook(() => useCreateJob(onCreated));

    act(() => result.current.onFieldChange("title", "Roof repair"));
    act(() => result.current.onFieldChange("description", "Full replacement"));
    act(() => result.current.onFieldChange("street", "123 Main St"));
    act(() => result.current.onFieldChange("city", "Austin"));
    act(() => result.current.onFieldChange("state", "TX"));
    act(() => result.current.onFieldChange("zipCode", "78701"));
    fillRequiredValidFields(result);

    await act(async () => {
      await result.current.onSubmit(submitEvent());
    });

    expect(createJobActionMock).toHaveBeenCalledExactlyOnceWith(
      expect.objectContaining({
        title: "Roof repair",
        latitude: 30.27,
        longitude: -97.74,
        customerId: VALID_CUSTOMER_ID,
      }),
    );
    expect(onCreated).toHaveBeenCalledOnce();
  });

  test("a successful submit resets the form back to its initial (empty) state", async () => {
    createJobActionMock.mockResolvedValue({ success: true });
    const { result } = renderHook(() => useCreateJob(vi.fn()));

    act(() => result.current.onFieldChange("title", "Roof repair"));
    fillRequiredValidFields(result);
    await act(async () => {
      await result.current.onSubmit(submitEvent());
    });

    expect(result.current.fields.title).toBe("");
    expect(result.current.error).toBeNull();
  });

  test("a failed submit keeps the entered fields and surfaces the server's error message", async () => {
    createJobActionMock.mockResolvedValue({ success: false, error: "Customer not found." });
    const onCreated = vi.fn();
    const { result } = renderHook(() => useCreateJob(onCreated));

    act(() => result.current.onFieldChange("title", "Roof repair"));
    fillRequiredValidFields(result);
    await act(async () => {
      await result.current.onSubmit(submitEvent());
    });

    expect(result.current.error).toBe("Customer not found.");
    expect(result.current.fields.title).toBe("Roof repair");
    expect(result.current.isSubmitting).toBe(false);
    expect(onCreated).not.toHaveBeenCalled();
  });

  test("isSubmitting is true while the action is in flight", async () => {
    let resolveAction!: (value: { success: boolean }) => void;
    createJobActionMock.mockReturnValue(
      new Promise((resolve) => {
        resolveAction = resolve;
      }),
    );
    const { result } = renderHook(() => useCreateJob(vi.fn()));
    fillRequiredValidFields(result);

    let submitPromise!: Promise<void>;
    act(() => {
      submitPromise = result.current.onSubmit(submitEvent());
    });

    expect(result.current.isSubmitting).toBe(true);

    await act(async () => {
      resolveAction({ success: true });
      await submitPromise;
    });

    expect(result.current.isSubmitting).toBe(false);
  });

  test("typing an out-of-range latitude sets an inline field error immediately, without waiting for submit", () => {
    const { result } = renderHook(() => useCreateJob(vi.fn()));

    act(() => result.current.onFieldChange("latitude", "999"));

    expect(result.current.fieldErrors.latitude).toMatch(/between -90 and 90/);
  });

  test("submitting with an out-of-range longitude is blocked before calling createJobAction", async () => {
    const { result } = renderHook(() => useCreateJob(vi.fn()));

    act(() => result.current.onFieldChange("latitude", "30.27"));
    act(() => result.current.onFieldChange("longitude", "200"));
    act(() => result.current.onFieldChange("customerId", VALID_CUSTOMER_ID));

    await act(async () => {
      await result.current.onSubmit(submitEvent());
    });

    expect(createJobActionMock).not.toHaveBeenCalled();
    expect(result.current.fieldErrors.longitude).toMatch(/between -180 and 180/);
  });

  test("fixing an invalid field clears its error", () => {
    const { result } = renderHook(() => useCreateJob(vi.fn()));

    act(() => result.current.onFieldChange("latitude", "999"));
    expect(result.current.fieldErrors.latitude).toBeDefined();

    act(() => result.current.onFieldChange("latitude", "30.27"));
    expect(result.current.fieldErrors.latitude).toBeUndefined();
  });

  test("a customerId that isn't a GUID is rejected, both inline and at submit", async () => {
    const { result } = renderHook(() => useCreateJob(vi.fn()));

    act(() => result.current.onFieldChange("customerId", "cust-1"));
    expect(result.current.fieldErrors.customerId).toMatch(/valid GUID/);

    act(() => result.current.onFieldChange("latitude", "30.27"));
    act(() => result.current.onFieldChange("longitude", "-97.74"));

    await act(async () => {
      await result.current.onSubmit(submitEvent());
    });

    expect(createJobActionMock).not.toHaveBeenCalled();
  });

  test("an empty customerId is rejected as required", () => {
    const { result } = renderHook(() => useCreateJob(vi.fn()));

    act(() => result.current.onFieldChange("customerId", "not-empty"));
    act(() => result.current.onFieldChange("customerId", ""));

    expect(result.current.fieldErrors.customerId).toMatch(/required/);
  });

  test("assigneeId is optional — empty is fine, but a non-empty value must still be a GUID", () => {
    const { result } = renderHook(() => useCreateJob(vi.fn()));

    expect(result.current.fieldErrors.assigneeId).toBeUndefined();

    act(() => result.current.onFieldChange("assigneeId", "not-a-guid"));
    expect(result.current.fieldErrors.assigneeId).toMatch(/valid GUID/);

    act(() => result.current.onFieldChange("assigneeId", ""));
    expect(result.current.fieldErrors.assigneeId).toBeUndefined();
  });
});

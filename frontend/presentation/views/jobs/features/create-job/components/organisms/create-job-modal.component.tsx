"use client";

import type { CreateJobFormFields } from "../../create-job-form-fields.type";
import { useCreateJob } from "../../hooks/use-create-job.hook";
import { useCustomers } from "../../hooks/use-customers.hook";

interface CreateJobModalProps {
  open: boolean;
  onClose: () => void;
  onCreated: () => void;
}

interface FormFieldProps {
  label: string;
  field: keyof CreateJobFormFields;
  value: string;
  onFieldChange: (field: keyof CreateJobFormFields, value: string) => void;
  type?: string;
  required?: boolean;
  min?: number;
  max?: number;
  step?: string;
  error?: string;
}

/**
 * Controlled Component pattern (AC.md 2.3.1): this input owns no state of its
 * own — its value always comes from the parent (useCreateJob's reducer state)
 * and every keystroke is reported back via onChange, rather than the input
 * managing an uncontrolled DOM value.
 */
function FormField({
  label,
  field,
  value,
  onFieldChange,
  type = "text",
  required,
  min,
  max,
  step,
  error,
}: FormFieldProps) {
  return (
    <label className="flex flex-col gap-1 text-sm">
      <span className="font-medium text-gray-700">{label}</span>
      <input
        className={`rounded-md border px-2 py-1 ${error ? "border-red-500" : "border-gray-300"}`}
        type={type}
        value={value}
        required={required}
        min={min}
        max={max}
        step={step}
        aria-invalid={error ? true : undefined}
        onChange={(event) => onFieldChange(field, event.target.value)}
      />
      {error ? (
        <span className="text-xs text-red-600" data-testid={`${field}-error`}>
          {error}
        </span>
      ) : null}
    </label>
  );
}

interface CustomerSelectProps {
  value: string;
  onFieldChange: (field: keyof CreateJobFormFields, value: string) => void;
  error?: string;
}

/**
 * A dropdown of real customers, not a free-text GUID field — the customer id
 * problem (typing an unknown/invalid id) is solved by construction instead of
 * by validating harder after the fact. Still a Controlled Component: value
 * and the change handler both come from the parent.
 */
function CustomerSelect({ value, onFieldChange, error }: CustomerSelectProps) {
  const { customers, isLoading, isError } = useCustomers();

  return (
    <label className="flex flex-col gap-1 text-sm">
      <span className="font-medium text-gray-700">Customer</span>
      <select
        className={`rounded-md border bg-white px-2 py-1 ${error ? "border-red-500" : "border-gray-300"}`}
        value={value}
        required
        disabled={isLoading || isError}
        aria-invalid={error ? true : undefined}
        onChange={(event) => onFieldChange("customerId", event.target.value)}
      >
        <option value="" disabled>
          {isLoading ? "Loading customers…" : "Select a customer"}
        </option>
        {customers.map((customer) => (
          <option key={customer.id} value={customer.id}>
            {customer.name}
          </option>
        ))}
      </select>
      {isError ? <span className="text-xs text-red-600">Could not load customers.</span> : null}
      {error ? (
        <span className="text-xs text-red-600" data-testid="customerId-error">
          {error}
        </span>
      ) : null}
    </label>
  );
}

/**
 * Thin shell (AC.md 2.1.6): every bit of state and the submit handler live in
 * useCreateJob — this component only wires hook output to JSX.
 */
export function CreateJobModal({ open, onClose, onCreated }: CreateJobModalProps) {
  const { fields, fieldErrors, isSubmitting, error, onFieldChange, onSubmit } = useCreateJob(() => {
    onCreated();
    onClose();
  });

  return !open ? null : (
    <div className="fixed inset-0 flex items-center justify-center bg-black/40" data-testid="create-job-modal">
      <form
        onSubmit={onSubmit}
        className="flex max-h-[90vh] w-full max-w-lg flex-col gap-3 overflow-y-auto rounded-lg bg-white p-6"
      >
        <h2 className="text-lg font-semibold">Create job</h2>

        <FormField label="Title" field="title" value={fields.title} onFieldChange={onFieldChange} required />
        <FormField
          label="Description"
          field="description"
          value={fields.description}
          onFieldChange={onFieldChange}
          required
        />
        <FormField label="Street" field="street" value={fields.street} onFieldChange={onFieldChange} required />
        <FormField label="City" field="city" value={fields.city} onFieldChange={onFieldChange} required />
        <FormField label="State" field="state" value={fields.state} onFieldChange={onFieldChange} required />
        <FormField label="Zip code" field="zipCode" value={fields.zipCode} onFieldChange={onFieldChange} required />
        <FormField
          label="Latitude"
          field="latitude"
          type="number"
          min={-90}
          max={90}
          step="any"
          value={fields.latitude}
          onFieldChange={onFieldChange}
          error={fieldErrors.latitude}
          required
        />
        <FormField
          label="Longitude"
          field="longitude"
          type="number"
          min={-180}
          max={180}
          step="any"
          value={fields.longitude}
          onFieldChange={onFieldChange}
          error={fieldErrors.longitude}
          required
        />
        <CustomerSelect value={fields.customerId} onFieldChange={onFieldChange} error={fieldErrors.customerId} />
        <FormField
          label="Scheduled date (optional)"
          field="scheduledDate"
          type="date"
          value={fields.scheduledDate}
          onFieldChange={onFieldChange}
        />
        <FormField
          label="Assignee ID (optional)"
          field="assigneeId"
          value={fields.assigneeId}
          onFieldChange={onFieldChange}
          error={fieldErrors.assigneeId}
        />

        {error ? <p className="text-sm text-red-600">{error}</p> : null}

        <div className="mt-2 flex justify-end gap-2">
          <button type="button" onClick={onClose} className="rounded-md px-3 py-1.5 text-sm">
            Cancel
          </button>
          <button
            type="submit"
            disabled={isSubmitting}
            className="rounded-md bg-blue-600 px-3 py-1.5 text-sm text-white disabled:opacity-50"
          >
            {isSubmitting ? "Creating…" : "Create job"}
          </button>
        </div>
      </form>
    </div>
  );
}

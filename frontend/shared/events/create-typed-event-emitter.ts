import type { TypedEventEmitter } from "./typed-event-emitter.type";

export function createTypedEventEmitter<Events extends object>(): TypedEventEmitter<Events> {
  // Handlers for different events genuinely have different payload types, so a
  // single collection can't name one honest element type. `never` is the only
  // parameter type every `(payload: Events[K]) => void` is assignable to
  // (function parameters are checked contravariantly, and `never` is a subtype
  // of everything) — but that same variance means a `(payload: never) => void`
  // can't be *called* with a real payload without a cast back to the specific
  // signature. The cast is scoped to this one call site and never touches
  // `any`; the public interface above stays fully type-safe either way.
  const handlersByEvent = new Map<keyof Events, Set<(payload: never) => void>>();

  function on<Event extends keyof Events>(event: Event, handler: (payload: Events[Event]) => void): void {
    const handlers = handlersByEvent.get(event) ?? new Set<(payload: never) => void>();
    handlers.add(handler);
    handlersByEvent.set(event, handlers);
  }

  function off<Event extends keyof Events>(event: Event, handler: (payload: Events[Event]) => void): void {
    handlersByEvent.get(event)?.delete(handler);
  }

  function emit<Event extends keyof Events>(event: Event, payload: Events[Event]): void {
    const handlers = handlersByEvent.get(event);
    handlers?.forEach((handler) => (handler as (payload: Events[Event]) => void)(payload));
  }

  return { on, off, emit };
}

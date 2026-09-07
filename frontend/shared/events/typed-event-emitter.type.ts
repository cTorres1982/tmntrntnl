/**
 * `Events` maps event names to their payload type. `.on`/`.off`/`.emit` all
 * infer the handler/payload signature from the chosen event name — passing
 * a handler with the wrong payload type, or an event name not in `Events`,
 * is a compile-time error.
 */
export interface TypedEventEmitter<Events extends object> {
  on<Event extends keyof Events>(event: Event, handler: (payload: Events[Event]) => void): void;
  off<Event extends keyof Events>(event: Event, handler: (payload: Events[Event]) => void): void;
  emit<Event extends keyof Events>(event: Event, payload: Events[Event]): void;
}

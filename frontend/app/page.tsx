import { redirect } from "next/navigation";

/** The whole app is the /jobs feature (AC.md Part 2) — no separate landing page. */
export default function Home() {
  redirect("/jobs");
}

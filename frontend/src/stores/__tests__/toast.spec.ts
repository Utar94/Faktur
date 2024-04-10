import type { ToastOptions } from "logitar-vue3-ui";
import { beforeEach, describe, expect, it } from "vitest";
import { createPinia, setActivePinia } from "pinia";

import { useToastStore } from "../toast";

const toast: ToastOptions = {
  duration: 10 * 1000,
  fade: true,
  text: "Hello World!",
  title: "Message",
  variant: "info",
};

describe("toastStore", () => {
  beforeEach(() => {
    setActivePinia(createPinia());
  });

  it("should add a toast to the list", () => {
    const toasts = useToastStore();
    expect(toasts.toasts.length).toBe(0);

    toasts.add(toast);
    expect(toasts.toasts.length).toBe(1);
    expect(toasts.toasts.at(0)?.id).toBe(toast.id);
  });

  it("should remove a toast from the list", () => {
    const toasts = useToastStore();
    expect(toasts.toasts.length).toBe(0);

    toasts.add(toast);
    expect(toasts.toasts.at(0)?.id).toBe(toast.id);

    toasts.remove(toast);
    expect(toasts.toasts.length).toBe(0);
  });
});

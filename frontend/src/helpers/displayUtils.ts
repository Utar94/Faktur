import type { Department } from "@/types/departments";

export function formatDepartment(department: Department): string {
  return `${department.displayName} (#${department.number})`;
}

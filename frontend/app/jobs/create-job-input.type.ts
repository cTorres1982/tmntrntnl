export interface CreateJobInput {
  title: string;
  description: string;
  street: string;
  city: string;
  state: string;
  zipCode: string;
  latitude: number;
  longitude: number;
  customerId: string;
  notes?: string;
  scheduledDate?: string;
  assigneeId?: string;
}

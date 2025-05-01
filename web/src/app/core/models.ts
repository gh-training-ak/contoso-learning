export type MeetingType = 'Online' | 'InPerson' | 'Either';

export type Subject =
  | 'Mathematics'
  | 'Physics'
  | 'Chemistry'
  | 'Biology'
  | 'ComputerScience'
  | 'English'
  | 'History'
  | 'Geography'
  | 'Economics';

export interface MentorSearchResult {
  readonly id: string;
  readonly displayName: string;
  readonly hourlyRate: number;
  readonly currency: string;
  readonly rating: number;
  readonly reviewCount: number;
  readonly distanceKm: number | null;
}

export interface MentorFilters {
  subject?: Subject;
  meetingType?: MeetingType;
  maxHourlyRate?: number;
  minimumRating?: number;
  withinKm?: number;
  page?: number;
  pageSize?: number;
}

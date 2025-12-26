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

/** Paged envelope introduced in 3.0. Before that the API returned a bare array. */
export interface PagedResult<T> {
  readonly items: readonly T[];
  readonly page: number;
  readonly pageSize: number;
  readonly total: number;
  readonly totalPages: number;
  readonly hasNext: boolean;
  readonly hasPrevious: boolean;
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

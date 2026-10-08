import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';
import { TestBed, fakeAsync, tick } from '@angular/core/testing';
import { MentorSearchResult, PagedResult } from '../core/models';
import { MentorSearchService } from './mentor-search.service';

describe('MentorSearchService', () => {
  let service: MentorSearchService;
  let http: HttpTestingController;

  const emptyPage = {
    items: [],
    page: 1,
    pageSize: 20,
    total: 0,
    totalPages: 0,
    hasNext: false,
    hasPrevious: false,
  };

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    service = TestBed.inject(MentorSearchService);
    http = TestBed.inject(HttpTestingController);
  });

  afterEach(() => http.verify());

  it('omits undefined filters from the query string', () => {
    service.search({ subject: 'Physics', maxHourlyRate: undefined }).subscribe();

    const req = http.expectOne((r) => r.url === '/api/mentors');
    expect(req.request.params.get('subject')).toBe('Physics');
    expect(req.request.params.has('maxHourlyRate')).toBe(false);
    req.flush(emptyPage);
  });

  it('retries twice before falling back to an empty page', fakeAsync(() => {
    let result: PagedResult<MentorSearchResult> | undefined;
    service.search({ pageSize: 10 }).subscribe((page) => (result = page));

    // One attempt plus the two retries configured in the service.
    for (let attempt = 0; attempt < 3; attempt++) {
      http.expectOne((r) => r.url === '/api/mentors').error(new ProgressEvent('network error'));
      tick(2000);
    }

    expect(result?.items.length).toBe(0);
    expect(result?.pageSize).toBe(10);
  }));

  it('sorts the subject list', () => {
    let subjects: readonly string[] = [];
    service.subjects().subscribe((s) => (subjects = s));

    http
      .expectOne((r) => r.url === '/api/mentors/subjects')
      .flush(['Physics', 'Biology', 'English']);

    expect(subjects).toEqual(['Biology', 'English', 'Physics']);
  });

  it('returns null when a mentor is not found', () => {
    let mentor: MentorSearchResult | null = {} as MentorSearchResult;
    service.byId('missing').subscribe((m) => (mentor = m));

    http.expectOne((r) => r.url === '/api/mentors/missing').flush(null, {
      status: 404,
      statusText: 'Not Found',
    });

    expect(mentor).toBeNull();
  });
});

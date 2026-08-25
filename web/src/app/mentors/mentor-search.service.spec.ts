import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';
import { TestBed } from '@angular/core/testing';
import { MentorSearchService } from './mentor-search.service';

describe('MentorSearchService', () => {
  let service: MentorSearchService;
  let http: HttpTestingController;

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
    req.flush({ items: [], page: 1, pageSize: 20, total: 0, totalPages: 0, hasNext: false, hasPrevious: false });
  });

  it('falls back to an empty page when the request fails', (done) => {
    service.search({ pageSize: 10 }).subscribe((page) => {
      expect(page.items.length).toBe(0);
      expect(page.pageSize).toBe(10);
      done();
    });

    for (let i = 0; i < 3; i++) {
      http.expectOne('/api/mentors').error(new ProgressEvent('network error'));
    }
  });
});

import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, catchError, map, of, retry, shareReplay, timer } from 'rxjs';
import { MentorFilters, MentorSearchResult } from '../core/models';

@Injectable({ providedIn: 'root' })
export class MentorSearchService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = '/api/mentors';

  search(filters: MentorFilters): Observable<readonly MentorSearchResult[]> {
    return this.http
      .get<MentorSearchResult[]>(this.baseUrl, { params: this.toParams(filters) })
      .pipe(
        retry({ count: 2, delay: (_, attempt) => timer(attempt * 500) }),
        catchError(() => of(this.emptyPage(filters.pageSize ?? 20))),
        shareReplay({ bufferSize: 1, refCount: true }),
      );
  }

  byId(id: string): Observable<MentorSearchResult | null> {
    return this.http
      .get<MentorSearchResult>(`${this.baseUrl}/${id}`)
      .pipe(catchError(() => of(null)));
  }

  subjects(): Observable<readonly string[]> {
    return this.http.get<readonly string[]>(`${this.baseUrl}/subjects`).pipe(
      map((subjects) => [...subjects].sort((a, b) => a.localeCompare(b))),
      catchError(() => of([])),
    );
  }

  private toParams(filters: MentorFilters): HttpParams {
    let params = new HttpParams();

    for (const [key, value] of Object.entries(filters)) {
      if (value !== undefined && value !== null && value !== '') {
        params = params.set(key, String(value));
      }
    }

    return params;
  }

}

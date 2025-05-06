import { CommonModule } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MentorFilters, MentorSearchResult } from '../core/models';
import { MentorSearchService } from './mentor-search.service';

@Component({
  selector: 'contoso-mentor-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <section class="mentor-list">
      <h2>Find a mentor</h2>

      <form class="filters" (ngSubmit)="search()">
        <label>
          Subject
          <select [(ngModel)]="subject" name="subject">
            <option [ngValue]="undefined">Any</option>
            <option value="Mathematics">Mathematics</option>
            <option value="Physics">Physics</option>
            <option value="ComputerScience">Computer science</option>
          </select>
        </label>

        <label>
          Maximum hourly rate
          <input type="number" min="0" [(ngModel)]="maxRate" name="maxRate" />
        </label>

        <button type="submit" [disabled]="loading()">Search</button>
      </form>

      @if (loading()) {
        <p class="muted" role="status">Searching...</p>
      } @else if (results().length === 0) {
        <p class="muted">No mentors match those filters yet. Try widening the price range.</p>
      } @else {
        <p class="summary">{{ total() }} mentors, showing {{ results().length }}</p>
        <ul>
          @for (mentor of results(); track mentor.id) {
            <li>
              <strong>{{ mentor.displayName }}</strong>
              <span>{{ mentor.rating | number: '1.1-1' }} from {{ mentor.reviewCount }} reviews</span>
              <span>{{ mentor.hourlyRate | currency: mentor.currency }} per hour</span>
              @if (mentor.distanceKm !== null) {
                <span>{{ mentor.distanceKm | number: '1.0-1' }} km away</span>
              }
            </li>
          }
        </ul>
      }
    </section>
  `,
})
export class MentorListComponent {
  private readonly service = inject(MentorSearchService);

  protected subject: MentorFilters['subject'];
  protected maxRate?: number;

  readonly results = signal<readonly MentorSearchResult[]>([]);
  readonly total = signal(0);
  readonly loading = signal(false);

  readonly averageRate = computed(() => {
    const items = this.results();
    return items.length === 0 ? 0 : items.reduce((sum, m) => sum + m.hourlyRate, 0) / items.length;
  });

  search(): void {
    this.loading.set(true);

    this.service.search({ subject: this.subject, maxHourlyRate: this.maxRate, pageSize: 20 }).subscribe({
      next: (page) => {
        this.results.set(page.items);
        this.total.set(page.total);
      },
      complete: () => this.loading.set(false),
      error: () => this.loading.set(false),
    });
  }
}

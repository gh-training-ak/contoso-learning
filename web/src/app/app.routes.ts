import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./mentors/mentor-list.component').then((m) => m.MentorListComponent),
    title: 'Find a mentor',
  },
  { path: '**', redirectTo: '' },
];

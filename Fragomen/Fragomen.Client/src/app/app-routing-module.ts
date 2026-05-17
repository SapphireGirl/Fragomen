import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { Users } from './components/users/users'

const routes: Routes = [
  { path: 'users', loadComponent: () => import('./components/users/users').then(m => m.Users) },
  { path: 'cases', loadComponent: () => import('./components/cases/cases').then(m => m.Cases) },
  { path: '', redirectTo: '/users', pathMatch: 'full' },
  { path: '**', redirectTo: '/users' }
];
@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule { }

import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Users } from './components/users/users';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, Users],
  templateUrl: './app.html',
  styleUrls: ['./app.css'],
})
export class App {
  protected readonly title = signal('Fragomen.Client');
}

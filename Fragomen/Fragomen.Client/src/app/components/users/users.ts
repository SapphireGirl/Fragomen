import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatExpansionModule } from '@angular/material/expansion';
import { HttpClient } from '@angular/common/http';
import { Router, ActivatedRoute } from '@angular/router'; 

interface User {
  id: number;
  firstName: string;
  lastName: string;
  country: string;
  email?: string;
}

@Component({
  selector: 'app-users',
  standalone: true,
  imports: [CommonModule, MatExpansionModule],
  templateUrl: './users.html',
  styleUrls: ['./users.css'],
})
export class Users implements OnInit {
  users: User[] = [];
  isLoading = false;
  errorMessage = '';

  constructor(private http: HttpClient, private router: Router) { }

  ngOnInit(): void {
    this.fetchUsers();
  }

  fetchUsers(): void {
    this.isLoading = true;
    this.errorMessage = '';

    const url = '/api/User/GetAllUsers';

    this.http.get<User[]>(url).subscribe({
      next: (users: User[]): void => {
        this.users = users ?? [];
        this.isLoading = false;
      },
      error: (error: { message: string }) => {
        this.errorMessage = error?.message ?? 'Failed to load users.';
        this.isLoading = false;
      },
    });
  }

  goToCase() { this.router.navigate(['/cases']); }
}

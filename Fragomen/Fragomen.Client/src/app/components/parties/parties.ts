import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatListModule } from '@angular/material/list';
import { MatChipsModule } from '@angular/material/chips';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatTooltipModule } from '@angular/material/tooltip'; 
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router'; 
import { Case, CaseParty, Party } from '../interfaces/interfaces'; 

@Component({
  selector: 'app-parties',
  standalone: true,
  imports: [
    CommonModule,
    MatExpansionModule,
    MatListModule,
    MatChipsModule,
    MatIconModule,
    MatButtonModule,     // add if you use mat-button
    MatTooltipModule     // add if you use mat-tooltip
  ],
  templateUrl: './parties.html',
  styleUrls: ['./parties.css'],
})
export class Parties implements OnInit{
  isLoading = false;
  parties: Party[] = [];
  error: string | null = null; // add this
  errorMessage = '';

  constructor(private http: HttpClient) { }

  ngOnInit(): void {
    this.fetchParties();
  }

  fetchParties(): void {
    
    this.isLoading = true;
    this.errorMessage = '';

    const url = 'api/case/GetAllParties';

    this.http.get<{ parties: Party[] }>(url).subscribe({
      next: (response: { parties: Party[] }): void => {
        this.parties = response?.parties ?? [];
        this.error = null; // clear any previous error
        this.isLoading = false;
      },
      error: (error: any): void => {
        this.error = error?.message || 'An error occurred while fetching parties';
        this.errorMessage = 'Failed to load parties';
        this.isLoading = false;
      }
    });
  }

  editParty(party: Party): void {

  }

  addCaseToParty(party: Party): void {

  }

  openCase(caseId: number): void {
  }

  // add these trackBy functions
  trackByPartyId(_: number, p: Party): number {
    return p.partyId;
  }

  trackByCasePartyId(_: number, cp: CaseParty): number {
    return cp.casePartyId;
  }

}

import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatListItem, MatListModule } from '@angular/material/list';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Router } from '@angular/router';
import { Case, CaseParty, Party } from '../interfaces/interfaces';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';


@Component({
  selector: 'app-cases',
  standalone: true,
  templateUrl: './cases.html',
  styleUrls: ['./cases.css'],
  imports: [CommonModule, MatExpansionModule, MatListModule, MatIconModule, MatChipsModule]
})
export class Cases implements OnInit {
  caseData!: Case;
  caseParties: CaseParty[] = [];
  isLoading = false;
  error: any = null;

  constructor(private http: HttpClient) { }

  ngOnInit(): void {
    this.fetchCase(
      1,
      (response) => { console.log('Got case:', response); },
      (err) => { console.error('Fetch error:', err); }
    );
  }

  fetchCase(caseId: number, next?: (res: any) => void, error?: (err: any) => void): void {
    this.isLoading = true;
    this.error = null;

    const params = new HttpParams().set('caseId', String(caseId));
    const url = 'api/Case/GetCase_PartiesByCaseId';

    this.http.get<Case>(url, { params }).subscribe({
      next: (response: Case) => {
        this.isLoading = false;
        this.caseData = response;
        this.caseParties = response?.caseParties ?? [];
        if (next) {
          console.log('Fetched case data:', response);
          next(response);
        }
      },
      error: (err) => {
        this.isLoading = false;
        this.error = err;
        if (error) error(err);
        else console.error('Fetch error:', err);
      }
    });
  }



  editCase(caseId: number): void {

    // Navigate to the case details page

  }

  createPartyForCase(caseId: number): void {

  }

  openParty(partyId: number): void {

  }

  // add these trackBy functions
  trackByCaseId(index: number, item: CaseParty) {
    return item?.casePartyId ?? index;
  }

  trackByPartyId(_: number, p: Party): number {
    return p.partyId;
  }

  trackByCasePartyId(_: number, cp: CaseParty): number {
    return cp.casePartyId;
  }

}

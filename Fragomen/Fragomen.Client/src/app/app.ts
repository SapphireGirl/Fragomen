import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AppRoutingModule } from './app-routing-module';
//import { Users } from './components/users/users'; 
import { Cases } from './components/cases/cases'; 

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, AppRoutingModule, Cases],
  templateUrl: './app.html',
  styleUrls: ['./app.css'],
})
export class App {}

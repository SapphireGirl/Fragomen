import { NgModule, provideBrowserGlobalErrorListeners } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { MatGridListModule } from '@angular/material/grid-list';
import { MatChipsModule } from '@angular/material/chips';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { AppRoutingModule } from './app-routing-module';

import { Cases } from './components/cases/cases';
import { Users } from './components/users/users';

@NgModule({
  imports: [
    BrowserModule,
    AppRoutingModule,
    MatGridListModule,
    MatChipsModule,
    MatExpansionModule,
    MatListModule,
    MatIconModule,
    MatButtonModule,
    Cases,
    Users
  ],
})
export class AppModule {}

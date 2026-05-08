import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClientModule } from '@angular/common/http';
import { MatGridListModule } from '@angular/material/grid-list';

import { UsersComponent } from './components/users/users.component';

@NgModule({
  declarations: [UsersComponent],
    imports: [
        BrowserModule,
        BrowserAnimationsModule,
        HttpClientModule,
        MatGridListModule
    ],
    bootstrap: [UsersComponent],
    schemas: []
})
export class AppModule {}
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { UnauthorizedComponent } from './components/unauthorized/unauthorized.component';

@NgModule({
  declarations: [ ],
  imports: [
    CommonModule,
    RouterModule,
    UnauthorizedComponent
  ],
  exports: [
    UnauthorizedComponent
  ]
})
export class SharedModule { }

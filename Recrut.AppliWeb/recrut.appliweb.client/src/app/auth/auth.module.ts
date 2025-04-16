import { NgModule } from '@angular/core';
import { LoginComponent } from './components/login/login.component';
import { RegisterComponent } from './components/register/register.component';

@NgModule({
  imports: [
    LoginComponent,
    RegisterComponent
  ]
})
export class AuthModule { }

import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { AuthModule } from './auth/auth.module';
import { JwtService } from './auth/interceptors/jwt.service';

@NgModule({
  declarations: [ ],
  imports: [
    BrowserModule,
    AuthModule,
    AppRoutingModule,
    AppComponent
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: JwtService, multi: true }
  ],
  schemas: [
    CUSTOM_ELEMENTS_SCHEMA
  ]
})
export class AppModule { }

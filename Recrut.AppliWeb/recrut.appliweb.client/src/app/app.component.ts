import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
// import { NavigationEnd, Router } from '@angular/router';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
  standalone: true,
  imports: [RouterOutlet]
})
export class AppComponent {
  ngOnInit() {
      throw new Error('Method not implemented.');
  }
  title = 'recrut.appliweb.client';
}

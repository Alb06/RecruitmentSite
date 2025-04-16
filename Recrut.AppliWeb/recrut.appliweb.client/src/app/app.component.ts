import { Component } from '@angular/core';
/*import { NavigationEnd, Router } from '@angular/router';*/

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
  standalone: false
})
export class AppComponent {
  ngOnInit() {
      throw new Error('Method not implemented.');
  }
  forecasts(forecasts: any) {
      throw new Error('Method not implemented.');
  }
  title = 'recrut.appliweb.client';
  //constructor(private router: Router) {
  //  router.events.subscribe(event => {
  //    if (event instanceof NavigationEnd) {
  //      console.log('Route actuelle:', event.url);
  //    }
  //  });
  //}
}

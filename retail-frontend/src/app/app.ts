import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NavbarComponent } from './components/navbar/navbar.component';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet, NavbarComponent],
  template: `
    <app-navbar></app-navbar>
    <main>
      <router-outlet></router-outlet>
    </main>
    <footer class="bg-dark text-white text-center py-3 mt-5">
      <small>© 2026 RetailOrder System — Built with Angular &amp; ASP.NET Core</small>
    </footer>
  `,
  styles: [`
    main { min-height: calc(100vh - 120px); }
  `]
})
export class App {
  title = 'retail-frontend';
}

import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { CartService } from '../../services/cart.service';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './navbar.component.html'
})
export class NavbarComponent implements OnInit {
  auth = inject(AuthService);
  cartService = inject(CartService);

  ngOnInit(): void {
    if (this.auth.isCustomer) {
      this.cartService.getCart().subscribe();
    }
  }

  logout(): void {
    this.cartService.clearLocal();
    this.auth.logout();
  }
}

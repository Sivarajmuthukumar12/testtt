import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { OrderService } from '../../services/order.service';
import { CartService } from '../../services/cart.service';

@Component({
  selector: 'app-checkout',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './checkout.component.html'
})
export class CheckoutComponent implements OnInit {
  orderService = inject(OrderService);
  cartService = inject(CartService);
  router = inject(Router);

  deliveryAddress = '';
  couponCode = '';
  loyaltyPointsToRedeem = 0;
  loading = false;
  error = '';
  placedOrder: any = null;

  ngOnInit(): void {
    const state = history.state;
    if (state?.couponCode) this.couponCode = state.couponCode;
    if (state?.loyaltyPointsToRedeem) this.loyaltyPointsToRedeem = state.loyaltyPointsToRedeem;
  }

  placeOrder(): void {
    if (!this.deliveryAddress.trim()) {
      this.error = 'Please enter a delivery address.';
      return;
    }
    this.loading = true;
    this.error = '';
    this.orderService.placeOrder({
      couponCode: this.couponCode || undefined,
      loyaltyPointsToRedeem: this.loyaltyPointsToRedeem,
      deliveryAddress: this.deliveryAddress
    }).subscribe({
      next: (order) => {
        this.loading = false;
        this.placedOrder = order;
        this.cartService.clearLocal();
      },
      error: (err) => {
        this.loading = false;
        this.error = err.error?.message || 'Failed to place order.';
      }
    });
  }
}

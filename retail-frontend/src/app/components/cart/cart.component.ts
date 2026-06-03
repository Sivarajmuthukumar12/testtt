import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { CartService } from '../../services/cart.service';
import { CouponService } from '../../services/coupon.service';
import { Cart } from '../../models/cart.model';

@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './cart.component.html'
})
export class CartComponent implements OnInit {
  cartService = inject(CartService);
  couponService = inject(CouponService);
  router = inject(Router);

  cart: Cart | null = null;
  loading = false;
  couponCode = '';
  couponResult: any = null;
  couponError = '';
  loyaltyPoints = 0;
  loyaltyPointsToRedeem = 0;

  ngOnInit(): void {
    this.loadCart();
    this.loadLoyaltyPoints();
  }

  loadCart(): void {
    this.loading = true;
    this.cartService.getCart().subscribe({
      next: (cart) => { this.cart = cart; this.loading = false; },
      error: () => this.loading = false
    });
  }

  loadLoyaltyPoints(): void {
    this.couponService.getLoyaltyPoints().subscribe({
      next: (lp) => this.loyaltyPoints = lp.points,
      error: () => {}
    });
  }

  updateQuantity(itemId: number, qty: number): void {
    this.cartService.updateItem(itemId, qty).subscribe(cart => this.cart = cart);
  }

  removeItem(itemId: number): void {
    this.cartService.removeItem(itemId).subscribe(cart => this.cart = cart);
  }

  clearCart(): void {
    this.cartService.clearCart().subscribe(() => { this.cart = null; this.couponResult = null; });
  }

  applyCoupon(): void {
    this.couponError = '';
    this.couponResult = null;
    this.couponService.applyCoupon(this.couponCode).subscribe({
      next: (res) => this.couponResult = res,
      error: (err) => this.couponError = err.error?.message || 'Invalid coupon.'
    });
  }

  get finalAmount(): number {
    const base = this.couponResult ? this.couponResult.finalAmount : (this.cart?.totalAmount ?? 0);
    const loyaltyDiscount = Math.floor(this.loyaltyPointsToRedeem / 100) * 10;
    return Math.max(0, base - loyaltyDiscount);
  }

  checkout(): void {
    this.router.navigate(['/checkout'], {
      state: {
        couponCode: this.couponResult?.couponCode,
        loyaltyPointsToRedeem: this.loyaltyPointsToRedeem
      }
    });
  }
}

import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CouponService } from '../../../services/coupon.service';

@Component({
  selector: 'app-admin-coupons',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin-coupons.component.html'
})
export class AdminCouponsComponent implements OnInit {
  couponService = inject(CouponService);

  coupons: any[] = [];
  loading = false;
  showForm = false;
  successMsg = '';
  errorMsg = '';

  form = {
    code: '', discountPercentage: 0, fixedDiscountAmount: 0,
    minimumOrderAmount: 0, expiryDate: '', usageLimit: 1
  };

  ngOnInit(): void {
    this.loadCoupons();
  }

  loadCoupons(): void {
    this.loading = true;
    this.couponService.getAllCoupons().subscribe({
      next: (c) => { this.coupons = c; this.loading = false; },
      error: () => this.loading = false
    });
  }

  save(): void {
    this.errorMsg = '';
    this.couponService.createCoupon(this.form).subscribe({
      next: () => {
        this.showForm = false;
        this.successMsg = 'Coupon created!';
        this.loadCoupons();
        setTimeout(() => this.successMsg = '', 3000);
      },
      error: (err) => this.errorMsg = err.error?.message || 'Failed to create coupon.'
    });
  }

  delete(id: number): void {
    if (!confirm('Delete this coupon?')) return;
    this.couponService.deleteCoupon(id).subscribe(() => this.loadCoupons());
  }
}

import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CouponService } from '../../services/coupon.service';

@Component({
  selector: 'app-loyalty',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './loyalty.component.html'
})
export class LoyaltyComponent implements OnInit {
  couponService = inject(CouponService);
  loyalty: any = null;
  loading = false;

  ngOnInit(): void {
    this.loading = true;
    this.couponService.getLoyaltyPoints().subscribe({
      next: (lp) => { this.loyalty = lp; this.loading = false; },
      error: () => this.loading = false
    });
  }
}

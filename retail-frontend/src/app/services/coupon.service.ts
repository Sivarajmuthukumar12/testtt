import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class CouponService {
  private http = inject(HttpClient);

  getAllCoupons(): Observable<any[]> {
    return this.http.get<any[]>('/api/coupons');
  }

  createCoupon(req: any): Observable<any> {
    return this.http.post<any>('/api/coupons', req);
  }

  deleteCoupon(id: number): Observable<void> {
    return this.http.delete<void>(`/api/coupons/${id}`);
  }

  applyCoupon(couponCode: string): Observable<any> {
    return this.http.post<any>('/api/coupons/apply', { couponCode });
  }

  getLoyaltyPoints(): Observable<any> {
    return this.http.get<any>('/api/loyalty');
  }
}

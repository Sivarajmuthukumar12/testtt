import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { Cart } from '../models/cart.model';

@Injectable({ providedIn: 'root' })
export class CartService {
  private http = inject(HttpClient);
  private cartSubject = new BehaviorSubject<Cart | null>(null);
  cart$ = this.cartSubject.asObservable();

  get cartItemCount(): number {
    return this.cartSubject.value?.totalItems ?? 0;
  }

  getCart(): Observable<Cart> {
    return this.http.get<Cart>('/api/cart').pipe(
      tap(cart => this.cartSubject.next(cart))
    );
  }

  addItem(productId: number, quantity: number = 1): Observable<Cart> {
    return this.http.post<Cart>('/api/cart/items', { productId, quantity }).pipe(
      tap(cart => this.cartSubject.next(cart))
    );
  }

  updateItem(itemId: number, quantity: number): Observable<Cart> {
    return this.http.put<Cart>(`/api/cart/items/${itemId}`, { quantity }).pipe(
      tap(cart => this.cartSubject.next(cart))
    );
  }

  removeItem(itemId: number): Observable<Cart> {
    return this.http.delete<Cart>(`/api/cart/items/${itemId}`).pipe(
      tap(cart => this.cartSubject.next(cart))
    );
  }

  clearCart(): Observable<void> {
    return this.http.delete<void>('/api/cart').pipe(
      tap(() => this.cartSubject.next(null))
    );
  }

  clearLocal(): void {
    this.cartSubject.next(null);
  }
}

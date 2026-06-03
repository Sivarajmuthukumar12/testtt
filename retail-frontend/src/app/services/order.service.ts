import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Order, PlaceOrderRequest } from '../models/order.model';

@Injectable({ providedIn: 'root' })
export class OrderService {
  private http = inject(HttpClient);

  placeOrder(req: PlaceOrderRequest): Observable<Order> {
    return this.http.post<Order>('/api/orders', req);
  }

  getMyOrders(): Observable<Order[]> {
    return this.http.get<Order[]>('/api/orders/my-orders');
  }

  getOrder(id: number): Observable<Order> {
    return this.http.get<Order>(`/api/orders/${id}`);
  }

  trackOrder(id: number): Observable<any> {
    return this.http.get<any>(`/api/orders/${id}/track`);
  }

  reorder(id: number): Observable<any> {
    return this.http.post<any>(`/api/orders/${id}/reorder`, {});
  }

  getAllOrders(page = 1, pageSize = 10): Observable<Order[]> {
    return this.http.get<Order[]>(`/api/orders/all?page=${page}&pageSize=${pageSize}`);
  }

  updateStatus(id: number, status: string): Observable<Order> {
    return this.http.put<Order>(`/api/orders/${id}/status`, { status });
  }

  assignDelivery(orderId: number, deliveryPartnerId: number): Observable<any> {
    return this.http.put<any>(`/api/orders/${orderId}/assign-delivery`, { deliveryPartnerId });
  }
}

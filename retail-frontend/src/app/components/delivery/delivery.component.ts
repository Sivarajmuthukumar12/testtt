import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { AuthService } from '../../services/auth.service';
import { Order } from '../../models/order.model';

@Component({
  selector: 'app-delivery',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './delivery.component.html'
})
export class DeliveryComponent implements OnInit {
  http = inject(HttpClient);
  auth = inject(AuthService);

  orders: Order[] = [];
  loading = false;
  successMsg = '';

  ngOnInit(): void {
    this.loadOrders();
  }

  loadOrders(): void {
    this.loading = true;
    this.http.get<Order[]>('/api/delivery/orders').subscribe({
      next: (o) => { this.orders = o; this.loading = false; },
      error: () => this.loading = false
    });
  }

  accept(orderId: number): void {
    this.http.put<any>(`/api/delivery/orders/${orderId}/accept`, {}).subscribe({
      next: () => {
        this.successMsg = `Order #${orderId} accepted!`;
        this.loadOrders();
        setTimeout(() => this.successMsg = '', 3000);
      }
    });
  }

  markDelivered(orderId: number): void {
    this.http.put<any>(`/api/delivery/orders/${orderId}/deliver`, {}).subscribe({
      next: () => {
        this.successMsg = `Order #${orderId} marked as delivered!`;
        this.loadOrders();
        setTimeout(() => this.successMsg = '', 3000);
      }
    });
  }

  getStatusClass(status: string): string {
    const map: Record<string, string> = {
      'Pending': 'bg-warning text-dark',
      'Accepted': 'bg-info text-white',
      'OutForDelivery': 'bg-primary text-white',
      'Delivered': 'bg-success text-white'
    };
    return map[status] ?? 'bg-secondary text-white';
  }
}

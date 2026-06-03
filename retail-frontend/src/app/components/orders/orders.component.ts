import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { OrderService } from '../../services/order.service';
import { CartService } from '../../services/cart.service';
import { Order } from '../../models/order.model';

@Component({
  selector: 'app-orders',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './orders.component.html'
})
export class OrdersComponent implements OnInit {
  orderService = inject(OrderService);
  cartService = inject(CartService);

  orders: Order[] = [];
  loading = false;
  reorderSuccess = '';
  selectedOrder: Order | null = null;

  ngOnInit(): void {
    this.loading = true;
    this.orderService.getMyOrders().subscribe({
      next: (orders) => { this.orders = orders; this.loading = false; },
      error: () => this.loading = false
    });
  }

  reorder(orderId: number): void {
    this.orderService.reorder(orderId).subscribe({
      next: () => {
        this.reorderSuccess = 'Items added to cart!';
        this.cartService.getCart().subscribe();
        setTimeout(() => this.reorderSuccess = '', 3000);
      },
      error: () => {}
    });
  }

  getStatusClass(status: string): string {
    const map: Record<string, string> = {
      'Pending': 'bg-warning text-dark',
      'Accepted': 'bg-info text-white',
      'OutForDelivery': 'bg-primary text-white',
      'Delivered': 'bg-success text-white',
      'Cancelled': 'bg-danger text-white'
    };
    return map[status] ?? 'bg-secondary text-white';
  }

  getStatusIcon(status: string): string {
    const map: Record<string, string> = {
      'Pending': 'bi-clock',
      'Accepted': 'bi-check-circle',
      'OutForDelivery': 'bi-truck',
      'Delivered': 'bi-check-circle-fill',
      'Cancelled': 'bi-x-circle'
    };
    return map[status] ?? 'bi-circle';
  }
}

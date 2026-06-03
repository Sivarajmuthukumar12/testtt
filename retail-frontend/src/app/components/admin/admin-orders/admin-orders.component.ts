import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { OrderService } from '../../../services/order.service';
import { Order } from '../../../models/order.model';

@Component({
  selector: 'app-admin-orders',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin-orders.component.html'
})
export class AdminOrdersComponent implements OnInit {
  orderService = inject(OrderService);

  orders: Order[] = [];
  loading = false;
  page = 1;
  pageSize = 10;
  successMsg = '';

  statuses = ['Pending', 'Accepted', 'OutForDelivery', 'Delivered', 'Cancelled'];

  ngOnInit(): void {
    this.loadOrders();
  }

  loadOrders(): void {
    this.loading = true;
    this.orderService.getAllOrders(this.page, this.pageSize).subscribe({
      next: (orders) => { this.orders = orders; this.loading = false; },
      error: () => this.loading = false
    });
  }

  updateStatus(order: Order, status: string): void {
    this.orderService.updateStatus(order.id, status).subscribe({
      next: () => {
        order.orderStatus = status;
        this.successMsg = `Order #${order.id} updated to ${status}`;
        setTimeout(() => this.successMsg = '', 3000);
      }
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
}

import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { ProductService } from '../../../services/product.service';
import { Product } from '../../../models/product.model';

@Component({
  selector: 'app-admin-inventory',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin-inventory.component.html'
})
export class AdminInventoryComponent implements OnInit {
  productService = inject(ProductService);
  http = inject(HttpClient);

  lowStockProducts: Product[] = [];
  allProducts: Product[] = [];
  loading = false;
  selectedProductId: number | null = null;
  transactions: any[] = [];
  updateForm = { newQuantity: 0, transactionType: 'StockAdded', notes: '' };
  successMsg = '';

  ngOnInit(): void {
    this.loading = true;
    this.productService.getLowStock().subscribe(p => { this.lowStockProducts = p; this.loading = false; });
    this.productService.getProducts().subscribe(p => this.allProducts = p);
  }

  loadTransactions(productId: number): void {
    this.selectedProductId = productId;
    this.http.get<any[]>(`/api/inventory/${productId}/transactions`).subscribe(t => this.transactions = t);
  }

  updateStock(productId: number): void {
    this.http.put(`/api/inventory/${productId}/stock`, this.updateForm).subscribe({
      next: () => {
        this.successMsg = 'Stock updated!';
        this.productService.getLowStock().subscribe(p => this.lowStockProducts = p);
        this.productService.getProducts().subscribe(p => this.allProducts = p);
        if (this.selectedProductId === productId) this.loadTransactions(productId);
        setTimeout(() => this.successMsg = '', 3000);
      }
    });
  }
}

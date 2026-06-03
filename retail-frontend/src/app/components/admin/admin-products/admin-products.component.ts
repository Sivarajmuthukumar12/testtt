import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ProductService } from '../../../services/product.service';
import { Category, Product } from '../../../models/product.model';

@Component({
  selector: 'app-admin-products',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin-products.component.html'
})
export class AdminProductsComponent implements OnInit {
  productService = inject(ProductService);

  products: Product[] = [];
  categories: Category[] = [];
  loading = false;
  showForm = false;
  editingProduct: Product | null = null;
  successMsg = '';
  errorMsg = '';

  form = {
    name: '', description: '', price: 0, stockQuantity: 0,
    minimumStockLevel: 5, imageUrl: '', categoryId: 0, isActive: true
  };

  ngOnInit(): void {
    this.loadAll();
  }

  loadAll(): void {
    this.loading = true;
    this.productService.getCategories().subscribe(cats => this.categories = cats);
    this.productService.getProducts().subscribe({
      next: (p) => { this.products = p; this.loading = false; },
      error: () => this.loading = false
    });
  }

  openCreate(): void {
    this.editingProduct = null;
    this.form = { name: '', description: '', price: 0, stockQuantity: 0, minimumStockLevel: 5, imageUrl: '', categoryId: 0, isActive: true };
    this.showForm = true;
  }

  openEdit(p: Product): void {
    this.editingProduct = p;
    this.form = { name: p.name, description: p.description, price: p.price, stockQuantity: p.stockQuantity, minimumStockLevel: p.minimumStockLevel, imageUrl: p.imageUrl ?? '', categoryId: p.categoryId, isActive: p.isActive };
    this.showForm = true;
  }

  save(): void {
    this.errorMsg = '';
    const req = { ...this.form };
    const obs = this.editingProduct
      ? this.productService.updateProduct(this.editingProduct.id, req)
      : this.productService.createProduct(req);

    obs.subscribe({
      next: () => {
        this.showForm = false;
        this.successMsg = this.editingProduct ? 'Product updated!' : 'Product created!';
        this.loadAll();
        setTimeout(() => this.successMsg = '', 3000);
      },
      error: (err) => this.errorMsg = err.error?.message || 'Failed to save product.'
    });
  }

  toggleActive(p: Product): void {
    this.productService.toggleActive(p.id).subscribe(() => this.loadAll());
  }

  delete(p: Product): void {
    if (!confirm(`Delete "${p.name}"?`)) return;
    this.productService.deleteProduct(p.id).subscribe(() => this.loadAll());
  }
}

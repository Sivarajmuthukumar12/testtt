import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { ProductService } from '../../services/product.service';
import { CartService } from '../../services/cart.service';
import { AuthService } from '../../services/auth.service';
import { Category, Product } from '../../models/product.model';

@Component({
  selector: 'app-products',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './products.component.html'
})
export class ProductsComponent implements OnInit {
  productService = inject(ProductService);
  cartService = inject(CartService);
  auth = inject(AuthService);

  products: Product[] = [];
  categories: Category[] = [];
  loading = false;
  addingToCart: { [key: number]: boolean } = {};
  successMsg: { [key: number]: boolean } = {};

  // Filters
  search = '';
  selectedCategory: number | undefined;
  minPrice: number | undefined;
  maxPrice: number | undefined;

  ngOnInit(): void {
    this.loadCategories();
    this.loadProducts();
  }

  loadCategories(): void {
    this.productService.getCategories().subscribe(cats => this.categories = cats);
  }

  loadProducts(): void {
    this.loading = true;
    this.productService.getProducts(this.search || undefined, this.selectedCategory, this.minPrice, this.maxPrice)
      .subscribe({
        next: (products) => { this.products = products; this.loading = false; },
        error: () => this.loading = false
      });
  }

  addToCart(product: Product): void {
    if (!this.auth.isCustomer) return;
    this.addingToCart[product.id] = true;
    this.cartService.addItem(product.id, 1).subscribe({
      next: () => {
        this.addingToCart[product.id] = false;
        this.successMsg[product.id] = true;
        setTimeout(() => delete this.successMsg[product.id], 2000);
      },
      error: () => this.addingToCart[product.id] = false
    });
  }

  clearFilters(): void {
    this.search = '';
    this.selectedCategory = undefined;
    this.minPrice = undefined;
    this.maxPrice = undefined;
    this.loadProducts();
  }

  getProductImage(product: Product): string {
    if (product.imageUrl) return product.imageUrl;
    const name = product.name.toLowerCase();
    if (name.includes('pizza')) return 'https://images.unsplash.com/photo-1565299624946-b28f40a0ae38?w=400&h=300&fit=crop';
    if (name.includes('pepsi') || name.includes('cola') || name.includes('sprite')) return 'https://images.unsplash.com/photo-1622483767028-3f66f32aef97?w=400&h=300&fit=crop';
    if (name.includes('bread')) return 'https://images.unsplash.com/photo-1549931319-a545dcf3bc73?w=400&h=300&fit=crop';
    return 'https://images.unsplash.com/photo-1546069901-ba9599a7e63c?w=400&h=300&fit=crop';
  }
}

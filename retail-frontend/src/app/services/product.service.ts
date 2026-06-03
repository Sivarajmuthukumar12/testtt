import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Category, CreateProductRequest, Product } from '../models/product.model';

@Injectable({ providedIn: 'root' })
export class ProductService {
  private http = inject(HttpClient);

  getProducts(search?: string, categoryId?: number, minPrice?: number, maxPrice?: number): Observable<Product[]> {
    let params = new HttpParams();
    if (search) params = params.set('search', search);
    if (categoryId) params = params.set('categoryId', categoryId);
    if (minPrice != null) params = params.set('minPrice', minPrice);
    if (maxPrice != null) params = params.set('maxPrice', maxPrice);
    return this.http.get<Product[]>('/api/products', { params });
  }

  getProduct(id: number): Observable<Product> {
    return this.http.get<Product>(`/api/products/${id}`);
  }

  createProduct(req: CreateProductRequest): Observable<Product> {
    return this.http.post<Product>('/api/products', req);
  }

  updateProduct(id: number, req: any): Observable<Product> {
    return this.http.put<Product>(`/api/products/${id}`, req);
  }

  deleteProduct(id: number): Observable<void> {
    return this.http.delete<void>(`/api/products/${id}`);
  }

  toggleActive(id: number): Observable<Product> {
    return this.http.patch<Product>(`/api/products/${id}/toggle-active`, {});
  }

  getLowStock(): Observable<Product[]> {
    return this.http.get<Product[]>('/api/products/low-stock');
  }

  getCategories(): Observable<Category[]> {
    return this.http.get<Category[]>('/api/categories');
  }

  createCategory(req: { name: string; description: string }): Observable<Category> {
    return this.http.post<Category>('/api/categories', req);
  }

  deleteCategory(id: number): Observable<void> {
    return this.http.delete<void>(`/api/categories/${id}`);
  }
}

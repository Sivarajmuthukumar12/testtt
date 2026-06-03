import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ProductService } from '../../../services/product.service';
import { Category } from '../../../models/product.model';

@Component({
  selector: 'app-admin-categories',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './admin-categories.component.html'
})
export class AdminCategoriesComponent implements OnInit {
  productService = inject(ProductService);
  categories: Category[] = [];
  loading = false;
  showForm = false;
  form = { name: '', description: '' };
  successMsg = '';
  errorMsg = '';

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    this.productService.getCategories().subscribe({
      next: (c) => { this.categories = c; this.loading = false; },
      error: () => this.loading = false
    });
  }

  save(): void {
    this.errorMsg = '';
    this.productService.createCategory(this.form).subscribe({
      next: () => {
        this.showForm = false;
        this.successMsg = 'Category created!';
        this.form = { name: '', description: '' };
        this.load();
        setTimeout(() => this.successMsg = '', 3000);
      },
      error: (err) => this.errorMsg = err.error?.message || 'Failed.'
    });
  }

  delete(id: number): void {
    if (!confirm('Delete this category?')) return;
    this.productService.deleteCategory(id).subscribe(() => this.load());
  }
}

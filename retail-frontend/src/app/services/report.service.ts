import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { DashboardStats, SalesReport, TopProduct } from '../models/report.model';

@Injectable({ providedIn: 'root' })
export class ReportService {
  private http = inject(HttpClient);

  getDashboard(): Observable<DashboardStats> {
    return this.http.get<DashboardStats>('/api/reports/dashboard');
  }

  getDailySales(date: string): Observable<SalesReport> {
    return this.http.get<SalesReport>(`/api/reports/sales/daily?date=${date}`);
  }

  getMonthlySales(year: number, month: number): Observable<SalesReport> {
    return this.http.get<SalesReport>(`/api/reports/sales/monthly?year=${year}&month=${month}`);
  }

  getTopProducts(count = 10): Observable<TopProduct[]> {
    return this.http.get<TopProduct[]>(`/api/reports/top-products?count=${count}`);
  }

  getTopCategories(): Observable<any[]> {
    return this.http.get<any[]>('/api/reports/top-categories');
  }
}

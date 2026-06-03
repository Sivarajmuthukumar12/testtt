import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ReportService } from '../../../services/report.service';
import { DashboardStats } from '../../../models/report.model';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './dashboard.component.html'
})
export class DashboardComponent implements OnInit {
  reportService = inject(ReportService);
  stats: DashboardStats | null = null;
  loading = false;

  ngOnInit(): void {
    this.loading = true;
    this.reportService.getDashboard().subscribe({
      next: (s) => { this.stats = s; this.loading = false; },
      error: () => this.loading = false
    });
  }
}

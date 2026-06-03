export interface DashboardStats {
  totalOrders: number;
  totalRevenue: number;
  totalCustomers: number;
  totalProducts: number;
  pendingOrders: number;
  deliveredOrders: number;
  lowStockProducts: number;
  topSellingProducts: TopProduct[];
}

export interface TopProduct {
  productId: number;
  productName: string;
  totalQuantitySold: number;
  totalRevenue: number;
}

export interface SalesReport {
  date: string;
  totalOrders: number;
  totalRevenue: number;
}

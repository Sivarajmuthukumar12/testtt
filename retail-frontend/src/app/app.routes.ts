import { Routes } from '@angular/router';
import { authGuard, adminGuard, customerGuard } from './guards/auth.guard';

export const routes: Routes = [
  // Public
  { path: '', loadComponent: () => import('./components/home/home.component').then(m => m.HomeComponent) },
  { path: 'login', loadComponent: () => import('./components/login/login.component').then(m => m.LoginComponent) },
  { path: 'register', loadComponent: () => import('./components/register/register.component').then(m => m.RegisterComponent) },
  { path: 'products', loadComponent: () => import('./components/products/products.component').then(m => m.ProductsComponent) },

  // Customer
  { path: 'cart', loadComponent: () => import('./components/cart/cart.component').then(m => m.CartComponent), canActivate: [authGuard, customerGuard] },
  { path: 'checkout', loadComponent: () => import('./components/checkout/checkout.component').then(m => m.CheckoutComponent), canActivate: [authGuard, customerGuard] },
  { path: 'orders', loadComponent: () => import('./components/orders/orders.component').then(m => m.OrdersComponent), canActivate: [authGuard, customerGuard] },
  { path: 'loyalty', loadComponent: () => import('./components/loyalty/loyalty.component').then(m => m.LoyaltyComponent), canActivate: [authGuard, customerGuard] },

  // Shared (any logged-in user)
  { path: 'profile', loadComponent: () => import('./components/profile/profile.component').then(m => m.ProfileComponent), canActivate: [authGuard] },

  // Admin
  { path: 'admin/dashboard', loadComponent: () => import('./components/admin/dashboard/dashboard.component').then(m => m.DashboardComponent), canActivate: [authGuard, adminGuard] },
  { path: 'admin/products', loadComponent: () => import('./components/admin/admin-products/admin-products.component').then(m => m.AdminProductsComponent), canActivate: [authGuard, adminGuard] },
  { path: 'admin/categories', loadComponent: () => import('./components/admin/admin-categories/admin-categories.component').then(m => m.AdminCategoriesComponent), canActivate: [authGuard, adminGuard] },
  { path: 'admin/orders', loadComponent: () => import('./components/admin/admin-orders/admin-orders.component').then(m => m.AdminOrdersComponent), canActivate: [authGuard, adminGuard] },
  { path: 'admin/coupons', loadComponent: () => import('./components/admin/admin-coupons/admin-coupons.component').then(m => m.AdminCouponsComponent), canActivate: [authGuard, adminGuard] },
  { path: 'admin/inventory', loadComponent: () => import('./components/admin/admin-inventory/admin-inventory.component').then(m => m.AdminInventoryComponent), canActivate: [authGuard, adminGuard] },

  // Delivery Partner
  { path: 'delivery', loadComponent: () => import('./components/delivery/delivery.component').then(m => m.DeliveryComponent), canActivate: [authGuard] },

  // Fallback
  { path: '**', redirectTo: '' }
];

export interface Category {
  id: number;
  name: string;
  description: string;
  createdDate: string;
}

export interface Product {
  id: number;
  name: string;
  description: string;
  price: number;
  stockQuantity: number;
  minimumStockLevel: number;
  imageUrl?: string;
  isActive: boolean;
  categoryId: number;
  categoryName: string;
  isLowStock: boolean;
}

export interface CreateProductRequest {
  name: string;
  description: string;
  price: number;
  stockQuantity: number;
  minimumStockLevel: number;
  imageUrl?: string;
  categoryId: number;
}

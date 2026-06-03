export interface OrderItem {
  productId: number;
  productName: string;
  quantity: number;
  unitPrice: number;
  lineTotal: number;
}

export interface Order {
  id: number;
  totalAmount: number;
  discountAmount: number;
  finalAmount: number;
  orderStatus: string;
  couponCode?: string;
  loyaltyPointsUsed: number;
  loyaltyPointsEarned: number;
  orderDate: string;
  deliveredDate?: string;
  userId: number;
  customerName: string;
  deliveryPartnerName?: string;
  items: OrderItem[];
}

export interface PlaceOrderRequest {
  couponCode?: string;
  loyaltyPointsToRedeem: number;
  deliveryAddress: string;
}

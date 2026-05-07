export interface Product {
  id: number;
  name: string;
  description?: string;
  price: number;
  category?: string;
  imageUrl?: string;
  stock: number;
}

export interface CartItem {
  product: Product;
  quantity: number;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  name: string;
  email: string;
  password: string;
}

export interface AuthResponse {
  token: string;
  userId: number;
  name: string;
}

export interface CheckoutRequest {
  userId: number;
  shippingAddress: string;
  items: { productId: number; quantity: number }[];
}
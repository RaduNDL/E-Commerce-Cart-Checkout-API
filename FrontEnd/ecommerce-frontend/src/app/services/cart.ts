import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { CartItem, Product } from '../models/models';

@Injectable({ providedIn: 'root' })
export class CartService {
  private itemsSubject = new BehaviorSubject<CartItem[]>([]);
  items$ = this.itemsSubject.asObservable();

  get items(): CartItem[] {
    return this.itemsSubject.value;
  }

  get totalCount(): number {
    return this.items.reduce((sum, i) => sum + i.quantity, 0);
  }

  get totalPrice(): number {
    return this.items.reduce((sum, i) => sum + i.product.price * i.quantity, 0);
  }

  addToCart(product: Product) {
    const current = this.items;
    const existing = current.find(i => i.product.id === product.id);
    if (existing) {
      existing.quantity++;
      this.itemsSubject.next([...current]);
    } else {
      this.itemsSubject.next([...current, { product, quantity: 1 }]);
    }
  }

  removeItem(productId: number) {
    this.itemsSubject.next(this.items.filter(i => i.product.id !== productId));
  }

  updateQuantity(productId: number, quantity: number) {
    if (quantity <= 0) { this.removeItem(productId); return; }
    const current = this.items.map(i =>
      i.product.id === productId ? { ...i, quantity } : i
    );
    this.itemsSubject.next(current);
  }

  clearCart() {
    this.itemsSubject.next([]);
  }
}
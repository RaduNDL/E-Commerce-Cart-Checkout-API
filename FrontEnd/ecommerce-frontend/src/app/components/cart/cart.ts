import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { CartService } from '../../services/cart';

@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './cart.html',
})
export class CartComponent {
  constructor(public cartService: CartService) {}

  updateQty(productId: number, event: Event) {
    const val = +(event.target as HTMLInputElement).value;
    this.cartService.updateQuantity(productId, val);
  }
}
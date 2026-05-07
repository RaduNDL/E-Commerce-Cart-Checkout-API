import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { CartService } from '../../services/cart';
import { AuthService } from '../../services/auth';

@Component({
  selector: 'app-checkout',
  standalone: true,
  imports: [FormsModule, CommonModule, RouterLink],
  templateUrl: './checkout.html',
})
export class CheckoutComponent {
  shippingAddress = '';
  paymentMethod: 'cash' | 'card' = 'cash';
  cardName = '';
  cardNumber = '';
  cardExpiry = '';
  cardCvv = '';
  loading = false;
  error = '';
  success = '';

  public cartService = inject(CartService);
  private authService = inject(AuthService);
  private http = inject(HttpClient);
  private router = inject(Router);

  onCardNumberChange(value: string) {
    const cleanValue = value.replace(/\D/g, '').substring(0, 16);
    this.cardNumber = cleanValue.replace(/(.{4})/g, '$1 ').trim();
  }

  onExpiryChange(value: string) {
    const cleanValue = value.replace(/\D/g, '').substring(0, 4);
    if (cleanValue.length >= 2) {
      this.cardExpiry = `${cleanValue.substring(0, 2)}/${cleanValue.substring(2, 4)}`;
    } else {
      this.cardExpiry = cleanValue;
    }
  }

  onCvvChange(value: string) {
    this.cardCvv = value.replace(/\D/g, '').substring(0, 4);
  }

  placeOrder() {
    this.error = '';

    if (!this.authService.isLoggedIn) {
      this.error = 'You must be logged in to place an order.';
      return;
    }
    if (!this.shippingAddress.trim()) {
      this.error = 'Please enter a shipping address.';
      return;
    }
    if (this.cartService.items.length === 0) {
      this.error = 'Your cart is empty.';
      return;
    }
    if (this.paymentMethod === 'card') {
      if (this.cardName.trim().length < 3) {
        this.error = 'Please enter a valid cardholder name.';
        return;
      }
      if (this.cardNumber.replace(/\D/g, '').length < 16) {
        this.error = 'Card number must be 16 digits.';
        return;
      }
      if (this.cardExpiry.length !== 5) {
        this.error = 'Expiry date must be in MM/YY format.';
        return;
      }
      const month = parseInt(this.cardExpiry.substring(0, 2), 10);
      if (month < 1 || month > 12) {
        this.error = 'Invalid expiry month.';
        return;
      }
      if (this.cardCvv.length < 3) {
        this.error = 'CVV must be 3 or 4 digits.';
        return;
      }
    }

    this.loading = true;

    const payload = {
      userId: this.authService.currentUser!.userId,
      shippingAddress: this.shippingAddress,
      paymentMethod: this.paymentMethod,
      items: this.cartService.items.map(i => ({
        productId: i.product.id,
        quantity: i.quantity,
        price: i.product.price
      }))
    };

    const headers = new HttpHeaders({
      Authorization: `Bearer ${this.authService.currentUser!.token}`
    });

    this.http.post('http://localhost:5025/api/orders/checkout', payload, { headers }).subscribe({
      next: (res: any) => {
        this.loading = false;
        this.success = `Order #${res.orderId} placed successfully! Total: $${res.total}`;
        this.cartService.clearCart();
        setTimeout(() => this.router.navigate(['/products']), 3000);
      },
      error: (err) => {
        this.loading = false;
        this.error = err.error?.message || 'Checkout failed. Please try again.';
      }
    });
  }
}
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

  private validateCard(): string | null {
    if (this.cardName.trim().length < 3)
      return 'Please enter a valid cardholder name (min 3 characters).';

    const digits = this.cardNumber.replace(/\D/g, '');
    if (digits.length !== 16)
      return 'Card number must be exactly 16 digits.';

    let sum = 0;
    for (let i = 0; i < 16; i++) {
      let digit = parseInt(digits[15 - i]);
      if (i % 2 === 1) { digit *= 2; if (digit > 9) digit -= 9; }
      sum += digit;
    }
    if (sum % 10 !== 0)
      return 'Invalid card number.';

    if (this.cardExpiry.length !== 5)
      return 'Expiry date must be in MM/YY format.';

    const [mm, yy] = this.cardExpiry.split('/');
    const month = parseInt(mm, 10);
    const year = 2000 + parseInt(yy, 10);
    if (month < 1 || month > 12)
      return 'Invalid expiry month.';

    const now = new Date();
    const expiry = new Date(year, month, 0);
    if (expiry < now)
      return 'Card has expired.';

    if (this.cardCvv.length < 3)
      return 'CVV must be 3 or 4 digits.';

    return null;
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
      const cardError = this.validateCard();
      if (cardError) { this.error = cardError; return; }
    }

    this.loading = true;

    // userId scos — backend-ul il ia din JWT
    const payload = {
      shippingAddress: this.shippingAddress,
      items: this.cartService.items.map(i => ({
        productId: i.product.id,
        quantity: i.quantity
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
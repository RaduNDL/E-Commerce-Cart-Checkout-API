import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProductService } from '../../services/product';
import { CartService } from '../../services/cart';
import { Product } from '../../models/models';

@Component({
  selector: 'app-product-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './product-list.html',
})
export class ProductListComponent implements OnInit {
  products = signal<Product[]>([]);
  added = signal<{ [id: number]: boolean }>({});

  constructor(private productService: ProductService, private cartService: CartService) {}

  ngOnInit() {
    this.productService.getAll().subscribe({
      next: p => this.products.set(p),
      error: err => console.error('[ProductList] Error fetching products:', err)
    });
  }

  addToCart(product: Product) {
    this.cartService.addToCart(product);
    this.added.update(a => ({ ...a, [product.id]: true }));
    setTimeout(() => this.added.update(a => {
      const copy = { ...a };
      delete copy[product.id];
      return copy;
    }), 1500);
  }
}
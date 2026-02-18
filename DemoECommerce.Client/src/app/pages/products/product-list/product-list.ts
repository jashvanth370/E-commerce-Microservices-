import { Component, inject, signal } from '@angular/core';
import { ProductService } from '../../../services/product/product.service';
import { CartService } from '../../../services/cart/cart.service';
import { ProductDTO } from '../../../models/product';
import { RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-product-list',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './product-list.html',
  styleUrl: './product-list.css',
})
export class ProductList {
  productService = inject(ProductService);
  cartService = inject(CartService);
  products = signal<ProductDTO[]>([]);

  constructor() {
    this.loadProducts();
  }

  loadProducts() {
    this.productService.getAllProducts().subscribe({
      next: (res) => {
        this.products.set(res);
        console.log(res);
      },
      error: (err) => {
        console.error('Failed to load products', err);
      }
    });
  }

  addToCart(product: ProductDTO) {
    this.cartService.addToCart(product);
    alert(`Added ${product.name} to cart`);
  }
}

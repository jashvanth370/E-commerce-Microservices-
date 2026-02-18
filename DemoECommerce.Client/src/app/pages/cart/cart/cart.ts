import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { CartService } from '../../../services/cart/cart.service';
import { OrderService } from '../../../services/order/order.service';
import { AuthService } from '../../../services/auth/auth.service';
import { OrderDTO } from '../../../models/order';

@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './cart.html',
  styleUrl: './cart.css',
})
export class Cart {
  cartService = inject(CartService);
  orderService = inject(OrderService);
  authService = inject(AuthService);
  router = inject(Router);

  get cartItems() {
    return this.cartService.cartItems();
  }

  get total() {
    return this.cartService.totalStart();
  }

  removeFromCart(productId: number) {
    this.cartService.removeFromCart(productId);
  }

  checkout() {
    if (!this.authService.currentUser()) {
      alert('Please login to checkout');
      this.router.navigate(['/login']);
      return;
    }

    const userId = this.authService.currentUser()!.id;
    const items = this.cartItems;

    if (items.length === 0) return;

    items.forEach(item => {
      const order: any = {
        id: 0,
        productId: item.productId,
        clientId: userId,
        purchaseQuantity: item.quantity,
        orderDate: new Date().toISOString()
      };

      this.orderService.createOrder(order).subscribe({
        next: (res) => {
          if (res.flag) {
            console.log(`Order for ${item.productName} created`);
          } else {
            console.error(`Failed to create order for ${item.productName}: ${res.message}`);
          }
        },
        error: (err) => console.error(err)
      });
    });

    alert('Orders processed!');
    this.cartService.clearCart();
    this.router.navigate(['/orders']);
  }
}

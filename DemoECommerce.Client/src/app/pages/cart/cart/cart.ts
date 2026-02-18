import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { CartService } from '../../../services/cart/cart.service';
import { OrderService } from '../../../services/order/order.service';
import { AuthService } from '../../../services/auth/auth.service';
import { OrderDTO } from '../../../models/order';
import { jwtDecode } from 'jwt-decode';

@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [CommonModule, RouterLink],
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

  private getClientId(): number | null {
    const token = localStorage.getItem('token');
    if (!token) return null;

    const decoded: any = jwtDecode(token);
    return Number(decoded.id); // ✅ matches your JWT payload
  }

  checkout() {
    const clientId = this.getClientId();
    if (!clientId || isNaN(clientId)) {
      alert('Please login to checkout.');
      this.router.navigate(['/login']);
      return;
    }

    const items = this.cartItems;
    if (items.length === 0) {
      alert('Your cart is empty.');
      return;
    }

    let allOrdersProcessed = true;

    items.forEach(item => {
      const order: OrderDTO = {
        id: 0,
        productId: item.productId,
        clientId: clientId,
        purcheseQuantity: Number(item.quantity), // ✅ matches backend spelling
        orderDate: new Date().toISOString()
      };

      this.orderService.createOrder(order).subscribe({
        next: (res) => {
          if (res.flag) {
            console.log(`Order for ${item.productName} created`);
          } else {
            allOrdersProcessed = false;
            console.error(`Failed to create order for ${item.productName}: ${res.message}`);
          }
        },
        error: (err) => {
          allOrdersProcessed = false;
          console.error(err);
        }
      });
    });

    // Clear cart after orders
    this.cartService.clearCart();

    if (allOrdersProcessed) {
      alert('All orders placed successfully!');
    } else {
      alert('Some orders failed. Check console for details.');
    }

    this.router.navigate(['/orders']);
  }
}

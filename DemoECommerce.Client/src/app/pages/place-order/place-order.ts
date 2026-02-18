import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { OrderService } from '../../services/order/order.service';
import { OrderDTO } from '../../models/order';
import { AuthService } from '../../services/auth/auth.service';

@Component({
  selector: 'app-place-order',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './place-order.html',
  styleUrl: './place-order.css'
})
export class PlaceOrderComponent {

  fb = inject(FormBuilder);
  orderService = inject(OrderService);
  route = inject(ActivatedRoute);
  router = inject(Router);
  authService = inject(AuthService);

  productId!: number;

  orderForm = this.fb.group({
    purchaseQuantity: [1, [Validators.required, Validators.min(1)]]
  });

  constructor() {
    // Get productId from route
    this.productId = Number(this.route.snapshot.paramMap.get('id'));
  }



  onSubmit() {
    if (this.orderForm.invalid) return;

    const clientId = this.authService.getClientId();

    if (!clientId || isNaN(clientId)) {
      alert('Invalid user. Please login again.');
      return;
    }
    const order: OrderDTO = {
      id: 0,
      productId: this.productId,
      clientId: clientId!,
      purcheseQuantity: Number(this.orderForm.value.purchaseQuantity),
      orderDate: new Date().toISOString()
    };

    this.orderService.createOrder(order).subscribe({
      next: (res) => {
        if (res.flag) {
          alert('Order placed successfully');
          this.router.navigate(['/orders']);
        } else {
          alert(res.message);
        }
      },
      error: (err) => console.error(err)
    });
  }
}

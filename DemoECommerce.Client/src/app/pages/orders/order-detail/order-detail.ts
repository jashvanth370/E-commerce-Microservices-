import { Component, inject, signal, Input } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';
import { OrderService } from '../../../services/order/order.service';
import { OrderDetailDTO } from '../../../models/order';

@Component({
  selector: 'app-order-detail',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './order-detail.html',
  styleUrl: './order-detail.css',
})
export class OrderDetail {
  private route = inject(ActivatedRoute);
  orderService = inject(OrderService);
  orderDetail = signal<OrderDetailDTO | null>(null);

  constructor() {
    const id = Number(this.route.snapshot.paramMap.get('id'));

    if (id) {
      this.loadOrderDetails(id);
    }
  }

  loadOrderDetails(id: number) {
    this.orderService.getOrderDetails(id).subscribe({
      next: (res: any) => {
        this.orderDetail.set(res);
      },
      error: (err) => console.error(err)
    });
  }
}

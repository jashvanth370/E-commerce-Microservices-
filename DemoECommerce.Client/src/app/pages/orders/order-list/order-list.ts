import { Component, inject, signal, OnInit } from '@angular/core';
import { OrderService } from '../../../services/order/order.service';
import { AuthService } from '../../../services/auth/auth.service';
import { OrderDTO } from '../../../models/order';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
    selector: 'app-order-list',
    standalone: true,
    imports: [CommonModule,RouterLink],
    templateUrl: './order-list.html',
    styleUrl: './order-list.css',
})
export class OrderList implements OnInit {
    orderService = inject(OrderService);
    authService = inject(AuthService);
    orders = signal<OrderDTO[]>([]);

    ngOnInit() {
        this.loadOrders();
    }

    loadOrders() {
        const user = this.authService.currentUser();
        if (user) {
            this.orderService.getOrdersByClient(user.id).subscribe({
                next: (res) => {
                    console.log(res);
                    this.orders.set(res);
                },
                error: (err) => console.error(err)
            });
        }
    }
}

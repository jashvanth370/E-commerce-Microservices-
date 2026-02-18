import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { OrderDTO } from '../../models/order';
import { Response } from '../../models/auth';

@Injectable({
    providedIn: 'root',
})
export class OrderService {
    private url = '/api/orders';

    constructor(private http: HttpClient) { }

    createOrder(order: OrderDTO): Observable<Response> {
        return this.http.post<Response>(this.url, order);
    }

    getOrdersByClient(clientId: number): Observable<OrderDTO[]> {
        return this.http.get<OrderDTO[]>(`${this.url}/client/${clientId}`);
    }

    getOrderDetails(orderId: number) {
        return this.http.get(`${this.url}/details/${orderId}`);
    }
}

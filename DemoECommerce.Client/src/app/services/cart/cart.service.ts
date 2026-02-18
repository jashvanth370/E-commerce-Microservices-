import { Injectable, signal, computed, effect } from '@angular/core';
import { CartItem } from '../../models/order';
import { ProductDTO } from '../../models/product';

@Injectable({
    providedIn: 'root',
})
export class CartService {
    cartItems = signal<CartItem[]>([]);

    totalStart = computed(() => this.cartItems().reduce((acc, item) => acc + item.quantity * item.price, 0));
    itemCount = computed(() => this.cartItems().reduce((acc, item) => acc + item.quantity, 0));

    constructor() {
        const saved = localStorage.getItem('cart');
        if (saved) {
            this.cartItems.set(JSON.parse(saved));
        }

        effect(() => {
            localStorage.setItem('cart', JSON.stringify(this.cartItems()));
        });
    }

    addToCart(product: ProductDTO, quantity: number = 1) {
        this.cartItems.update(items => {
            const existing = items.find(i => i.productId === product.id);
            if (existing) {
                return items.map(i => i.productId === product.id ? { ...i, quantity: i.quantity + quantity } : i);
            }
            return [...items, {
                productId: product.id,
                productName: product.name,
                price: product.price,
                quantity,
                image: product.image
            }];
        });
    }

    removeFromCart(productId: number) {
        this.cartItems.update(items => items.filter(i => i.productId !== productId));
    }

    clearCart() {
        this.cartItems.set([]);
    }
}

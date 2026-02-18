export interface OrderDTO {
    id: number;
    productId: number;
    clientId: number;
    purcheseQuantity: number;
    orderDate: string;
}

export interface CartItem {
    productId: number;
    productName: string;
    price: number;
    quantity: number;
    image?: string;
}

export interface OrderDetailDTO {
    orderId: number;
    productId: number;
    clientId: number;
    name: string;
    email: string;
    address: string;
    telephoneNumber: string;
    productName: string;
    purcheseQuantity: number;
    unitPrice: number;
    totalPrice: number;
    orderDate: string;
}

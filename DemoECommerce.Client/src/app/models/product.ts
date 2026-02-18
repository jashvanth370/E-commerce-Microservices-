export interface ProductDTO {
    id: number;
    name: string;
    description: string;
    originalPrice: number;
    newPrice?: number; // If applicable
    quantity: number;
    categoryId?: number;
    categoryName?: string;
    image?: string; // Base64 or URL
}

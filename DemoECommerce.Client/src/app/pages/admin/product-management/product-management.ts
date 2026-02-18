import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ProductService } from '../../../services/product/product.service';
import { ProductDTO } from '../../../models/product';

@Component({
    selector: 'app-product-management',
    standalone: true,
    imports: [CommonModule, ReactiveFormsModule],
    templateUrl: './product-management.html',
    styleUrl: './product-management.css',
})
export class ProductManagement {
    productService = inject(ProductService);
    fb = inject(FormBuilder);

    products = signal<ProductDTO[]>([]);
    showForm = signal<boolean>(false);
    isEditing = signal<boolean>(false);

    productForm = this.fb.group({
        id: [0],
        name: ['', Validators.required],
        description: ['', Validators.required],
        originalPrice: [0, [Validators.required, Validators.min(0)]],
        newPrice: [0, [Validators.min(0)]],
        quantity: [0, [Validators.required, Validators.min(0)]],
        categoryId: [1, Validators.required], // Default for now, no category API
        image: ['']
    });

    constructor() {
        this.loadProducts();
    }

    loadProducts() {
        this.productService.getAllProducts().subscribe({
            next: (res) => this.products.set(res),
            error: (err) => console.error(err)
        });
    }

    toggleForm() {
        this.showForm.set(!this.showForm());
        if (!this.showForm()) {
            this.productForm.reset({ id: 0, categoryId: 1 });
            this.isEditing.set(false);
        }
    }

    startEdit(product: ProductDTO) {
        this.productForm.patchValue(product as any);
        this.isEditing.set(true);
        this.showForm.set(true);
    }

    deleteProduct(id: number) {
        if (confirm('Are you sure you want to delete this product?')) {
            this.productService.deleteProduct(id).subscribe({
                next: (res) => {
                    if (res.flag) {
                        this.loadProducts();
                        alert('Product deleted');
                    } else {
                        alert(res.message);
                    }
                },
                error: (err) => console.error(err)
            });
        }
    }

    onSubmit() {
        if (this.productForm.invalid) return;

        const formValue = this.productForm.value;

        const product: ProductDTO = {
            id: formValue.id ?? 0,
            name: formValue.name ?? '',
            description: formValue.description ?? '',
            price: Number(formValue.originalPrice),   // FIX HERE
            newPrice: formValue.newPrice ? Number(formValue.newPrice) : undefined,
            quantity: Number(formValue.quantity),
            categoryId: Number(formValue.categoryId),
            image: formValue.image ?? ''
        };

        if (this.isEditing()) {
            this.productService.updateProduct(product).subscribe({
                next: (res) => {
                    if (res.flag) {
                        alert('Product updated');
                        this.loadProducts();
                        this.toggleForm();
                    } else {
                        alert(res.message);
                    }
                }
            });
        } else {
            product.id = 0;

            this.productService.createProduct(product).subscribe({
                next: (res) => {
                    if (res.flag) {
                        alert('Product created');
                        this.loadProducts();
                        this.toggleForm();
                    } else {
                        alert(res.message);
                    }
                }
            });
        }
    }
}

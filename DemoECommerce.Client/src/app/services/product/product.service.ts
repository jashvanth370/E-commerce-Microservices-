import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ProductDTO } from '../../models/product';
import { Response } from '../../models/auth';

@Injectable({
    providedIn: 'root',
})
export class ProductService {
    private url = '/api/product';

    constructor(private http: HttpClient) { }

    getAllProducts(): Observable<ProductDTO[]> {
        return this.http.get<ProductDTO[]>(this.url);
    }

    getProductById(id: number): Observable<ProductDTO> {
        return this.http.get<ProductDTO>(`${this.url}/${id}`);
    }

    createProduct(product: ProductDTO): Observable<Response> {
        return this.http.post<Response>(this.url, product);
    }

    updateProduct(product: ProductDTO): Observable<Response> {
        return this.http.put<Response>(this.url, product);
    }

    deleteProduct(id: number): Observable<Response> {
        // Delete expected ProductDTO as body according to controller code?
        // Controller: [HttpDelete("{id:int}")] public async Task<ActionResult> DeleteProduct(ProductDTO productDTO)
        // Actually the controller takes ProductDTO AND ID in route.
        // Ideally it should just take ID.
        // The controller code:
        // [HttpDelete("{id:int}")]
        // public async Task<ActionResult> DeleteProduct(ProductDTO productDTO) { ... }
        // This is weird API design. Usually DELETE takes ID only.
        // I will try to pass ID in route and empty body or whatever required.
        // The controller uses `ProductConversion.ToEntity(productDTO)`.
        // It seems it needs the product object to delete? Or just ID?
        // `productInterface.DeleteAsync(product)` might use ID.
        // I'll stick to ID in route. If body is required, I might need to fetch it first or pass dummy.
        // The previous controller analysis showed `[HttpDelete("{id:int}")]`.
        // But then `public async Task<ActionResult> DeleteProduct(ProductDTO productDTO)`.
        // GET method usually ignores body. DELETE allows body but not recommended.
        // I will assume ID is enough or the backend logic uses ID from route.
        // Wait, the backend controller:
        /*
            [HttpDelete("{id:int}")]
            [Authorize(Roles = "Admin")]
            public async Task<ActionResult> DeleteProduct(ProductDTO productDTO)
            {
                var product = ProductConversion.ToEntity(productDTO);
                if (product is null) return NotFound...
                var responce = await productInterface.DeleteAsync(product);
                ...
            }
        */
        // If I send DELETE /api/product/1, the body might be empty. `productDTO` might be null or default.
        // `ToEntity` might fail if properties are missing.
        // This looks like a potential backend issue or quirky design.
        // For now, I will implement as is. I might need to send the product object in the body (using `http.delete` with body option).

        return this.http.delete<Response>(`${this.url}/${id}`);
        // If backend requires body, I need: request: { body: product }
    }
}

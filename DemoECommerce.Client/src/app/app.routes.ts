import { Routes } from '@angular/router';
import { Home } from './pages/home/home';
import { Login } from './pages/login/login';
import { Register } from './pages/register/register';
import { Cart } from './pages/cart/cart/cart';
import { ProductList } from './pages/products/product-list/product-list';
import { ProductDetail } from './pages/products/product-detail/product-detail';
import { OrderList } from './pages/orders/order-list/order-list';
import { OrderDetail } from './pages/orders/order-detail/order-detail';
import { AdminDashboard } from './pages/admin/admin-dashboard/admin-dashboard';
import { ProductManagement } from './pages/admin/product-management/product-management';
import { authGuard } from './services/auth/auth.guard';
import { adminGuard } from './services/auth/admin.guard';

export const routes: Routes = [
    { path: '', component: Home },
    { path: 'login', component: Login },
    { path: 'register', component: Register },
    { path: 'products', component: ProductList },
    { path: 'products/:id', component: ProductDetail },
    { path: 'cart', component: Cart, canActivate: [authGuard] },
    { path: 'orders', component: OrderList, canActivate: [authGuard] },
    { path: 'orders/:id', component: OrderDetail, canActivate: [authGuard] },
    { path: 'admin', component: AdminDashboard, canActivate: [adminGuard] },
    { path: 'admin/products', component: ProductManagement, canActivate: [adminGuard] }
];

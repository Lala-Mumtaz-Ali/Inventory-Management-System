import { Routes } from '@angular/router';
import { LoginComponent } from './login/login.component'; // adjust the path based on your file structure
import { DashboardComponent } from './dashboard/dashboard.component';
import { EditEmployeeComponent } from './edit-employee/edit-employee.component';
import { WarehouseComponent } from './warehouse/warehouse.component';
import { OrdersComponent } from './orders/orders.component';
import { ProductComponent } from './product/product.component';

export const routes: Routes = [
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path:'dashboard',  component:DashboardComponent },
  { path: 'edit-employee', component:EditEmployeeComponent},
  { path: 'warehouse', component:WarehouseComponent},
  { path: 'orders', component:OrdersComponent},
  { path: 'product', component:ProductComponent}
];
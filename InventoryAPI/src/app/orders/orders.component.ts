import { Component, OnInit } from '@angular/core';
import { OrdersService } from '../services/orders.service';
import { Product, Order, OrderDispatchDTO, Supplier, OrderItem, PurchaseOrder} from '../models/DTOs';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-orders',
  standalone: true,
  templateUrl: './orders.component.html',
  styleUrls: ['./orders.component.css'],
  imports: [FormsModule, CommonModule]
})
export class OrdersComponent implements OnInit {

  products: Product[] = [];
  orders: Order[] = [];
  orderItems: OrderItem[] = [];
  suppliers: Supplier[] = [];
  purchaseOrders: PurchaseOrder[] = [];
  orderId: number = 0;
  category: string = '';
  warehouseId: number = 0;
  categoryOptions: string[] = ['Shoes', 'Bags', 'Shirts', 'Pants', 'Accessories'];
  // Flags for messages
  errorMessage: string = '';
  successMessage: string = ''; 

  // Flags to show/hide forms
  categoryFormVisible: boolean = false;
  pendingOrdersFormVisible: boolean = false;
  dispatchOrdersFormVisible: boolean = false;
  lowStockFormVisible: boolean = false;
  missingProductsFormVisible: boolean = false;
  receivePurchaseFormVisible: boolean = false;
  suppliersFormVisible: boolean = false;
  purchaseOrdersFormVisible: boolean = false;
  orderItemsFormVisible:boolean = false;

  constructor(private ordersService: OrdersService) {}

  ngOnInit(): void {}

  // Reset all form visibility and clear data
  resetAllForms(): void {
    this.categoryFormVisible = false;
    this.pendingOrdersFormVisible = false;
    this.dispatchOrdersFormVisible = false;
    this.lowStockFormVisible = false;
    this.missingProductsFormVisible = false;
    this.receivePurchaseFormVisible = false;
    this.orderItemsFormVisible = false;
    this.purchaseOrdersFormVisible = false;
    this.suppliersFormVisible = false;
    this.products = [];
    this.orders = [];
    this.category = '';
    this.orderId = 0;
    this.warehouseId = 0;
    this.errorMessage = '';
    this.successMessage = '';
  }

  // Toggle form visibility with reset of other forms
  toggleForm(formName: string): void {
    this.resetAllForms();

    if (formName === 'categoryForm') {
      this.categoryFormVisible = true;
    } else if (formName === 'pendingOrdersForm') {
      this.pendingOrdersFormVisible = true;
    } else if (formName === 'dispatchOrdersForm') {
      this.dispatchOrdersFormVisible = true;
    } else if (formName === 'lowStockForm') {
      this.lowStockFormVisible = true;
    } else if (formName === 'missingProductsForm') {
      this.missingProductsFormVisible = true;
    } else if (formName === 'receivePurchaseForm') {
      this.receivePurchaseFormVisible = true;
    } else if (formName === 'suppliersForm') {
      this.suppliersFormVisible = true;
      this.loadSuppliers();
    } else if (formName === 'purchaseOrdersForm') {
      this.purchaseOrdersFormVisible = true;
      this.loadPurchaseOrders();
    } else if (formName === 'orderItemsForm'){
      this.orderItemsFormVisible = true;
      this.loadOrderItems();
    }
  }


  // Get All OrderItems
  loadOrderItems(): void {
    this.ordersService.getAllOrderItems().subscribe((items) => {
      this.orderItems = items;
      console.log(this.orderItems);
    }, error => {
      this.errorMessage = 'Failed to load order items';
    });
  }

  // Get All Suppliers
  loadSuppliers(): void {
    this.ordersService.getAllSuppliers().subscribe((suppliers) => {
      this.suppliers = suppliers;
      console.log(suppliers);
    }, error => {
      this.errorMessage = 'Failed to load suppliers';
    });
  }


  // Get All PurchaseOrders
  loadPurchaseOrders(): void {
    this.ordersService.getAllPurchaseOrders().subscribe((purchaseOrders) => {
      this.purchaseOrders = purchaseOrders;
      console.log(purchaseOrders);
    }, error => {
      this.errorMessage = 'Failed to load purchase orders';
    });
  }


  // Load products by category
  loadProductsByCategory(category: string): void {
    this.ordersService.getProductsByCategory(category).subscribe((products) => {
      this.products = products;
      console.log(this.products);
    });
  }

  // Load pending orders
  loadPendingOrders(): void {
    this.ordersService.getPendingCustomerOrders().subscribe((orders) => {
      this.orders = orders;
      console.log(this.orders);
    });
  }

  // Dispatch orders
  dispatchOrders(orderId: number, warehouseId: number): void {
    const dispatchData: OrderDispatchDTO[] = [{ order_Id: orderId, wareHouse_Id: warehouseId }];
    this.ordersService.dispatchOrders(dispatchData).subscribe(
      (response) => {
        console.log('Dispatch response:', response);
        this.successMessage = 'Order successfully dispatched!';
      },
      (error) => {
        this.errorMessage = 'An unexpected error occurred during dispatching.';
      }
    );
  }

  // Handle reorder low stock products
  reorderLowStock(): void {
    this.ordersService.reorderLowStockProducts().subscribe(
      (response) => {
        console.log('Low stock reorder response:', response);
        this.successMessage = 'Low stock products successfully reordered!';
      },
      (error) => {
        if (error.status === 404) {
          this.errorMessage = 'No Product in any warehouse below Low Stock Threshold';
        } else {
          this.errorMessage = 'An unexpected error occurred';
        }
      }
    );
  }

  // Handle reorder missing products
  reorderMissingProducts(): void {
    this.ordersService.reorderMissingProducts().subscribe(
      (response) => {
        console.log('Missing products reorder response:', response);
        this.successMessage = 'Missing products successfully reordered!';
      },
      (error) => {
        if (error.status === 404) {
          this.errorMessage = 'All Products Available in all Warehouses';
        } else {
          this.errorMessage = 'An unexpected error occurred';
        }
      }
    );
  }

  // Receive purchase orders
  receivePurchaseOrders(): void {
    this.ordersService.receivePurchaseOrders().subscribe(
        (response) => {
            console.log('Received purchase orders:', response);
            this.successMessage = 'Orders successfully retrieved';
            const message = response.message;
          
            if (message === 'All Purchase Orders to this date have  been Recieved') {
                this.errorMessage = message;
            } else {
                this.errorMessage = 'Unexpected response received';
            }
        },
        (error) => {
            console.error('Error receiving purchase orders:', error);
            this.errorMessage = 'An unexpected error occurred';
        }
    );
  }
}

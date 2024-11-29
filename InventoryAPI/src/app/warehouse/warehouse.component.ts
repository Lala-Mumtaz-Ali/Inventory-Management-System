import { Component, OnInit } from '@angular/core';
import { WarehouseService } from '../services/warehouse.service';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormGroup, FormBuilder, Validators } from '@angular/forms';
import { LotMovements } from '../models/DTOs';

@Component({
  selector: 'app-warehouse',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './warehouse.component.html',
  styleUrls: ['./warehouse.component.css'],
})
export class WarehouseComponent implements OnInit {
  // Data variables
  warehouses: any[] = [];
  warehouseLots: any[] = [];
  transferableWarehouses: any[] = [];
  lotHistory: any[] = [];
  message: string = '';
  loading: boolean = false;
  lotMovement: LotMovements[] = [];

  // Form for warehouse actions
  warehouseForm: FormGroup;

  // Flags for form visibility
  warehouseFormVisible: boolean = false;
  warehouseLotsFormVisible: boolean = false;
  transferableWarehousesFormVisible: boolean = false;
  lotHistoryFormVisible: boolean = false;
  transferLotFormVisible: boolean = false;

  constructor(
    private warehouseService: WarehouseService,
    private fb: FormBuilder
  ) {
    // Initialize the form group with relevant fields
    this.warehouseForm = this.fb.group({
      warehouseId: [null, Validators.required],
      productId: [null, Validators.required],
      lot_id: [null, Validators.required],
      Dest_id: [null, Validators.required],
    });
  }

  ngOnInit(): void {}

  // Reset all form visibility and clear data
  resetAllForms(): void {
    this.warehouseFormVisible = false;
    this.warehouseLotsFormVisible = false;
    this.transferableWarehousesFormVisible = false;
    this.lotHistoryFormVisible = false;
    this.transferLotFormVisible = false;
    this.message = '';
    this.warehouses = [];
    this.warehouseLots = [];
    this.transferableWarehouses = [];
    this.lotHistory = [];
    this.lotMovement = [];
    this.warehouseForm.reset();
  }

  // Toggle form visibility
  toggleForm(formName: string): void {
    this.resetAllForms(); // Reset all forms and data before toggling the form

    if (formName === 'warehouseForm') {
      this.warehouseFormVisible = true;
    } else if (formName === 'warehouseLotsForm') {
      this.warehouseLotsFormVisible = true;
    } else if (formName === 'transferableWarehousesForm') {
      this.transferableWarehousesFormVisible = true;
    } else if (formName === 'lotHistoryForm') {
      this.lotHistoryFormVisible = true;
    } else if (formName === 'transferLotForm') {
      this.transferLotFormVisible = true;
    } else if (formName === 'warehouseListForm') {
      this.getWarehouses();  // Handle Get Warehouses without form
    } else if (formName === 'allLotMovements'){
      this.getLotMovements();
    }
  }

  // Fetch list of warehouses
  getWarehouses() {
    this.loading = true;
    this.warehouseService.getWarehouses().subscribe({
      next: (data) => {
        this.warehouses = data;
        console.log(this.warehouses);
        this.message = '';
      },
      error: (err) => {
        this.message = `Error fetching warehouses: ${err.message}`;
      },
      complete: () => (this.loading = false),
    });
  }

  // Fetch lots for a warehouse by ID
  getWarehouseLots() {
    const { warehouseId, productId } = this.warehouseForm.value;
    if (!warehouseId || !productId) {
      this.message = 'Please provide both Warehouse ID and Product ID.';
      return;
    }
    this.loading = true;
    this.warehouseService.getWarehouseLots(warehouseId, productId).subscribe({
      next: (data) => {
        this.warehouseLots = data;
        console.log(this.warehouseLots);
        this.message = '';
      },
      error: (err) => {
        this.message = `Error fetching lots: ${err.message}`;
      },
      complete: () => (this.loading = false),
    });
  }

  // Fetch transferable warehouses for a lot
  getTransferableWarehouses() {
    const { lotId } = this.warehouseForm.value;
    if (!lotId) {
      this.message = 'Please provide Lot ID.';
      return;
    }
    this.loading = true;
    this.warehouseService.getTransferableWarehouses(lotId).subscribe({
      next: (data) => {
        this.transferableWarehouses = data;
        this.message = '';
      },
      error: (err) => {
        this.message = `Error fetching transferable warehouses: ${err.message}`;
      },
      complete: () => (this.loading = false),
    });
  }

  // Transfer a lot between warehouses
  transferLot() {
    const { lot_id, Dest_id } = this.warehouseForm.value;
    if (!lot_id || !Dest_id) {
      this.message = 'Please provide both Lot ID and Destination Warehouse ID.';
      return;
    }
    this.loading = true;
    this.warehouseService.transferLot(lot_id, Dest_id).subscribe({
      next: (data) => {
        console.log(data);
        this.message = data.message || 'Lot successfully transferred!';
        console.log(this.message);
      },
      error: (err) => {
        this.message = `Error transferring lot: ${err.message}`;
      },
      complete: () => (this.loading = false),
    });
  }

  // Track Lot History
  trackLot() {
    const { lotId } = this.warehouseForm.value;
    if (!lotId) {
      this.message = 'Please provide Lot ID to track.';
      return;
    }
    this.loading = true;
    this.warehouseService.trackLot(lotId).subscribe({
      next: (data) => {
        this.lotHistory = data;
        console.log(this.lotHistory);
        this.message = '';
      },
      error: (err) => {
        this.message = `Error tracking lot: ${err.message}`;
      },
      complete: () => (this.loading = false),
    });
  }


  // Get all lot movements
  getLotMovements(){
    this.loading = true;
    this.warehouseService.getAllLotMovements().subscribe({
      next: (lotMovements) => {
        this.lotMovement = lotMovements;
        console.log(lotMovements);
        this.message = ''
      },
      error: (err) => {
        this.message = `Error while retrieving the lot Movements: ${err.message}`;
      },
      complete: () => (this.loading = false),
    });
  }
}

import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class WarehouseService {
  private apiUrl = `${environment.apiBaseUrl}/WareHouse`;

  constructor(private http: HttpClient) {}

  // Get list of warehouses
  getWarehouses(): Observable<any> {
    return this.http.get(`${this.apiUrl}/WareHouseList`);
  }

  // Get warehouse lots by ID
  getWarehouseLots(id: number, productId: number): Observable<any> {
    return this.http.get(`${this.apiUrl}/GetWarehouseLots/${id}?product_id=${productId}`);
  }

  // Get transferable warehouses for a lot
  getTransferableWarehouses(lotId: number): Observable<any> {
    return this.http.get(`${this.apiUrl}/GetTransferableWarehouses/${lotId}`);
  }

  // Transfer lot between warehouses
  transferLot(lot_id: number, Dest_id: number): Observable<any> {
    return this.http.post(`${this.apiUrl}/TransferLot/${lot_id}/${Dest_id}`, {});
  }

  // Track lot history
  trackLot(lotId: number): Observable<any> {
    return this.http.get(`${this.apiUrl}/TrackLot/${lotId}`);
  }

  getAllLotMovements(): Observable<any> {
    return this.http.get(`${this.apiUrl}/AllLotMovements`);
  }
}

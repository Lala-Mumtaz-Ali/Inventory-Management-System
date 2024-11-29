export interface Product {
  id: number;
  name: string;
  current_Price: number;
  category_id: number;     
  description: string;
  low_Stock_Threshold: number;
}

  
 export interface Order {
    id: number;
    wareHouse_Id: number;
    totalQuantity: number;
    totalAmount: number;
    is_Dispatched: string;
    shipping_Address: string;
}
  
export interface OrderDispatchDTO {
  order_Id: number;
  wareHouse_Id: number; 
}


export interface OrderItem {
  id: number;             
  order_Id: number;
  product_Id: number;      
  quantity: number;          
  unitPrice: number;          
  sub_Total: number;          
}

export interface Supplier {
  id: number;                  
  name: string;                
  contact: string;             
  category_id: string;              
}


export interface PurchaseOrder {
  id: number;                  
  product_Id: number;          
  wareHouse_Id: string;              
  order_Date: number;         
  quantity: Date;                 
  totalPrice: OrderItem[];
  supplier_Id:number;
  delivery_Date: Date;
  is_Received: number;     
}


export interface LotMovements{
  id: number;
  source: number;
  destination: number;
  product_Id: number;
  lot_Id: number;
  related_Movement: number;
  transaction_date: Date;
}

  
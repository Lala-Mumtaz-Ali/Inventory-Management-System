import { Component } from '@angular/core';
import { ProductService } from '../services/product.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-product',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './product.component.html',
  styleUrls: ['./product.component.css'],
})
export class ProductComponent {
  
  products: any[] = [];
  productLots:any[] = [];
  product: any = {};
  getProduct1: any = {};
  analysis: any = {};
  message: string = '';
  loading: boolean = false;
  productId!: number;
  categoryOptions: string[] = ['Shoes', 'Bags', 'Shirts', 'Pants', 'Accessories'];
  category!: string;
  showProductIdForm: boolean = false;
  showProductAnalysisForm: boolean = false; 
  deleteProductIdForm: boolean = false;
  editingProduct: any = {}; 
  showEditForm: boolean = false; 
  filteredProduct: any = {}; 
  showFilteredProductsForm: boolean = false;
  displayContext: string = '';
  showCreateProduct: boolean = false;
  newProduct: any = {
    name: '',
    category_id: null,
    current_price: null,
    description: '',
    low_stock_threshold: null,
  };
  allProducts: any[] = [];
  showAllProducts:boolean = false;

  categoryMapping: { [key: number]: string } = {
    1: 'Shoes',
    2: 'Bags',
    3: 'Shirts',
    4: 'Pants',
    5: 'Accessories',
  };


  constructor(private productService: ProductService) {}

  // Reset the content of the body (clear any displayed product, messages, etc.)
  resetBodyContent() {
    this.product = {};
    this.getProduct1 = {};
    this.analysis = {};
    this.products = [];
    this.allProducts = [];
    this.message = '';
    this.productId = 0;
    this.loading = false;
    this.showProductIdForm = false;
    this.showProductAnalysisForm = false;
    this.showFilteredProductsForm = false;
    this.deleteProductIdForm = false;
    this.showEditForm = false;
    this.showCreateProduct = false;
    this.newProduct = {
      name: '',
      category_id: null,
      current_price: null,
      description: '',
      low_stock_threshold: null,
    };
    this.showAllProducts = false;
    this.productLots = [];
  }

  // Toggle the display of the Product ID form
  toggleProductIdForm() {
    this.resetBodyContent(); // Reset body content when clicking the option
    this.showProductIdForm = !this.showProductIdForm;
  }

  // Toggle the display of the Product Analysis form
  toggleProductAnalysisForm() {
    this.resetBodyContent(); // Reset body content when clicking the option
    this.showProductAnalysisForm = !this.showProductAnalysisForm;
  }

  // Toggle the display of the Create Product Form
  toggleCreateProductForm() {
    this.resetBodyContent(); // Reset body content when clicking the option
    this.showCreateProduct = !this.showCreateProduct;
  }
  
  // Toggle the Filtered Products form
  toggleFilteredProductForm(){
    this.resetBodyContent();
    this.showFilteredProductsForm = !this.showFilteredProductsForm;
  }

  // Toggle the Edit Product form
  toggleEditProductForm() {
    this.resetBodyContent();
    this.showEditForm = !this.showEditForm;
  }

  toggleDeleteIdForm(){
    this.resetBodyContent();
    this.deleteProductIdForm = !this.deleteProductIdForm;
  }

  toggleAllProductLotsForm(){
    this.resetBodyContent();
    this.getAllProductLots();
  }

  //Get all Products
  fetchAllProducts(){
    this.resetBodyContent();
    this.productService.getAllProducts().subscribe({
      next: (data) => {
        this.allProducts = data;
      },
      error: (err) => {
        console.error('Error fetching products:', err);
        alert('Failed to load products. Please try again later.');
      }
    });
  }

  
  // Get Product by ID
  getProduct(id: number) {
    this.resetBodyContent(); // Reset body content before fetching the product
    this.loading = true;
    this.productService.getProduct(id).subscribe({
      next: (data) => {
        this.getProduct1 = data;
        this.message = '';
      },
      error: (err) => {
        this.message = `Error fetching product: ${err}`;
      },
      complete: () => (this.loading = false),
    });
  }

  // Handle form submission to fetch product by ID
  getProductById() {
    if (this.productId) {
      this.displayContext = "Product Details"
      this.getProduct(this.productId); // Fetch the product with the provided ID
    } else {
      this.message = 'Please enter a valid product ID!';
    }
    this.showProductIdForm = false; // Hide the form after submission
  }

  // Get Product Analysis by ID
  getProductAnalysis(id: number) {
    this.resetBodyContent(); // Reset body content before fetching the product analysis
    this.loading = true;
    this.productService.getProductAnalysis(id).subscribe({
      next: (data) => {
        console.log('Product Analysis Data:', data); // Log the response to ensure it's correct
        this.analysis = data;  // Assign the full response to `analysis` object
        this.message = '';
      },
      error: (err) => {
        console.error('Error fetching analysis:', err);
        this.message = `Error fetching analysis: ${err}`;
      },
      complete: () => (this.loading = false),
    });
  }
  

  // Handle form submission to fetch product analysis by ID
  getProductAnalysisById() {
    if (this.productId) {
      this.getProductAnalysis(this.productId); // Fetch the product analysis with the provided ID
    } else {
      this.message = 'Please enter a valid product ID for analysis!';
    }
    this.showProductAnalysisForm = false; // Hide the form after submission
  }


  // Main Filtered Products by Category
  getMainFilteredProductsbyCategory() {
    if (this.category) {
      this.getMainFilteredProducts(this.category); // Call the service to update the product
    } else {
      this.message = 'Enter the correct category';
    }
    this.showEditForm = false; // Hide the form after submission
  }
  
  // Get Products by Category
  getMainFilteredProducts(category: string) {
    this.resetBodyContent(); // Reset body content before fetching the products
    this.loading = true;
    this.productService.getMainFilteredProducts(category).subscribe({
      next: (data) => {
        if (data.length > 0) {
          this.products = data;
          this.message = '';
        } else {
          this.message = 'No products found for the specified category.';
        }
      },
      error: (err) => {
        console.error('Error fetching filtered products:', err);
        this.message = `Error fetching products: ${err}`;
      },
      complete: () => (this.loading = false),
    });
  }


  createProductInputs() {
    // Check if all required fields are filled
    if (
      this.product.name &&
      this.product.category_id &&
      this.product.current_price &&
      this.product.description &&
      this.product.low_stock_threshold
    ) {
      this.createProduct(this.product);
      this.showCreateProduct = false; // Hide the form after submission
    } else {
      this.message = 'Please fill out all required fields to create a product!';
    }
  }

  createProduct(product: any) {
    this.loading = true; // Show loading spinner
    this.productService.createProduct(product).subscribe({
      next: (data) => {
        this.message = 'Product successfully added to the database!';
        this.product = {}; // Reset the product form
      },
      error: (err) => {
        this.message = `Error creating product: ${err}`;
      },
      complete: () => {
        this.loading = false; // Hide loading spinner
      },
    });
  }

  editProductById() {
    if (this.productId && this.editingProduct) {
      this.displayContext = "Edited Details";
      this.editProduct(this.productId, this.editingProduct); // Call the service to update the product
    } else {
      this.message = 'Please fill out all fields to edit the product!';
    }
    this.showEditForm = false; // Hide the form after submission
  }

  // Edit Product by ID
  editProduct(id: number, product: any) {
    this.resetBodyContent(); // Reset body content before editing a product
    this.loading = true;
    this.productService.editProduct(id, product).subscribe({
      next: (data) => {
        this.message = data.message;
      },
      error: (err) => {
        this.message = `Error editing product: ${err}`;
      },
      complete: () => (this.loading = false),
    });
  }

  deleteById() {
    if (this.productId) {
      this.deleteProduct(this.productId); // Fetch the product with the provided ID
    } else {
      this.message = 'Please enter a valid product ID!';
    }
    this.showProductIdForm = false; // Hide the form after submission
  }
  
  // Delete Product by ID
  deleteProduct(id: number) {
    this.resetBodyContent(); // Reset body content before deleting a product
    this.loading = true;
    this.productService.deleteProduct(id).subscribe({
      next: (data) => {
        this.message = 'Product deleted successfully';
      },
      error: (err) => {
        this.message = `Product may be in use in Orders table\nError deleting product: ${err}`;
      },
      complete: () => (this.loading = false),
    });
  }


  getAllProductLots(): void {
    this.productService.getAllProductLots().subscribe(
      (data) => {
        this.productLots = data;
      },
      (error) => {
        console.error('Error fetching product lots:', error);
        this.message = 'Could not fetch product lots. Please try again later.';
      }
    );
  }
}

using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using Inventory.Models;
using Inventory.Data.Configurations;
using static Inventory.Data.Configurations.LotMovementConfig;




namespace Inventory.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
        public DbSet<Employees> Employees { get; set; }

        public DbSet<Category> Category { get; set; }

        public DbSet<Product> Product { get; set; }
        //public DbSet<Product_Instance> Product_Instance { get; set; }
        //public DbSet<Generic> Generic { get; set; }


        //public DbSet<Attribute_Information> Attribute_Information { get; set; }

        //public DbSet<Attribute_Values> Attribute_Values { get; set; }
        public DbSet<Product_Lot> Product_Lot { get; set; }
        //public DbSet<SubCategory> SubCategories { get; set; }
        //public DbSet<Lot_Attribute>Lot_Attribute { get; set; }
        public DbSet<WareHouse> WareHouse { get; set; }
        public DbSet<LotMovements> LotMovements { get; set; }

        public DbSet<PurchaseOrders> PurchaseOrders { get; set; }
        public DbSet<Supplier> Supplier { get; set; }
        public DbSet<ProductOrderDto> ProductOrderDto { get; set; }
        public DbSet<Orders> Orders { get; set; }

        public DbSet<OrderItems> OrderItems { get; set; }
        protected override void OnModelCreating(ModelBuilder md)
        {

            // Employees Table Definition

            md.Entity<ProductOrderDto>().HasNoKey();
            md.Entity<MissingProductOrderDto>().HasNoKey();
            //md.ApplyConfiguration(new GenericConfig());
            md.ApplyConfiguration(new CategoryConfig());
            md.ApplyConfiguration(new ProductConfig());
            //md.ApplyConfiguration(new ProductInstanceConfig());
            //md.ApplyConfiguration(new AttributeConfig());
            //md.ApplyConfiguration(new LotAttributeConfig());
            md.ApplyConfiguration(new ProductLotConfig());
            md.ApplyConfiguration(new WareHouseConfig());
            md.ApplyConfiguration(new LotMovementConfig());
            md.ApplyConfiguration(new SupplierConfig());
            md.ApplyConfiguration(new PurchaseOrderConfig());

            md.Entity<Employees>(entity =>
            {
                entity.HasKey(e => e.user_name);
                entity.Property(e => e.user_name)
                  .HasMaxLength(20);

                entity.HasAlternateKey(e => e.password);
                entity.Property(e => e.password)
                 .HasMaxLength(20);

                entity.Property(e => e.contact_no)

                    .HasMaxLength(10);

                entity.Property(e => e.role)
                  .IsRequired();

                entity.Property(e => e.email)
                  .IsRequired();

                entity.Property(e => e.name)
                  .HasMaxLength(50);


                entity.ToTable(t => t.HasCheckConstraint("CK_Role", "role IN ('Admin', 'Inventory_Manager', 'Warehouse_Manager', 'Purchasing_Agent', 'Finance')"));

            });


            base.OnModelCreating(md);
        }
    }
}



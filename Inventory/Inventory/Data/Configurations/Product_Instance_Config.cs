using Inventory.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pomelo.EntityFrameworkCore.MySql.Query.Expressions.Internal;
using System.Reflection.Emit;

namespace Inventory.Data.Configurations
{
    public class CategoryConfig : IEntityTypeConfiguration<Category>

    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(c => c.Id);
            builder.HasAlternateKey(c => c.Code);

            builder.Property(c => c.Name)
                .IsRequired();


        }
    }



    public class ProductConfig : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {

            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id)
                .ValueGeneratedOnAdd();

            builder.HasAlternateKey(p => p.Name);

            builder.HasOne<Category>()
                  .WithMany()
                  .HasForeignKey(p => p.Category_id)
                  .OnDelete(DeleteBehavior.Cascade);

            builder.Property(p => p.Category_id)
                .IsRequired();

            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(50);



            builder.Property(p => p.Current_Price)
                .IsRequired();
            builder.ToTable(t => t.HasCheckConstraint("CK_Price", "current_price>=0"));

            builder.Property(p => p.Description)
                .HasMaxLength(200);

            builder.Property(p => p.Low_Stock_Threshold)
                .IsRequired();
        }
    }


    public class ProductLotConfig : IEntityTypeConfiguration<Product_Lot>
    {

        public void Configure(EntityTypeBuilder<Product_Lot> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .ValueGeneratedOnAdd();


            builder.HasOne<Product>()
                 .WithMany()
                 .HasForeignKey(p => p.Product_id)
                 .OnDelete(DeleteBehavior.Cascade);

           

            builder.HasOne<WareHouse>()
                 .WithMany()
                 .HasForeignKey(p => p.WareHouse_Id)
                 .OnDelete(DeleteBehavior.Cascade);


            builder.Property(p => p.Quantity)
                .IsRequired();

            builder.ToTable(t => t.HasCheckConstraint("CK_Quantity", "quantity>0"));

            builder.Property(p => p.Product_id)
                .IsRequired();


        }
    }

    public class WareHouseConfig : IEntityTypeConfiguration<WareHouse>
    {
        public void Configure(EntityTypeBuilder<WareHouse> builder)
        {
            builder.HasKey(p => p.Id);




            builder.Property(p => p.Name)
                .IsRequired();

            builder.Property(p => p.Location)
                .IsRequired();


        }


    }

    public class LotMovementConfig : IEntityTypeConfiguration<LotMovements>
    {
        public void Configure(EntityTypeBuilder<LotMovements> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id)
                .ValueGeneratedOnAdd();
            builder.HasOne<WareHouse>()
                  .WithMany()
                  .HasForeignKey(p => p.Source)
                  .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne<WareHouse>()
                  .WithMany()
                  .HasForeignKey(p => p.Destination)
                  .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne<Product>()
                  .WithMany()
                  .HasForeignKey(p => p.Product_Id)
                  .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne<Product_Lot>()
                  .WithMany()
                  .HasForeignKey(p => p.Lot_Id)
                  .OnDelete(DeleteBehavior.SetNull);

            builder.Property(p => p.Transaction_date)
                .IsRequired();

            builder.Property(p => p.Lot_Id)
                .IsRequired();
            builder.Property(p => p.Source)
                .IsRequired();

            builder.Property(p => p.Destination)
                .IsRequired();


        }

        public class SupplierConfig : IEntityTypeConfiguration<Supplier>
        {

            public void Configure(EntityTypeBuilder<Supplier> builder)
            {
                builder.HasKey(p => p.Id);

                builder.Property(p => p.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                builder.Property(p => p.Contact)
                    .IsRequired()
                    .HasMaxLength(11);

                builder.HasOne<Category>()
                  .WithMany()
                  .HasForeignKey(p => p.Category_id)
                  .OnDelete(DeleteBehavior.Cascade);


            }
        }
        public class PurchaseOrderConfig : IEntityTypeConfiguration<PurchaseOrders>
        {

            public void Configure(EntityTypeBuilder<PurchaseOrders> builder)
            {
                builder.HasKey(p => p.Id);
                builder.Property(p => p.Id)
                    .ValueGeneratedOnAdd();

                builder.HasOne<Product>()
                  .WithMany()
                  .HasForeignKey(p => p.Product_Id)
                  .OnDelete(DeleteBehavior.Cascade);


                builder.HasOne<Supplier>()
                  .WithMany()
                  .HasForeignKey(p => p.Supplier_Id)
                  .OnDelete(DeleteBehavior.Cascade);

                builder.Property(p => p.TotalPrice)
                    .IsRequired();

                builder.Property(p => p.Quantity)
                    .IsRequired();


                builder.Property(p => p.Product_Id)
                    .IsRequired();
                builder.Property(p => p.Supplier_Id)
                    .IsRequired();


            }
        }
    }
}




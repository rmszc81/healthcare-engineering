using System;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Healthcare.Engineering.Migrations;

using Database.Model;

[DbContext(typeof(Context))]
public class ContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
        #pragma warning disable 612, 618
        modelBuilder.HasAnnotation("ProductVersion", "7.0.5");

        modelBuilder.Entity("Healthcare.Engineering.Database.Model.Customer", b =>
        {
            b.Property<Guid>("Id")
                .HasColumnType("TEXT")
                .HasColumnName("id");

            b.Property<string>("FirstName")
                .IsRequired()
                .HasMaxLength(256)
                .HasColumnType("TEXT")
                .HasColumnName("first_name");

            b.Property<string>("LastName")
                .IsRequired()
                .HasMaxLength(256)
                .HasColumnType("TEXT")
                .HasColumnName("last_name");
            
            b.Property<string>("Email")
                .IsRequired()
                .HasMaxLength(256)
                .HasColumnType("TEXT")
                .HasColumnName("email");
            
            b.Property<string>("PhoneNumber")
                .IsRequired()
                .HasMaxLength(256)
                .HasColumnType("TEXT")
                .HasColumnName("phone_number");

            b.HasKey("Id")
                .HasName("customer_id_pk");

            b.ToTable("customer", (string?)null);
        });
        #pragma warning restore 612, 618
    }
}
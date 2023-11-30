using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Healthcare.Engineering.Database.Model.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> entityBuilder)
    {
        entityBuilder.ToTable("customer");

        entityBuilder.HasKey(e => e.Id).HasName("customer_id_pk");

        entityBuilder.Property(e => e.Id).ValueGeneratedNever().HasColumnName("id");

        entityBuilder.Property(e => e.FirstName).IsRequired().HasColumnName("first_name");
        
        entityBuilder.Property(e => e.LastName).IsRequired().HasColumnName("last_name");
        
        entityBuilder.Property(e => e.Email).IsRequired().HasColumnName("email");
        
        entityBuilder.Property(e => e.PhoneNumber).IsRequired().HasColumnName("phone_number");
    }
}
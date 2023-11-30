using System;

using Microsoft.EntityFrameworkCore;

namespace Healthcare.Engineering.Database.Model;

public class Context : DbContext
{
    public Context() { }

    public Context(DbContextOptions<Context> options) : base(options) { }
    
    public virtual DbSet<Customer>? Customer { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
            throw new Exception("Database configuration is not set.");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new Configurations.CustomerConfiguration());
    }
}
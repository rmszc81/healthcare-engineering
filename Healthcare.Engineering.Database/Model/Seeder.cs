using System;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

namespace Healthcare.Engineering.Database.Model;

public class Seeder
{
    private readonly Context _context;

    public Seeder(Context context)
    {
        _context = context;
    }

    public async Task Seed()
    {
        if (await _context.Customer!.AnyAsync())
            return;

        await _context.AddAsync(new Customer
        {
            Id = Guid.Parse("5E4EB2F4-7EA4-4F7A-8CC5-8F8E80F32F3D"),
            FirstName = "John",
            LastName = "Stevens",
            Email = "john@stevens.com",
            PhoneNumber = "+1 1234567890"
        });

        await _context.AddAsync(new Customer
        {
            Id = Guid.Parse("B2D5EFCD-AF4D-41E6-946B-102C693D8E3B"),
            FirstName = "Steven",
            LastName = "Smith",
            Email = "steven@smith.com",
            PhoneNumber = "+1 0987654321"
        });

        await _context.AddAsync(new Customer
        {
            Id = Guid.Parse("38C07E1C-2B62-4C73-B2C9-6A268482F932"),
            FirstName = "Alice",
            LastName = "Johnson",
            Email = "alice@johnson.com",
            PhoneNumber = "+1 5678901234"
        });

        await _context.AddAsync(new Customer
        {
            Id = Guid.Parse("7F3E4D5B-9A2C-4E5A-A89B-3A22C1B4B839"),
            FirstName = "Michael",
            LastName = "Brown",
            Email = "michael@brown.com",
            PhoneNumber = "+1 9876543210"
        });


        await _context.AddAsync(new Customer
        {
            Id = Guid.Parse("A1F5E6D8-2C9B-43C7-964A-9D84B1E83271"),
            FirstName = "Emily",
            LastName = "Davis",
            Email = "emily@davis.com",
            PhoneNumber = "+1 2345678901"
        });

        await _context.SaveChangesAsync();
    }
}
using System;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

namespace Healthcare.Engineering.Validator;

using Healthcare.Engineering.Database.Model;

public class ValidatorSupport
{
    private readonly Context _context;

    public ValidatorSupport(Context context) =>
        _context = context;

    public async Task<bool> EmailExists(string email, Guid id) =>
        await _context.Customer!.AnyAsync(c => c.Email == email && c.Id != id);
    
    public async Task<bool> IdExists(Guid id) =>
        await _context.Customer!.AnyAsync(c => c.Id == id);
}
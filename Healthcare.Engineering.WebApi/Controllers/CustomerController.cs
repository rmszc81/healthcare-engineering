using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using AutoMapper;

using FluentValidation.Results;

namespace Healthcare.Engineering.WebApi.Controllers;

using Healthcare.Engineering.Database.Model;
using Healthcare.Engineering.DataObject.Data;
using Healthcare.Engineering.Validator;

[Route("api/[controller]")]
[ApiController]
public class CustomerController : ControllerBase
{
    private readonly Context _context;
    private readonly CustomerCreateValidator _createValidator;
    private readonly CustomerDeleteValidator _deleteValidator;
    private readonly CustomerUpdateValidator _updateValidator;
    private readonly ILogger<CustomerController> _logger;
    private readonly IMapper _mapper;
    private readonly Healthcare.Engineering.Services.Interfaces.IDocumentService _documentService;
    private readonly Healthcare.Engineering.Services.Interfaces.IEmailService _emailService;

    public CustomerController(Context context, CustomerCreateValidator createValidator,
        CustomerDeleteValidator deleteValidator, CustomerUpdateValidator updateValidator,
        ILogger<CustomerController> logger, IMapper mapper, Healthcare.Engineering.Services.Interfaces.IDocumentService documentService,
        Healthcare.Engineering.Services.Interfaces.IEmailService emailService)
    {
        _context = context;
        _createValidator = createValidator;
        _deleteValidator = deleteValidator;
        _updateValidator = updateValidator;
        _logger = logger;
        _mapper = mapper;
        _documentService = documentService;
        _emailService = emailService;
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string query)
    {
        _logger.LogInformation("Search customer invoked.");

        if (string.IsNullOrEmpty(query))
            return BadRequest("Query parameter is required.");

        var lowerQuery = query.ToLower();

        var results = await _context.Customer!
            .Where(w => w.FirstName!.ToLower().Contains(lowerQuery) || w.LastName!.ToLower().Contains(lowerQuery))
            .ToListAsync();

        if (!results.Any())
            return NotFound();

        _logger.LogInformation("Search customer request finished.");
        return Ok(results);
    }

    [HttpGet("list")]
    public async Task<IActionResult> List()
    {
        _logger.LogInformation("List customer invoked.");

        var results = await _context.Customer!.ToListAsync();

        if (!results.Any())
            return NotFound();

        _logger.LogInformation("List customer request finished.");
        return Ok(results);
    }

    [HttpGet]
    public async Task<IActionResult> Get(Guid id)
    {
        _logger.LogInformation("Get customer invoked.");

        var result = await _context.Customer!.SingleOrDefaultAsync(w => w.Id == id);

        if (result == null)
            return NotFound();

        _logger.LogInformation("Get customer request finished.");
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CustomerDto customerDto)
    {
        _logger.LogInformation("Create customer invoked.");

        var errors = await ValidateCustomerObject(OperationType.Create, customerDto);
        if (errors.Any())
            return BadRequest(errors);

        var customer = _mapper.Map<Customer>(customerDto);

        _context.Customer!.Add(customer);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Sending e-mail...");
        await _emailService.Send(customerDto.Email!, $"Customer {customerDto.Id} was created!");

        _logger.LogInformation("Synchronizing documents...");
        await _documentService.SyncDocumentsFromExternalSource(customerDto.Email!);

        _logger.LogInformation("Create customer request finished.");
        return Created(new Uri($"/{ControllerContext.ActionDescriptor.ControllerName}/{customer.Id}", UriKind.Relative),
            customerDto);
    }

    [HttpPut]
    public async Task<IActionResult> Put([FromBody] CustomerDto customerDto)
    {
        _logger.LogInformation("Update customer invoked.");

        var errors = await ValidateCustomerObject(OperationType.Update, customerDto);
        if (errors.Any())
            return BadRequest(errors);

        var oldCustomerData = await _context.Customer!.AsNoTracking().FirstAsync(w => w.Id == customerDto.Id);
        var newCustomerData = _mapper.Map<Customer>(customerDto);

        if (!oldCustomerData.Email!.Equals(newCustomerData.Email, StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning("Customer e-mail was changed!");

            _logger.LogInformation("Sending e-mail...");
            await Task.Run(async () =>
            {
                await _emailService.Send(customerDto.Email!, $"Customer {customerDto.Id} was created!");
            });

            _logger.LogInformation("Synchronizing documents...");
            await Task.Run(async () => { await _documentService.SyncDocumentsFromExternalSource(customerDto.Email!); });
        }

        _context.Attach(newCustomerData).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        _logger.LogInformation("Update customer request finished.");
        return Ok(newCustomerData);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(Guid id)
    {
        _logger.LogInformation("Delete customer invoked.");

        var errors = await ValidateCustomerObject(OperationType.Delete, new CustomerDto { Id = id });
        if (errors.Any())
            return BadRequest(errors);

        var customer = await _context.Customer!.FirstAsync(w => w.Id == id);

        _context.Remove(customer);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Delete customer request finished.");
        return NoContent();
    }

    private async Task<string[]> ValidateCustomerObject(OperationType operationType, CustomerDto customerDto)
    {
        _logger.LogInformation("Validating customer object.");

        var validationResult = operationType switch
        {
            OperationType.Create => await _createValidator.ValidateAsync(customerDto),
            OperationType.Update => await _updateValidator.ValidateAsync(customerDto),
            OperationType.Delete => await _deleteValidator.ValidateAsync(customerDto),
            _ => null
        };

        if (validationResult!.IsValid)
            return Array.Empty<string>();

        LogErrorMessages(validationResult.Errors);

        return validationResult.Errors.Select(s => s.ErrorMessage).ToArray();
    }

    private void LogErrorMessages(IEnumerable<ValidationFailure> errors)
    {
        foreach (var error in errors)
            _logger.LogError(
                "Property {PropertyName}: {ErrorMessage}", error.PropertyName, error.ErrorMessage);
    }
}
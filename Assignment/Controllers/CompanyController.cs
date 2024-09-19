using Assignment.Entity;
using Assignment.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Assignment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly CompanyContext _context;
        private readonly ILogger<CompanyController> _logger;
        public CompanyController(CompanyContext context, ILogger<CompanyController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("Getting all companies");

            var companies = await _context.Company.ToListAsync();            
            return Ok(companies);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Company>> GetById(int id)
        {
            _logger.LogInformation("Getting company with Id {id}", id);

            var company = await _context.Company.FirstOrDefaultAsync(x => x.Id == id);

            if (company == null)
            {
                var message = $"Company with Id {id} not found";
                _logger.LogWarning(message);
                return NotFound(message);
            }

            return Ok(company);
        }

        [HttpPost]
        public async Task<ActionResult<Company>> Add(AddCompanyDTO company)
        {
            _logger.LogInformation("Adding new company: {addedCompany}", JsonSerializer.Serialize(company));

            var addedCompany = new Company
            {
                Name = company.Name,
                TotalEmployee = company.TotalEmployee,
                EstablishedOn = company.EstablishedOn
            };

            _context.Company.Add(addedCompany);
            await _context.SaveChangesAsync();

            return Ok(addedCompany);
        }

        [HttpPut]
        public async Task<ActionResult<Company>> Update(Company updatedCompany)
        {
            _logger.LogInformation("Updating company: {updatedCompany}", JsonSerializer.Serialize(updatedCompany));

            var dbCompany = await _context.Company.FirstOrDefaultAsync(x => x.Id == updatedCompany.Id);
            if (dbCompany == null)
            {
                var message = $"Company with Id {updatedCompany.Id} not found";
                _logger.LogWarning(message);
                return BadRequest(message);
            }

            dbCompany.Name = updatedCompany.Name;
            dbCompany.TotalEmployee = updatedCompany.TotalEmployee;
            dbCompany.EstablishedOn = updatedCompany.EstablishedOn;
            
            await _context.SaveChangesAsync();

            return Ok(dbCompany);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            _logger.LogInformation("Deleting company with Id {id}", id);

            var dbCompany = await _context.Company.FirstOrDefaultAsync(x => x.Id == id);
            if (dbCompany != null)
            {
                _context.Company.Remove(dbCompany);
                await _context.SaveChangesAsync();
            }

            return Ok();
        }
    }
}

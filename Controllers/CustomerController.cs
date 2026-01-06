using LibraryManagement.Data;
using LibraryManagement.Models;
using LibraryManagement.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace LibraryManagement.Controllers
{
    [Authorize]
    public class CustomerController : Controller
{
    private readonly AppDbContext _context;

    public CustomerController(AppDbContext context)
    {
        _context = context;
    }
    
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        return View(await _context.Customers.ToListAsync());
    }


    public IActionResult Details(int id)
    {
        var customer = _context.Customers.FirstOrDefault(c => c.CustomerId == id);
        if (customer == null)
        {
            return NotFound();
        }
        return View(customer);
    }
    
    //handle create with same id
    private bool CustomerExists(int id, int? existingId = null)
    {
        return _context.Customers.Any(c => c.CustomerId == id && (!existingId.HasValue || c.CustomerId != existingId));
    }

    //create
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CustomerViewModel customerView)
    {
        var customer = new Customer{
            CustomerId = customerView.CustomerId,
            FullName = customerView.FullName,
            Email = customerView.Email,
            Address = customerView.Address
        };

        if (CustomerExists(customerView.CustomerId))
        {
            ModelState.AddModelError("CustomerId", "A customer with this ID already exists.");
            return View(customerView);
        }

        await _context.Customers.AddAsync(customer);
        await _context.SaveChangesAsync();
    
        return RedirectToAction("Index");
    }
    
    //edit
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var customer = await _context.Customers.FindAsync(id);

        if (customer == null)
        {
            return NotFound();
        }
        return View(customer);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Customer customerView)
    {
        var customer = await _context.Customers.FindAsync(customerView.CustomerId);
        
        if(customer is not null){
            customer.FullName = customerView.FullName;
            customer.Email = customerView.Email;
            customer.Address = customerView.Address;

            await _context.SaveChangesAsync();
        }
        return RedirectToAction("Index", "Customer");
    }

    //delete
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var customer = await _context.Customers.FindAsync(id);

        if (customer == null)
        {
            return NotFound();
        }
        return View(customer);
    }
    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var customer = await _context.Customers.FindAsync(id);

        if (customer != null){
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction("Index");
    }
}
}
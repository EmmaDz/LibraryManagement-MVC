using LibraryManagement.Data;
using LibraryManagement.Models;
using LibraryManagement.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace LibraryManagement.Controllers
{
    public class LibraryBranchController : Controller
{
    private readonly AppDbContext _context;

    public LibraryBranchController(AppDbContext context)
    {
        _context = context;
    }
    
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        return View(await _context.LibraryBranches.ToListAsync());
    }

    //Datils
    public IActionResult Details(int id)
    {
        var library = _context.LibraryBranches.FirstOrDefault(l => l.BranchId == id);
        if (library == null)
        {
            return NotFound();
        }
        return View(library);
    }

    //handle creat with same id
    private bool LibraryBranchExists(int id, int? existingId = null)
    {
        return _context.LibraryBranches.Any(l => l.BranchId == id && (!existingId.HasValue || l.BranchId != existingId));
    }

    //create
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(LibraryBranchViewModel libraryView)
    {
        var libraryB = new LibraryBranch{
            BranchId = libraryView.BranchId,
            BranchName = libraryView.BranchName,
            Street = libraryView.Street,
            State = libraryView.State,
            Postcode = libraryView.Postcode,
            Telephone = libraryView.Telephone
        };

        if (LibraryBranchExists(libraryView.BranchId))
        {
            ModelState.AddModelError("LibraryBranchId", "A library branch with this ID already exists.");
            return View(libraryView);
        }

        await _context.LibraryBranches.AddAsync(libraryB);
        await _context.SaveChangesAsync();
    
        return RedirectToAction("Index");
    }
    
    //edit
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var lb = await _context.LibraryBranches.FindAsync(id);

        if (lb == null)
        {
            return NotFound();
        }
        return View(lb);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(LibraryBranch lb)
    {
        var libB = await _context.LibraryBranches.FindAsync(lb.BranchId);
        
        if(libB is not null){
            libB.BranchName = lb.BranchName;
            libB.Street = lb.Street;
            libB.State = lb.State;
            libB.Postcode = lb.Postcode;
            libB.Telephone = lb.Telephone;

            await _context.SaveChangesAsync();
        }
        return RedirectToAction("Index");
    }

    //delete
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var lb = await _context.LibraryBranches.FindAsync(id);

        if (lb == null)
        {
            return NotFound();
        }
        return View(lb);
    }
    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var lb = await _context.LibraryBranches.FindAsync(id);

        if (lb != null){
            _context.LibraryBranches.Remove(lb);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction("Index");
    }
}
}
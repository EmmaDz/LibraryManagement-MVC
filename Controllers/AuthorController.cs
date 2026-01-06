using LibraryManagement.Data;
using LibraryManagement.Models;
using LibraryManagement.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

namespace LibraryManagement.Controllers
{
    public class AuthorController : Controller
{
    private readonly AppDbContext _context;

    public AuthorController(AppDbContext context)
    {
        _context = context;
    }
    
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        return View(await _context.Authors.ToListAsync());
    }

    //Datils
    public IActionResult Details(int id)
    {
        var author = _context.Authors.FirstOrDefault(a => a.AuthorId == id);
        if (author == null)
        {
            return NotFound();
        }
        return View(author);
    }

    //handle creat with same id
    private bool AuthorExists(int id, int? existingId = null)
    {
        return _context.Authors.Any(a => a.AuthorId == id && (!existingId.HasValue || a.AuthorId != existingId));
    }

    //create
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(AuthorViewModel authorView)
    {
        var author = new Author{
            AuthorId = authorView.AuthorId,
            AuthorName = authorView.AuthorName,
        };

        if (AuthorExists(authorView.AuthorId))
        {
            ModelState.AddModelError("AuthorId", "An author with this ID already exists.");
            return View(authorView);
        }

        await _context.Authors.AddAsync(author);
        await _context.SaveChangesAsync();
    
        return RedirectToAction("Index");
    }
    
    //edit
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var author = await _context.Authors.FindAsync(id);

        if (author == null)
        {
            return NotFound();
        }
        return View(author);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Author author)
    {
        var au = await _context.Authors.FindAsync(author.AuthorId);
        
        if(au is not null){
            au.AuthorName = author.AuthorName;

            await _context.SaveChangesAsync();


        }
        return RedirectToAction("Index");
    }

    //delete
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var author = await _context.Authors.FindAsync(id);

        if (author == null)
        {
            return NotFound();
        }
        return View(author);
    }
    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var author = await _context.Authors.FindAsync(id);

        if (author != null){
            _context.Authors.Remove(author);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction("Index");
    }
}
}
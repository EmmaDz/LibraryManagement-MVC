using Global.App.Middlewares;
using LibraryManagement.Data;
using LibraryManagement.Models;
using LibraryManagement.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace LibraryManagement.Controllers
{
    public class BookController : Controller
{
    private readonly AppDbContext _context;

    public BookController(AppDbContext context)
    {
        _context = context;
    }
    
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        return View(await _context.Books.ToListAsync());
    }

    //Details
    public IActionResult Details(int id)
    {
        var book = _context.Books.FirstOrDefault(b => b.BookId == id);
        if (book == null)
        {
            return NotFound();
        }
        return View(book);
    }

    //handle creat with same id
    private bool BookExists(int id, int? existingId = null)
    {
        return _context.Books.Any(b => b.BookId == id && (!existingId.HasValue || b.BookId != existingId));
    }

    //create
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(BookViewModel bookView)
    {
        if (BookExists(bookView.BookId))
        {
            // This will trigger the middleware to handle the exception.
            throw new DuplicateBookException($"A book with ID {bookView.BookId} already exists.");
        }

        var book = new Book{
            BookId = bookView.BookId,
            Title = bookView.Title,
            Author = bookView.Author,
            Branch = bookView.Branch,
        };

        await _context.Books.AddAsync(book);
        await _context.SaveChangesAsync();
    
        return RedirectToAction("Index");
    }
    
    //edit
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var book = await _context.Books.FindAsync(id);

        if (book == null)
        {
            return NotFound();
        }
        return View(book);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Book book)
    {
        var b = await _context. Books.FindAsync(book.BookId);
        
        if(b is not null){
            b.Title = book.Title;
            b.Author = book.Author;
            b.Branch = book.Branch;

            await _context.SaveChangesAsync();
        }
        return RedirectToAction("Index");
    }

    //delete
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var book = await _context.Books.FindAsync(id);

        if (book == null)
        {
            return NotFound();
        }
        return View(book);
    }
    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var book = await _context.Books.FindAsync(id);

        if (book != null){
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction("Index");
    }
}
}
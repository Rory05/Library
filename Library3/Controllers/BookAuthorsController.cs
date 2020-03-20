using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Library3;

namespace Library3.Controllers
{
    public class BookAuthorsController : Controller
    {
        private readonly LibraryContext _context;

        public BookAuthorsController(LibraryContext context)
        {
            _context = context;
        }

        // GET: BookAuthors
        public async Task<IActionResult> Index(int? id, string? name)
        {
            ViewBag.authorName = name;
            ViewBag.authorId = id;
            var libraryContext = _context.BookAuthors
                .Where(b => b.AuthorId == id)
                .Include(b => b.Author)
                .Include(b => b.Book)
                .Include(b => b.Book.Language);
            return View(await libraryContext.ToListAsync());
        }

        // GET: BookAuthors/Details/5
        public async Task<IActionResult> Details(int? id, string? name)
        {
            
            if (id == null)
            {
                return NotFound();
            }

            var bookAuthors = await _context.BookAuthors
                .Include(b => b.Author)
                .Include(b => b.Book)
                .Include(b => b.Book.Language)
                .FirstOrDefaultAsync(m => m.Id == id);
            var bgenr = from bg in _context.BookGenres
                        where bg.BookId == bookAuthors.BookId
                        select bg;
            List<Genres> genres = new List<Genres>();

            foreach (var bg in bgenr)
            {
                var genr = from g in _context.Genres
                           where g.Id == bg.GenreId
                           select g;
                foreach (var gen in genr)
                {
                    genres.Add(gen);
                }
            }
            ViewData["genreName"] = genres;

            var authbook = from ab in _context.BookAuthors
                           where ab.BookId == bookAuthors.BookId
                           select ab;
            List<Authors> authors = new List<Authors>();
            foreach (var ab in authbook)
            {
                var auth = from a in _context.Authors
                           where a.Id == ab.AuthorId
                           select a;
                foreach (var au in auth)
                { authors.Add(au); }
            }
            ViewData["authorName"] = authors;

            ViewBag.bookName = bookAuthors.Book.Name;
            if (bookAuthors == null)
            {
                return NotFound();
            }

            return View(bookAuthors);
        }

        // GET: BookAuthors/Create
        public IActionResult Create(int? id)
        {
            ViewBag.authorId = id;
            //ViewData["AuthorId"] = new SelectList(_context.Authors, "Id", "AuthorName");
            ViewData["BookId"] = new SelectList(_context.Books, "Id", "Name");
            return View();
        }

        // POST: BookAuthors/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int authorId,[Bind("Id,BookId")] BookAuthors bookAuthors)
        {
            BookAuthors ba = new BookAuthors();
            ba.AuthorId = authorId;
            bookAuthors.AuthorId = authorId;
            ba.BookId = bookAuthors.BookId;
            if (ModelState.IsValid)
            {
                _context.Add(ba);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", new { id = authorId });
            }
            //ViewData["AuthorId"] = new SelectList(_context.Authors, "Id", "AuthorName", bookAuthors.AuthorId);
            ViewData["BookId"] = new SelectList(_context.Books, "Id", "Name", bookAuthors.BookId);
            return View("Index", new { id = authorId });
        }

        // GET: BookAuthors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookAuthors = await _context.BookAuthors.FindAsync(id);
            if (bookAuthors == null)
            {
                return NotFound();
            }
            ViewData["AuthorId"] = new SelectList(_context.Authors, "Id", "AuthorName", bookAuthors.AuthorId);
            ViewData["BookId"] = new SelectList(_context.Books, "Id", "Name", bookAuthors.BookId);
            return View(bookAuthors);
        }

        // POST: BookAuthors/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BookId,AuthorId")] BookAuthors bookAuthors)
        {
            if (id != bookAuthors.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bookAuthors);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookAuthorsExists(bookAuthors.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["AuthorId"] = new SelectList(_context.Authors, "Id", "AuthorName", bookAuthors.AuthorId);
            ViewData["BookId"] = new SelectList(_context.Books, "Id", "Name", bookAuthors.BookId);
            return View(bookAuthors);
        }

        // GET: BookAuthors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookAuthors = await _context.BookAuthors
                .Include(b => b.Author)
                .Include(b => b.Book)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bookAuthors == null)
            {
                return NotFound();
            }

            return View(bookAuthors);
        }

        // POST: BookAuthors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bookAuthors = await _context.BookAuthors.FindAsync(id);
            _context.BookAuthors.Remove(bookAuthors);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookAuthorsExists(int id)
        {
            return _context.BookAuthors.Any(e => e.Id == id);
        }
    }
}

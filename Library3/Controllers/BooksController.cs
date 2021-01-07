using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Library3;
using Microsoft.AspNetCore.Authorization;

namespace Library3.Controllers
{
    public class BooksController : Controller
    {
        private readonly LibraryContext _context;

        public BooksController(LibraryContext context)
        {
            _context = context;
        }

        // GET: Books
        public async Task<IActionResult> Index(string? name)
        {
            ViewBag.bookNmae = name;
            var libraryContext = _context.Books.Include(b => b.Language);
            return View(await libraryContext.ToListAsync());
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id, string? name)
        {
            ViewBag.bookName = name;
            if (id == null)
            {
                return NotFound();
            }

            var books = await _context.Books
                .Include(b => b.Language)
                .FirstOrDefaultAsync(m => m.Id == id);

            var genrebook = from gb in _context.BookGenres
                            where gb.BookId == books.Id
                            select gb;
            List<Genres> genres = new List<Genres>();
            foreach(var gb in genrebook)
            {
                var gen = from g in _context.Genres
                          where g.Id == gb.GenreId
                          select g;
                foreach(var gn in gen)
                { genres.Add(gn); }
            }
            ViewData["genreName"] = genres;

            var authbook = from ab in _context.BookAuthors
                            where ab.BookId == books.Id
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

            if (books == null)
            {
                return NotFound();
            }

            return View(books);
        }

        // GET: Books/Create
        public IActionResult Create()
        {
            ViewData["LanguageId"] = new SelectList(_context.Language, "Id", "Lname");
            return View();
        }
        // POST: Books/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Id,PagesNum,LanguageId,Description")] Books books)
        {
            int counter = 0;
            foreach (var g in _context.Books)
            {
                if (g.Name == books.Name) { counter++; break; }
            }

            if (counter != 0)
            {
                ModelState.AddModelError("Name", "Таке ім'я вже існує");
                ViewData["LanguageId"] = new SelectList(_context.Language, "Id", "Lname", books.LanguageId);
                return View(books);
            } else
            {
                if (books.PagesNum < 0)
                {
                    ModelState.AddModelError("PagesNum", "Кількість сторінок понна бути додатня!");
                    ViewData["LanguageId"] = new SelectList(_context.Language, "Id", "Lname", books.LanguageId);
                    return View(books);
                }
                else
                {

                    if (ModelState.IsValid)
                    {
                        _context.Add(books);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                    ViewData["LanguageId"] = new SelectList(_context.Language, "Id", "Lname", books.LanguageId);
                    return View(books);
                }
            }
        }

        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var books = await _context.Books.FindAsync(id);
            if (books == null)
            {
                return NotFound();
            }
            ViewData["LanguageId"] = new SelectList(_context.Language, "Id", "Lname", books.LanguageId);
            return View(books);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Name,Id,PagesNum,LanguageId,Description")] Books books)
        {
            if (id != books.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(books);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BooksExists(books.Id))
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
            ViewData["LanguageId"] = new SelectList(_context.Language, "Id", "Lname", books.LanguageId);
            return View(books);
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var books = await _context.Books
                .Include(b => b.Language)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (books == null)
            {
                return NotFound();
            }

            return View(books);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var books = await _context.Books.FindAsync(id);
            var booksgenre = from bg in _context.BookGenres
                             where bg.BookId == books.Id
                             select bg;
            foreach(var bg in booksgenre)
            {
                _context.BookGenres.Remove(bg);
            }
            var booksauthor = from ba in _context.BookAuthors
                              where ba.BookId == books.Id
                              select ba;
            foreach (var ba in booksauthor)
            {
                _context.BookAuthors.Remove(ba);
            }
            _context.Books.Remove(books);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BooksExists(int id)
        {
            return _context.Books.Any(e => e.Id == id);
        }
    }
}

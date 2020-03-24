using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Library3;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace Library3.Controllers
{
    public class GenresController : Controller
    {
        private readonly LibraryContext _context;

        public GenresController(LibraryContext context)
        {
            _context = context;
        }
        // GET: Genres
        public async Task<IActionResult> Index()
        {
            return View(await _context.Genres.ToListAsync());
        }

        // GET: Genres/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var genres = await _context.Genres
                .FirstOrDefaultAsync(m => m.Id == id);
            if (genres == null)
            {
                return NotFound();
            }

            return View(genres);
        }

        // GET: Genres/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Genres/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Information")] Genres genres)
        {
            int counter = 0;
            foreach (var g in _context.Genres)
            {
                if (g.Name == genres.Name) { counter++; break; }
            }

            if (counter != 0)
            {
                ModelState.AddModelError("Name", "Таке ім'я вже існує");
                return View(genres);
            }
            else
            {
                if (ModelState.IsValid)
                {
                    _context.Add(genres);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                return View(genres);
            }
        }

        // GET: Genres/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var genres = await _context.Genres.FindAsync(id);
            if (genres == null)
            {
                return NotFound();
            }
            return View(genres);
        }

        // POST: Genres/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Information")] Genres genres)
        {
            if (id != genres.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(genres);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GenresExists(genres.Id))
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
            return View(genres);
        }

        // GET: Genres/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var genres = await _context.Genres
                .FirstOrDefaultAsync(m => m.Id == id);
            if (genres == null)
            {
                return NotFound();
            }

            return View(genres);
        }

        // POST: Genres/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var genres = await _context.Genres.FindAsync(id);
            var booksgenre = from bg in _context.BookGenres
                             where bg.GenreId == genres.Id
                             select bg;
            foreach (var bg in booksgenre)
            {
                _context.BookGenres.Remove(bg);
            }
            _context.Genres.Remove(genres);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GenresExists(int id)
        {
            return _context.Genres.Any(e => e.Id == id);
        }

        ///////////////
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(IFormFile fileExcel)
        {
            if (ModelState.IsValid)
            {
                if (fileExcel != null)
                {
                    using (var stream = new FileStream(fileExcel.FileName, FileMode.Create))
                    {
                        await fileExcel.CopyToAsync(stream);
                        using (XLWorkbook workBook = new XLWorkbook(stream, XLEventTracking.Disabled))
                        {
                            var lang = _context.BookGenres.Include(b => b.Book).Include(b => b.Book.Language).ToList();
                            //перегляд усіх листів (в даному випадку категорій)
                            foreach (IXLWorksheet worksheet in workBook.Worksheets)
                            {
                                //worksheet.Name - назва категорії. Пробуємо знайти в БД, якщо відсутня, то створюємо нову
                                Genres newgen;
                                var c = (from gen in _context.Genres
                                         where gen.Name.Contains(worksheet.Name)
                                         select gen).ToList();
                                if (c.Count > 0)
                                {
                                    newgen = c[0];
                                }
                                else
                                {
                                    newgen = new Genres();
                                    newgen.Name = worksheet.Name;
                                    _context.Genres.Add(newgen);
                                }
                                //перегляд усіх рядків                    
                                foreach (IXLRow row in worksheet.RowsUsed().Skip(1))
                                {
                                    
                                        Books book = new Books();
                                    int counter = 0;
                                    foreach (var bo in _context.Books)
                                    {
                                        if (bo.Name == row.Cell(1).Value.ToString()) { counter++; book = bo; break; }
                                    }
                                    if (counter > 0)
                                    {
                                        int count = 0;
                                        foreach (var gen in _context.BookGenres)
                                        {
                                            if ((book.Id == gen.BookId) && (newgen.Id == gen.GenreId)) { count++; break; }
                                        }if(count>0)
                                        {
                                            goto link1;// якщо такий книжко-жанр вже існує, переходимо до наступного рядка
                                        }else
                                        {
                                            BookGenres bookg = new BookGenres();
                                            bookg.Book = book;
                                            bookg.Genre = newgen;
                                            _context.BookGenres.Add(bookg);
                                            goto link1;// переходимо до наступного рядка, бо вже маємо інформацію про цю книжку
                                        }
                                        
                                    }
                                    else
                                    {
                                        book = new Books();
                                        book.Name = row.Cell(1).Value.ToString();
                                        _context.Books.Add(book);
                                    }
                                        
                                    book.PagesNum = Convert.ToInt32(row.Cell(3).Value);
                                    
                                    Language newlang; int? Lid=1;
                                    counter = 0;
                                    foreach (var la in _context.Language)
                                    {
                                        if (la.Lname == row.Cell(2).Value.ToString()) { counter++; Lid = la.Id; break; }
                                    }
                                    if (counter > 0)
                                    {
                                        book.LanguageId = Lid;
                                    }
                                    else
                                    {
                                        newlang = new Language();
                                        newlang.Lname = row.Cell(2).Value.ToString();
                                        _context.Language.Add(newlang);
                                        book.LanguageId = newlang.Id;
                                    }
                                    
                                        BookGenres bg = new BookGenres();
                                        bg.Book = book;
                                        bg.Genre = newgen;
                                        _context.BookGenres.Add(bg);
                                        
                                        int i = 4;
                                        while (row.Cell(i).Value.ToString().Length > 0)
                                        {
                                        BookAuthors ba = new BookAuthors();
                                        Authors author; Authors auth = new Authors();
                                                counter = 0;
                                                foreach (var au in _context.Authors)
                                                {
                                                   if (au.AuthorName == row.Cell(i).Value.ToString()) { counter++; auth = au; break; }
                                                }

                                                if (counter > 0)
                                                {
                                                  ba.Author = auth;
                                                }
                                                else
                                                {
                                                    author = new Authors();
                                                    author.AuthorName = row.Cell(i).Value.ToString();
                                                    ba.Author = author;
                                                    _context.Add(author);
                                                }
                                                ba.Book = book;
                                                _context.BookAuthors.Add(ba);
                                            i++;
                                        }

                                    link1:;
                                    await _context.SaveChangesAsync();
                                    
                                }
                            }
                        }
                    }
                }

            }
            return RedirectToAction(nameof(Index));
        }

    }
}

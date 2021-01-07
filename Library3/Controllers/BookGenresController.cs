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
using Spire.Doc;
using Spire.Doc.Documents;
using System.Drawing;


namespace Library3.Controllers
{
    public class BookGenresController : Controller
    {
        private readonly LibraryContext _context;

        public BookGenresController(LibraryContext context)
        {
            _context = context;
        }

        // GET: BookGenres
        public async Task<IActionResult> Index(int? id, string? name)
        {
            ViewBag.genreName = name;
            ViewBag.genreId = id;
            var libraryContext = _context.BookGenres
                .Where(b => b.GenreId == id)
                .Include(b => b.Book)
                .Include(b => b.Genre)
                .Include(b=> b.Book.Language);
            return View(await libraryContext.ToListAsync());
        }

        // GET: BookGenres/Details/5
        public async Task<IActionResult> Details(int? id, string? name)
        {
            
            if (id == null)
            {
                return NotFound();
            }

            var bookGenres = await _context.BookGenres
                .Include(b => b.Book)
                .Include(b => b.Genre)
                .Include(b=>b.Book.Language)
                .FirstOrDefaultAsync(m => m.Id == id);
            var bauth = from ba in _context.BookAuthors
                        where ba.BookId == bookGenres.BookId
                        select ba;
            List<Authors> authors = new List<Authors>();

            foreach(var ba in bauth)
            {
                var auth = from a in _context.Authors
                           where a.Id == ba.AuthorId
                           select a;
                foreach(var au in auth)
                {
                    authors.Add(au);
                }
            }
            ViewData["authorName"] = authors;

            var genrebook = from gb in _context.BookGenres
                            where gb.BookId == bookGenres.BookId
                            select gb;
            List<Genres> genres = new List<Genres>();
            foreach (var gb in genrebook)
            {
                var gen = from g in _context.Genres
                          where g.Id == gb.GenreId
                          select g;
                foreach (var gn in gen)
                { genres.Add(gn); }
            }
            ViewData["genreName"] = genres;

            ViewBag.bookName = bookGenres.Book.Name;
            if (bookGenres == null)
            {
                return NotFound();
            }

            return View(bookGenres);
        }

        // GET: BookGenres/Create
        public IActionResult Create(int? id)
        {
            ViewBag.genreId = id;
            ViewData["BookId"] = new SelectList(_context.Books, "Id", "Name");
            //ViewData["GenreId"] = new SelectList(_context.Genres, "Id", "Name");
            return View();
        }

        // POST: BookGenres/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int genreId, [Bind("Id,BookId")] BookGenres bookGenres)
        {
            BookGenres bg = new BookGenres();
            bg.GenreId = genreId;
            bookGenres.GenreId = genreId;
            bg.BookId = bookGenres.BookId;

            if (ModelState.IsValid)
            {
                _context.Add(bg);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", new { id = genreId});
            }
            //ViewData["BookId"] = new SelectList(_context.Books, "Id", "Name", bookGenres.BookId);
            //ViewData["GenreId"] = new SelectList(_context.Genres, "Id", "Name", bookGenres.GenreId);
            return RedirectToAction("Index", new { id = genreId });
        }

        // GET: BookGenres/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookGenres = await _context.BookGenres.FindAsync(id);
            if (bookGenres == null)
            {
                return NotFound();
            }
            ViewData["BookId"] = new SelectList(_context.Books, "Id", "Name", bookGenres.BookId);
            ViewData["GenreId"] = new SelectList(_context.Genres, "Id", "Name", bookGenres.GenreId);
            return View(bookGenres);
        }

        // POST: BookGenres/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,GenreId,BookId")] BookGenres bookGenres)
        {
            if (id != bookGenres.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bookGenres);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookGenresExists(bookGenres.Id))
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
            ViewData["BookId"] = new SelectList(_context.Books, "Id", "Name", bookGenres.BookId);
            ViewData["GenreId"] = new SelectList(_context.Genres, "Id", "Name", bookGenres.GenreId);
            return View(bookGenres);
        }

        // GET: BookGenres/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookGenres = await _context.BookGenres
                .Include(b => b.Book)
                .Include(b => b.Genre)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bookGenres == null)
            {
                return NotFound();
            }

            return View(bookGenres);
        }

        // POST: BookGenres/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bookGenres = await _context.BookGenres.FindAsync(id);
            _context.BookGenres.Remove(bookGenres);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookGenresExists(int id)
        {
            return _context.BookGenres.Any(e => e.Id == id);
        }

        ///////////////////////////////////////////////////////////
        public ActionResult Export(string name, int? id)
        {
            using (XLWorkbook workbook = new XLWorkbook(XLEventTracking.Disabled))
            {
                var genres = _context.BookGenres.Include(b=>b.Book).Include(l=>l.Book.Language).ToList();
                
                    var worksheet = workbook.Worksheets.Add(name);

                    worksheet.Cell("A1").Value = "Назва";
                    worksheet.Cell("B1").Value = "Мова";
                    worksheet.Cell("C1").Value = "К-ть сторінок";
                    worksheet.Cell("D1").Value = "Автор(и)";
                    worksheet.Row(1).Style.Font.Bold = true;
                    var bookgenres = from bookgen in _context.BookGenres
                                where bookgen.GenreId == id
                                select bookgen;
                    List<Books> books = new List<Books>();
                    foreach(var b in bookgenres)
                    {
                        var book = from boo in _context.Books
                                   where boo.Id == b.BookId
                                   select boo;
                        foreach (var bo in book)
                        { books.Add(bo); }
                    }

                    //нумерація рядків/стовпчиків починається з індекса 1 (не 0)
                    for (int i = 0; i < books.Count; i++)
                    {
                    worksheet.Cell(i + 2, 1).Value = books[i].Name;
                    worksheet.Cell(i + 2, 2).Value = books[i].Language.Lname;
                    worksheet.Cell(i + 2, 3).Value = books[i].PagesNum;

                        var ab = _context.BookAuthors.Where(a => a.BookId == books[i].Id).Include("Author").ToList();
                        
                        foreach (var a in ab)
                        {
                                worksheet.Cell(i + 2, 4).Value = a.Author.AuthorName;
                        }

                    }
                
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Flush();

                    return new FileContentResult(stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                    {
                        FileDownloadName = $"library_{DateTime.UtcNow.ToShortDateString()}.xlsx"
                    };
                }
            }
        }
        ////////////////////////////////////
        public ActionResult ExportDocx(string name, int? id)
        {
                Document doc = new Document();
                Section s = doc.AddSection();

            var genres = _context.BookGenres.Include(b => b.Book).Include(l => l.Book.Language).ToList();
                var bookgenres = from bookgen in _context.BookGenres
                                 where bookgen.GenreId == id
                                 select bookgen;
                List<Books> books = new List<Books>();
                foreach (var b in bookgenres)
                {
                    var book = from boo in _context.Books
                               where boo.Id == b.BookId
                               select boo;
                    foreach (var bo in book)
                    { books.Add(bo); }
                }
            ////////////
            ParagraphStyle style1 = new ParagraphStyle(doc);
            style1.Name = "Style1";
            style1.CharacterFormat.Bold = true;
            style1.CharacterFormat.FontSize = 18;
            doc.Styles.Add(style1);
            ////////////
            ParagraphStyle style2 = new ParagraphStyle(doc);
            style2.Name = "Style2";
            style2.CharacterFormat.Italic = true;
            style2.CharacterFormat.FontSize = 14;
            style2.CharacterFormat.TextColor = Color.Blue;
            doc.Styles.Add(style2);
            ////////////
            Paragraph p0 = s.AddParagraph();
            p0.AppendText(name);
            p0.ApplyStyle(style1.Name);

            for (int i = 0; i < books.Count(); i++)
                {
                Paragraph pn = s.AddParagraph();
                pn.AppendText( "Назва:  " + books[i].Name);
                pn.ApplyStyle(style2.Name);

                Paragraph p = s.AddParagraph();
                p.AppendText( "Автор:  ");
                    var ab = _context.BookAuthors.Where(a => a.BookId == books[i].Id).Include("Author").ToList();
                    foreach (var a in ab)
                    {
                       p.AppendText(a.Author.AuthorName + ";  ");
                    }
                    s.AddParagraph().AppendText("К-ть сторінок:  " + books[i].PagesNum);
                    s.AddParagraph().AppendText("Мова:  " + books[i].Language.Lname);
                    s.AddParagraph().AppendText("Опис:  " + books[i].Description);
                    s.AddParagraph();
                }

            using (var stream = new MemoryStream())
            {
                doc.SaveToFile(stream, FileFormat.Docx2013);
                stream.Flush();

                return new FileContentResult(stream.ToArray(), "application/vnd.openxmlformats-officedocument.wordprocessingml.document")
                {
                    FileDownloadName = $"library_{DateTime.UtcNow.ToShortDateString()}.docx"
                };
            }


        }


    }
}

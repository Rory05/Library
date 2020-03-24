using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Library3.Controllers
{
    [Authorize(Roles = "admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class ChartsController : ControllerBase
    {
        private readonly LibraryContext _context;

        public ChartsController(LibraryContext context)
        {
            _context = context;
        }
        [HttpGet("JsonData")]
        public JsonResult JsonData()
        {
            var genres = _context.Language.Include(l => l.Books).ToList();
            List<object> genBook = new List<object>();
            genBook.Add(new[] { "Жанр", "Кількість книжок" });
            foreach(var g in genres)
            {
                genBook.Add(new object[] { g.Lname, g.Books.Count() });
            }
            return new JsonResult(genBook);
        }
        [HttpGet("JsonData1")]
        public JsonResult JsonData1()
        {
            var genres = _context.Genres.Include(l => l.BookGenres).ToList();
            List<object> genBook = new List<object>();
            genBook.Add(new[] { "Жанр", "Кількість книжок" });
            foreach (var g in genres)
            {
                genBook.Add(new object[] { g.Name, g.BookGenres.Count() });
            }
            return new JsonResult(genBook);
        }
    }
}
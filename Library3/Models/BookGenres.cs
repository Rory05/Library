using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Library3
{
    public partial class BookGenres
    {
        public int Id { get; set; }
        public int? GenreId { get; set; }
        public int? BookId { get; set; }

        [Display(Name = "Книжка")]
        public virtual Books Book { get; set; }

        [Display(Name = "Жанр")]
        public virtual Genres Genre { get; set; }
    }
}

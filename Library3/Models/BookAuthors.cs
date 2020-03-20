using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Library3
{
    public partial class BookAuthors
    {
        public int Id { get; set; }
        public int? BookId { get; set; }
        public int? AuthorId { get; set; }

        [Display(Name = "Автор")]
        public virtual Authors Author { get; set; }
        [Display(Name = "Книжка")]
        public virtual Books Book { get; set; }
    }
}
